using Cysharp.Threading.Tasks;
using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEditor.Searcher;
using UnityEngine;

public class FriendPopup : Popup
{
    [field: Title("[ Friend Prefab ]")]
    [field: SerializeField] public GameObject FriendObjectPrefab { get; set; }

    [field: Title("[ View ]")]
    [field: SerializeField] public FriendPopupView View { get; set; }

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public FriendScrollView FriendScrollView { get; set; }
    [field: SerializeField] public FriendScrollView FriendSearchScrollView { get; set; }
    public List<FriendData> FriendList { get => FriendScrollView.GetDataList(); }

    [field: Title("[ Search ]")]
    private CancellationTokenSource tokenSource;
    private float searchTerm = .5f;

    public void Start()
    {
        FriendScrollView.Init(new List<FriendData>());
        FriendSearchScrollView.Init(new List<FriendData>());
        Setting();
    }

    public void Setting()
    {
        if (GameManager.instance.isGuest) return;

        // For Friend
        SettingFriend();

        // for Search
        SettingSearch();
        Searching(View.FriendSearchInputField.text);
    }

    #region Friend
    private void SettingFriend()
    {
        FriendScrollView.ResetData();

        foreach (var friendUuid in DatabaseConnector.instance.memberData.friends_uuid)
        {
            DatabaseConnector.instance.GetMemberData((userData) => Friend(friendUuid, userData), null, friendUuid);
        }
    }

    [Button]
    public void Friend(int uuid, UserDataView userDataView)
    {
        var data = new FriendData(FriendType.Friend, uuid, userDataView);
        FriendScrollView.AddData(data, true, true);
    }
    #endregion

    #region Search

    private void SettingSearch()
    {
        tokenSource = new CancellationTokenSource();
        View.FriendSearchInputField.onValueChanged.AddListener((value) => Searching(value));
    }
    private void Searching(string value)
    {
        if (!tokenSource.IsCancellationRequested)
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
        }
        tokenSource = new CancellationTokenSource();
        SearchTask(value, tokenSource.Token).Forget();
    }

    private async UniTaskVoid SearchTask(string nickName, CancellationToken ct)
    {
        SearchNickItems[] requestData = await SearchTermTask(nickName, ct);
        Search(requestData);
    }

    private async UniTask<SearchNickItems[]> SearchTermTask(string nickName, CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(searchTerm), cancellationToken: ct);

        bool isGet = false;
        SearchNickItems[] requestData = null;
        if (nickName == "")
        {
            isGet = true;
        }
        else
        {
            DatabaseConnector.instance.FindNicknameUUID((data) => { requestData = data; isGet = true; }, null, nickName);
        }
        await UniTask.WaitUntil(() => isGet, cancellationToken: ct);

        return requestData;
    }

    private void Search(SearchNickItems[] searchNickItems)
    {
        FriendSearchScrollView.ResetData();

        if (searchNickItems != null)
        {
            foreach (var search in searchNickItems)
            {
                if (search.uuid != DatabaseConnector.instance.memberUUID)
                {
                    if (FriendList.Exists(x => x.uuid == search.uuid))
                    {
                        var data = new FriendData(FriendType.FollowedSearch, search.uuid, search.info);
                        FriendSearchScrollView.AddData(data, true, true);
                    }
                    else
                    {
                        var data = new FriendData(FriendType.UnFollowedSearch, search.uuid, search.info);
                        FriendSearchScrollView.AddData(data, true, true);
                    }
                }
            }
        }
    }
    #endregion
}