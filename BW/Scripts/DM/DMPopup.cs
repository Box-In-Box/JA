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

public enum ChatSender
{
    Other,
    Mine,
}

public class DMPopup : Popup
{
    [field: Title("[ View ]")]
    [field: SerializeField] public DMPopupView View { get; private set; }

    [field: Title("[ Data ]")]
    [field: SerializeField, ReadOnly] public UserDataView Data { get; set; }

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public ChatDynamicScrollView ChatDynamicScrollView { get; set; }

    public void Start()
    {
        List<ChatScrollViewData> chatDatas = new List<ChatScrollViewData>();
        ChatDynamicScrollView.Init(chatDatas);
    }

    public void Setting(int uuid, UserDataView data)
    {
        if (GameManager.instance.isGuest) return;

        Data = data;
        View.NickNameText.text = data.nickname;

        // Get Data
        DatabaseConnector.instance.GetChattingRecord((chatData) => SettingChat(chatData), null, uuid);
    }

    private void SettingChat(ChatRecordData chatRecordData)
    {
        int memberUuid = DatabaseConnector.instance.memberUUID;
        ChatSender currentSender = ChatSender.Other;
        ChatSender previousSender = ChatSender.Other;

        DateTime? previousDate = null;
        int? previousMinute = null;
        DateTime currentDate;
        DateTime checkDate;

        for (int i = chatRecordData.chat_history.Count() -1; i >= 0; i--)
        {
            var chat = chatRecordData.chat_history[i];
            currentSender = memberUuid == chat.chat_uuid ? ChatSender.Mine : ChatSender.Other;

            currentDate = DateTime.ParseExact(chat.chat_time, "yyyy-MM-dd_HH:mm:ss", null);

            if (previousDate == null || previousDate.Value.Date != currentDate.Date)
            {
                DateChat(currentDate);
                previousDate = currentDate;
                previousMinute = null;
            }

            if (i == chatRecordData.chat_history.Count() - 1)
            {
                if (currentSender == ChatSender.Mine) previousSender = ChatSender.Other;
                else if (currentSender == ChatSender.Other) previousSender = ChatSender.Mine;
            }

            if (previousMinute == null || previousSender != currentSender ||  previousMinute.Value != currentDate.Minute)
            {
                if (i - 1 >= 0)
                {
                    var nextChat = chatRecordData.chat_history[i - 1];
                    checkDate = DateTime.ParseExact(nextChat.chat_time, "yyyy-MM-dd_HH:mm:ss", null);

                    if (currentDate == null || currentDate.Minute != checkDate.Minute)
                    {
                        Chat(chat, ChatPositionType.Solo, currentSender);
                    }
                    else
                    {
                        Chat(chat, ChatPositionType.Head, currentSender);
                    }
                }
                else
                {
                    Chat(chat, ChatPositionType.Solo, currentSender);
                }
                previousDate = currentDate;
                previousMinute = currentDate.Minute;
                previousSender = currentSender;
            }
            else
            {
                if (i - 1 >= 0)
                {
                    var nextChat = chatRecordData.chat_history[i - 1];
                    checkDate = DateTime.ParseExact(nextChat.chat_time, "yyyy-MM-dd_HH:mm:ss", null);

                    if (currentDate == null || currentDate.Minute != checkDate.Minute )
                    {
                        Chat(chat, ChatPositionType.Tail, currentSender);
                    }
                    else
                    {
                        Chat(chat, ChatPositionType.Body, currentSender);
                    }
                }
                else
                {
                    Chat(chat, ChatPositionType.Tail, currentSender);
                }
            } 
        }
    }

    public void Chat(ChatRecordItems chat, ChatPositionType chatPositionType, ChatSender chatSender)
    {
        switch (chatSender)
        {
            case ChatSender.Other:
                ReceiveChat(chat, chatPositionType);
                break;
            case ChatSender.Mine:
                SendChat(chat, chatPositionType);
                break;
        }
    }

    [Button]
    public void ReceiveChat(ChatRecordItems chat, ChatPositionType chatPositionType = ChatPositionType.Solo)
    {
        var data = new ChatScrollViewData(ChatType.ChatOther, chatPositionType, Data.nickname, chat.chat_string, ChatTime(chat.chat_time), null);
        ChatDynamicScrollView.AddData(data, true, true);
    }

    [Button]
    public void SendChat(ChatRecordItems chat, ChatPositionType chatPositionType = ChatPositionType.Solo)
    {
        var data = new ChatScrollViewData(ChatType.ChatMine, chatPositionType, "", chat.chat_string, ChatTime(chat.chat_time), null);
        ChatDynamicScrollView.AddData(data, true, true);
    }

    private string ChatTime(string chatTime)
    {
        DateTime dateTime = DateTime.ParseExact(chatTime, "yyyy-MM-dd_HH:mm:ss", null);
        return dateTime.ToString("tt h:mm");
    }

    [Button]
    public void DateChat(DateTime dateTime)
    {
        var data = new ChatScrollViewData(ChatType.ChatDate, ChatPositionType.Solo, "", "", dateTime.ToString("yyyy.MM.dd"), null);
        ChatDynamicScrollView.AddData(data, true, true);
    }
}