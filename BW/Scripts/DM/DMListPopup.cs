using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DMListPopup : Popup
{
    [field: Title("[ Friend Prefab ]")]
    [field: SerializeField] public GameObject UserMsgObjectPrefab { get; set; }

    [field: Title("[ View ]")]
    [field: SerializeField] public DMListPopupView View { get; set; }

    [field: Title("[ DM List ]")]
    [SerializeField] private List<UserMsg> userMsgList;

    public override void Awake()
    {
        base.Awake();

        Setting();
    }

    private void Setting()
    {
        if (GameManager.instance.isGuest) return;

        DatabaseConnector.instance.GetChattingLast((chatData) => SettingLastChatUser(chatData), null);
    }

    private void SettingLastChatUser(LastChatData lastChatData)
    {
        foreach (var lastChat in lastChatData.last_chats)
        {
            DatabaseConnector.instance.GetMemberData((userData) 
                => SettingLastChat(lastChat.friend_uuid, lastChat, userData), null, lastChat.friend_uuid);
        }
    }

    private void SettingLastChat(int uuid, LastChatItems chatData, UserDataView data)
    {
        var userMsg = Instantiate(UserMsgObjectPrefab, View.DMListContentPanel).GetComponent<UserMsg>();
        userMsg.Uuid = uuid;
        userMsg.Msg = chatData.friend_last_chat;
        userMsg.Time = chatData.chat_time;
        userMsg.Data = data;
        userMsg.Setting();
        userMsg.DmView();

        userMsgList.Add(userMsg);
        SortByDateTime();
    }

    private void SortByDateTime()
    {
        Transform[] children = new Transform[View.DMListContentPanel.transform.childCount];
        for (int i = 0; i < View.DMListContentPanel.transform.childCount; i++)
        {
            children[i] = View.DMListContentPanel.transform.GetChild(i);
        }

        userMsgList.Sort((a, b) => a.DateTime.CompareTo(b.DateTime));

        for (int i = 0; i < userMsgList.Count; i++)
        {
            Transform userMsgTransform = children.FirstOrDefault(child => child == userMsgList[i].transform);
            if (userMsgTransform != null)
            {
                userMsgTransform.SetSiblingIndex(children.Length - 1 - i);
            }
        }
    }
}