using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DMPopup : Popup
{
    [field: Title("[ View ]")]
    [field: SerializeField] public DMPopupView View { get; private set; }

    [field: Title("[ Data ]")]
    [field: SerializeField, ReadOnly] public UserData Data { get; set; }
    [field: SerializeField, ReadOnly] public UserDataView UserData { get; set; }

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public ChatDynamicScrollView ChatDynamicScrollView { get; set; }

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
        ChatDynamicScrollView.Init(new List<ChatScrollViewData>());

        View.SendButton.onClick.AddListener(() => Send());
        View.ChatInputField.onEndEdit.AddListener((value) => EnterChatSend());
    }

    public void Setting(UserData data)
    {
        if (GameManager.instance.isGuest) return;

        Data = data;
        View.NickNameText.text = data.nickname;
        UserData = null;

        // Get Data
        DatabaseConnector.instance.GetChattingRecord((chatData) => SettingChat(chatData), null, data.uuid);
        DatabaseConnector.instance.GetMemberData((value) => UserData = value, null, Data.uuid);
    }

    private void SettingChat(ChatRecordData chatRecordData)
    {
        DateTime? previousDate = null;
        DateTime currentDate;

        for (int i = chatRecordData.chat_history.Count() -1; i >= 0; i--)
        {
            var chat = chatRecordData.chat_history[i];

            currentDate = BW.Tool.GetDateTime(chat.chat_time);

            if (previousDate == null || previousDate.Value.Date != currentDate.Date)
            {
                DateChat(currentDate);
                previousDate = currentDate;
            }

            if (chat.chat_uuid == DatabaseConnector.instance.memberUUID)
            {
                SendChat(chat);
            }
            else
            {
                ReceiveChat(chat);
            }
        }
    }

    [Button]
    public void ReceiveChat(ChatRecordItems chat)
    {
        var data = new ChatScrollViewData(ChatType.ChatOther, Data.nickname, chat.chat_string, BW.Tool.GetDateTime(chat.chat_time), null);
        ChatDynamicScrollView.AddData(data, true, true);
    }

    [Button]
    public void SendChat(ChatRecordItems chat)
    {
        var data = new ChatScrollViewData(ChatType.ChatMine, "", chat.chat_string, BW.Tool.GetDateTime(chat.chat_time), null);
        ChatDynamicScrollView.AddData(data, true, true);
    }

    [Button]
    public void DateChat(DateTime dateTime)
    {
        var data = new ChatScrollViewData(ChatType.ChatDate, "", "", dateTime, null);
        ChatDynamicScrollView.AddData(data, true, true);
    }

    public void Send()
    {
        int myUuid = DatabaseConnector.instance.memberUUID;
        string msg = View.ChatInputField.text.Trim();

        if (myUuid < 0 || Data.uuid < 0 || msg == "") return;

        var chatItem = new ChatRecordItems();
        chatItem.chat_uuid = Data.uuid;
        chatItem.chat_time = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
        chatItem.chat_string = msg;


        DatabaseConnector.instance.SendChat(() =>
        {
            PhotonChatManager.instance.SendMessage(PhotonCommunityCode.DM, myUuid, Data.uuid, msg);
            SendChat(chatItem);
            PopupManager.instance.Get<DMListPopup>()?.Setting();
        }, null, Data.uuid, msg);
        
        View.ChatInputField.text = "";
        View.ChatInputField.ActivateInputField();
    }

    public void EnterChatSend()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) Send();
    }

    private void Receive(Community_Message community_Message)
    {
        if (community_Message.receiver == DatabaseConnector.instance.memberUUID && Data.uuid == community_Message.sender)
        {
            var chatItem = new ChatRecordItems();
            chatItem.chat_uuid = Data.uuid;
            chatItem.chat_time = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
            chatItem.chat_string = community_Message.msg;

            ReceiveChat(chatItem);
            PopupManager.instance.Get<DMListPopup>()?.Setting();
        }
    }
}