using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum UserMsgType
{
    DM,
    Guestbook,
}

[Serializable]
public class UserData
{
    public Sprite profile_image;
    public int uuid;
    public string nickname;
    public string msg;

    public UserData(Sprite profile_image, int uuid, string nickname, string msg)
    {
        this.profile_image = profile_image;
        this.uuid = uuid;
        this.nickname = nickname;
        this.msg = msg;
    }
}

[Serializable]
public class UserMsgData
{
    public UserMsgType userMsgType;
    public UserData userData;
    public DateTime dateTime;

    public UserMsgData(UserMsgType userMsgType, UserData userData, DateTime dateTime)
    {
        this.userMsgType = userMsgType;
        this.userData = userData;
        this.dateTime = dateTime;
    }
}

public class UserMsg : DynamicScrollViewItem<UserMsgData>
{
    [field: Title("[ View ]")]
    [field: SerializeField] public UserMsgView View { get; set; }

    [field: SerializeField, ReadOnly] public UserMsgData Data { get; set; }
    public override RectTransform Rect { get => View.Rect; }
    public override ContentSizeFitter SizeFitter { get => View.SizeFitter; }

    private void Start()
    {
        View.Button.onClick.AddListener(() => OnClick());
    }

    public override UserMsgData GetData()
    {
        return Data;
    }

    public override void OnUpdate(UserMsgData data)
    {
        Data = data;

        View.ProfileImage.sprite = data.userData.profile_image ?? View.DefaultSprite;
        View.NickNameText.text = data.userData.nickname;
        View.MsgText.text = data.userData.msg;
        View.DateText.text = BW.Tool.GetChatTime(data.dateTime);

        LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    }

    public void OnClick()
    {
        switch (Data.userMsgType)
        {
            case UserMsgType.DM:
                var dmPopup = PopupManager.instance.Open<DMPopup>(CommunityManager.instance.CommunityPrefab.DMPopup);
                dmPopup.Setting(Data.userData);
                break;
            case UserMsgType.Guestbook:
                var profilePopup = PopupManager.instance.Open<ProfilePopup>(CommunityManager.instance.CommunityPrefab.ProfilePopup);

                var data = new UserDataView();
                data.nickname = Data.userData.nickname;
                data.profile_image = Data.userData.profile_image;
                profilePopup.Setting(Data.userData.uuid, data);

                DatabaseConnector.instance.GetMemberData((value) =>
                {
                    var profilePopup = PopupManager.instance.Get<ProfilePopup>();
                    if (profilePopup) profilePopup.Setting(Data.userData.uuid, value);
                }, null, Data.userData.uuid);
                break;
        }
    }
}
