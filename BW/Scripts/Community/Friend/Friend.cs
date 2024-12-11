using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gongju.Web;
using System;
using UnityEngine.UI;
using DG.Tweening.Plugins.Options;
using UnityEditor;

public enum FriendType
{
    Friend,
    UnFollowedSearch,
    FollowedSearch,
}

[Serializable]
public class FriendData
{
    public FriendType friendType;
    public int uuid;
    public UserDataView userDataView;

    public FriendData(FriendType friendType, int uuid, UserDataView userDataView)
    {
        this.friendType = friendType;
        this.uuid = uuid;
        this.userDataView = userDataView;
    }
}
public class Friend : DynamicScrollViewItem<FriendData>
{
    [field: Title("[ View ]")]
    [field: SerializeField] public FriendView View { get; set; }

    [field: Title("[ Data ]")]
    [field: SerializeField, ReadOnly] public FriendData Data { get; set; }
    public override RectTransform Rect { get => View.Rect; }
    public override ContentSizeFitter SizeFitter { get => View.SizeFitter; }

    private void Start()
    {
        View.ProfileButton.onClick.AddListener(() => Profile());
        View.MyRoomButton.onClick.AddListener(() => MyRoom());
        View.MoreButton.onClick.AddListener(() => More());
        View.FollowButton.onClick.AddListener(() => Follow());
    }

    public override FriendData GetData()
    {
        return Data;
    }

    public override void OnUpdate(FriendData data)
    {
        Data = data;
        SetView(data.friendType);

        View.ProfileImage.sprite = data.userDataView.profile_image ?? View.DefaultSprite;
        View.NickNameText.text = data.userDataView.nickname;
        View.LevelText.text = $"Lv.{data.userDataView.level}";
        View.MsgText.text = data.userDataView.introduce;

        LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    }

    private void SetView(FriendType friendType)
    {
        switch (friendType)
        {
            case FriendType.Friend:
                View.Friend();
                break;
            case FriendType.UnFollowedSearch:
                View.UnFollowedSearch();
                break;
            case FriendType.FollowedSearch:
                View.FollowedSearch();
                break;
        }
    }

    private void Profile()
    {
        var popup = PopupManager.instance.Open<ProfilePopup>(CommunityManager.instance.CommunityPrefab.ProfilePopup);
        popup.Setting(Data.uuid, Data.userDataView);
    }

    private void MyRoom()
    {

    }

    private void More()
    {

    }

    private void Follow()
    {
        var profilePopup = PopupManager.instance.Get<ProfilePopup>();
        var friendPopup = PopupManager.instance.Get<FriendPopup>();
        switch (Data.friendType)
        {
            case FriendType.UnFollowedSearch:
                DatabaseConnector.instance.SendMemberFriendRequest(() => FollowUpdate(), null, true, Data.uuid);
                break;
            case FriendType.FollowedSearch:
                DatabaseConnector.instance.SendMemberFriendRequest(() => FollowUpdate(), null, false, Data.uuid);
                break;
        }
    }

    private void FollowUpdate()
    {
        var profilePopup = PopupManager.instance.Get<ProfilePopup>();
        var friendPopup = PopupManager.instance.Get<FriendPopup>();
        profilePopup?.Setting(Data.uuid, Data.userDataView);
        friendPopup?.Setting();
    }
}
