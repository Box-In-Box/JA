using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gongju.Web;
using UnityEngine.Pool;

public class Friend : MonoBehaviour
{
    [field: Title("[ View ]")]
    [field: SerializeField] public FriendView View { get; set; }

    [field: Title("[ Data ]")]
    [field: SerializeField, ReadOnly] public int Uuid { get; set; }
    [field: SerializeField, ReadOnly] public UserDataView Data { get; set; }
    [field: SerializeField, ReadOnly] public bool IsFriend { get; set; }

    public IObjectPool<Friend> MasagedPool { get; set; }

    public void Setting()
    {
        View.ProfileImage.sprite = Data.profile_image ?? View.DefaultSprite;
        View.NickNameText.text = Data.nickname;
        View.LevelText.text = $"Lv.{Data.level}";
        View.MsgText.text = Data.introduce;

        View.ProfileButton.onClick.AddListener(() => Profile(Uuid));
        View.MyRoomButton.onClick.AddListener(() => MyRoom(Uuid));
        View.MyRoomButton.onClick.AddListener(() => More(Uuid));
        View.MyRoomButton.onClick.AddListener(() => Follow(Uuid));
    }

    [Button]
    public void FriendView()
    {
        View.Friend();
        IsFriend = true;
    }

    [Button]
    public void SearchView()
    {
        View.Search();
        IsFriend = false;
    }

    [Button]
    public void SearchRegisteredView()
    {
        View.SearchRegistered();
        IsFriend = true;
    }

    private void Profile(int uuid)
    {
        var popup = PopupManager.instance.Open<ProfilePopup>(CommunityManager.instance.CommunityPrefab.ProfilePopup);
        popup.Setting(uuid);
    }

    private void MyRoom(int uuid)
    {

    }

    private void More(int uuid)
    {

    }

    private void Follow(int uuid)
    {

    }

    public void Release()
    {
        MasagedPool.Release(this);
    }
}
