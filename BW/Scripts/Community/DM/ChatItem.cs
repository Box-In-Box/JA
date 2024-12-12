using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ChatType
{
    ChatOther,
    ChatMine,
    ChatDate,
}

public enum ChatPositionType
{
    Solo,
    Head,
    Body,
    Tail,
}

[Serializable]
public class ChatScrollViewData
{
    public ChatType chatType;
    public string userName;
    public string message;
    public DateTime dateTime;
    public Sprite profileImage;

    public ChatScrollViewData(ChatType chatType, string userName, string message, DateTime dateTime, Sprite profileImage)
    {
        this.chatType = chatType;
        this.userName = userName;
        this.message = message;
        this.dateTime = dateTime;
        this.profileImage = profileImage;
    }

    public ChatPositionType chatPositionType = ChatPositionType.Solo;
}

public class ChatItem : DynamicScrollViewItem<ChatScrollViewData>
{
    [field: SerializeField] public List<ChatItemView> ChatItems { get; set; }
    [field: SerializeField, ReadOnly] public ChatItemView ChatItemView { get; set; }
    [field: SerializeField, ReadOnly] public ChatDynamicScrollView ChatDynamicScrollView { get; set; }
    [field: SerializeField, ReadOnly] public ChatScrollViewData Data { get; set; }
    [field: SerializeField, ReadOnly] public ChatScrollViewData PreviousData { get; set; }

    public Image ProfileImage { get => ChatItemView.ProfileImage; }
    public TMP_Text NickNameText { get => ChatItemView.NickNameText; }
    public TMP_Text MsgText { get => ChatItemView.MsgText; }
    public TMP_Text DateTimeText { get => ChatItemView.DateTimeText; }
    public Sprite DefaultSprite { get => ChatItemView.DefaultSprite; }

    public override RectTransform Rect { get => ChatItemView.Rect; }
    public override ContentSizeFitter SizeFitter { get => ChatItemView.SizeFitter; }

    private void Awake()
    {
        ChatDynamicScrollView = GetComponentInParent<ChatDynamicScrollView>();
        ChatItemView.ProfileImage.GetComponent<Button>().onClick.AddListener(() => Profile());
    }

    public override ChatScrollViewData GetData()
    {
        return Data ?? null;
    }

    public override void OnUpdate(ChatScrollViewData data)
    {
        Data = data;

        SetChatType();
        SetChatPosition();

        if (NickNameText)
        {
            NickNameText.text = data.userName;
        }
        if (MsgText)
        {
            MsgText.text = data.message;
        }
        if (DateTimeText)
        {
            DateTimeText.text = data.chatType == ChatType.ChatDate ? BW.Tool.GetChatTime(data.dateTime) : data.dateTime.ToString("tt h:mm");
        }
        if (ProfileImage)
        {
            ProfileImage.sprite = data.profileImage ?? DefaultSprite;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    }

    [Button]
    private void SetChatType()
    {
        foreach (var item in ChatItems)
        {
            if (item.ChatType == Data.chatType)
            {
                ChatItemView = item;
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private void SetChatPosition()
    {
        /*
        PreviousData = PreviousItem?.GetData();

        if (PreviousData != null && PreviousData.chatType != ChatType.ChatDate && Data.chatType != ChatType.ChatDate && PreviousData.chatType == Data.chatType)
        {
            if (PreviousData.dateTime.Minute == Data.dateTime.Minute)
            {
                if (PreviousData.chatPositionType == ChatPositionType.Solo)
                {
                    PreviousData.chatPositionType = ChatPositionType.Head;
                    PreviousItem.ChangeChatPositionType();
                }
                else if (PreviousData.chatPositionType == ChatPositionType.Tail)
                {
                    PreviousData.chatPositionType = ChatPositionType.Body;
                    PreviousItem.ChangeChatPositionType();
                }
                Data.chatPositionType = ChatPositionType.Tail;
                this.ChangeChatPositionType();
            }
        }
        */
    }

    public void ChangeChatPositionType()
    {
        switch (Data.chatPositionType)
        {
            case ChatPositionType.Solo:
                break;
            case ChatPositionType.Head:
                ChatItemView.VisibleDateTime(false);
                break;
            case ChatPositionType.Body:
                ChatItemView.VisibleProfileImage(false);
                ChatItemView.VisibleDateTime(false);
                break;
            case ChatPositionType.Tail:
                ChatItemView.VisibleProfileImage(false);
                break;
        }
    }

    private void Profile()
    {
        if (ChatItemView.ChatType != ChatType.ChatOther) return;

        var dmPopup = PopupManager.instance.Get<DMPopup>();
        var profilePopup = PopupManager.instance.Open<ProfilePopup>(CommunityManager.instance.CommunityPrefab.ProfilePopup);
        if (dmPopup?.UserData != null)
        {
            profilePopup.Setting(dmPopup.Data.uuid, dmPopup.UserData);
        }
        else
        {
            var data = new UserDataView();
            data.nickname = dmPopup.Data.nickname;
            data.introduce = dmPopup.Data.msg;
            data.profile_image = dmPopup.Data.profile_image;
            profilePopup.Setting(dmPopup.Data.uuid, data);
            DatabaseConnector.instance.GetMemberData((value) =>
            {
                var profilePopup = PopupManager.instance.Get<ProfilePopup>();
                if (profilePopup) profilePopup.Setting(dmPopup.Data.uuid, value);
            }, null, dmPopup.Data.uuid);
        }
    }
}
