using Cysharp.Threading.Tasks;
using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class FriendPopup : Popup
{
    [field: Title("[ Friend Prefab ]")]
    [field: SerializeField] public GameObject FriendObjectPrefab { get; set; }

    [field: Title("[ View ]")]
    [field: SerializeField] public FriendPopupView View { get; set; }

    [field: Title("[ Friend List ]")]
    [SerializeField] private List<Friend> firendList;

    [field: Title("[ Search List ]")]
    [SerializeField, Range(1, 30)] private int maxSearchCount = 10;
    [SerializeField] private IObjectPool<Friend> searchPool;
    [SerializeField] private List<Friend> activeSearchs;
    private CancellationTokenSource tokenSource;
    private float searchTerm = .5f;

    public override void Awake()
    {
        base.Awake();

        Setting();
    }

    private void Setting()
    {
        if (GameManager.instance.isGuest) return;

        // For Friend
        foreach (var friend_uuid in DatabaseConnector.instance.memberData.friends_uuid)
        {
            DatabaseConnector.instance.GetMemberData((userData) => SettingFriend(friend_uuid, userData), null, friend_uuid);
        }

        // for Search
        tokenSource = new CancellationTokenSource();
        searchPool = new ObjectPool<Friend>(CreateFriend, OnGetFriend, OnReleaseFriend, OnDestroyFriend, maxSize: maxSearchCount);
        View.FriendSearchInputField.onValueChanged.AddListener((value) => Searching(value));
    }

    #region Friend
    private void SettingFriend(int uuid, UserDataView data)
    {
        var friend = Instantiate(FriendObjectPrefab, View.FriendListContentPanel).GetComponent<Friend>();
        friend.Uuid = uuid;
        friend.Data = data;
        friend.Setting();
        friend.FriendView();
        firendList.Add(friend);
    }
    #endregion

    #region Search
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
        SettingSearch(requestData);
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

    private void SettingSearch(SearchNickItems[] searchNickItems)
    {
        for (int i = activeSearchs.Count -1; i >= 0; i--) 
        {
            activeSearchs[i].Release();
        }

        if (searchNickItems != null)
        {
            int count = 0;

            foreach (var item in searchNickItems)
            {
                if (item.uuid != DatabaseConnector.instance.memberUUID)
                {
                    var search = searchPool.Get();
                    search.Uuid = item.uuid;
                    search.Data = item.info;
                    search.Setting();

                    if (firendList.Exists(x => x.Uuid == search.Uuid))
                    {
                        search.SearchRegisteredView();
                    }
                    else
                    {
                        search.SearchView();
                    }
                }

                if (++count > maxSearchCount)
                {
                    break;
                }
            }
        }
    }

    #region Search Pool
    private Friend CreateFriend()
    {
        var friend = Instantiate(FriendObjectPrefab, View.FriendSearchContentPanel).GetComponent<Friend>();
        friend.MasagedPool = searchPool;
        return friend;
    }

    private void OnGetFriend(Friend friend)
    {
        activeSearchs.Add(friend);
        friend.gameObject.SetActive(true);
    }

    private void OnReleaseFriend(Friend friend)
    {
        activeSearchs.Remove(friend);
        friend.gameObject.SetActive(false);
    }

    private void OnDestroyFriend(Friend friend)
    {
        activeSearchs.Remove(friend);
        Destroy(friend.gameObject);
    }
    #endregion

    #endregion
}