using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DMListPopup : Popup
{
    [field: Title("[ View ]")]
    [field: SerializeField] public DMListPopupView View { get; set; }

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public UserMsgScrollView UserMsgScrollView { get; set; }

    public override void Awake()
    {
        base.Awake();

        Setting();
    }

    private void OnEnable()
    {
        if (CommunityManager.instance)
        {
            CommunityManager.instance.DmAction += Receive;
        }
    }

    private void OnDisable()
    {
        if (CommunityManager.instance)
        {
            CommunityManager.instance.DmAction -= Receive;
        }
    }

    public void Start()
    {
        UserMsgScrollView.Init(new List<UserMsgData>());
    }

    public void Setting()
    {
        if (GameManager.instance.isGuest) return;

        DatabaseConnector.instance.GetChattingLast((chatData) => SettingLastChat(chatData), null);
    }

    private void SettingLastChat(LastChatData data)
    {
        UserMsgScrollView.ResetData();

        foreach (var chatData in data.last_chats)
        {
            LastChat(chatData);
        }
    }

    [Button]
    public void LastChat(LastChatItems chat)
    {
        var userData = new UserData(chat.profile_image, chat.friend_uuid, chat.friend_nickname, chat.friend_last_chat);
        var data = new UserMsgData(UserMsgType.DM, userData, BW.Tool.GetDateTime(chat.chat_time));
        UserMsgScrollView.AddData(data, true, true);
    }

    private void Receive(Community_Message community_Message)
    {
        if (community_Message.receiver == DatabaseConnector.instance.memberUUID)
        {
            Setting();
        }
    }
}