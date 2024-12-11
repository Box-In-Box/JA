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

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public ChatDynamicScrollView ChatDynamicScrollView { get; set; }

    public void Start()
    {
        ChatDynamicScrollView.Init(new List<ChatScrollViewData>());
    }

    public void Setting(UserData data)
    {
        if (GameManager.instance.isGuest) return;

        Data = data;
        View.NickNameText.text = data.nickname;

        // Get Data
        DatabaseConnector.instance.GetChattingRecord((chatData) => SettingChat(chatData), null, data.uuid);
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
}