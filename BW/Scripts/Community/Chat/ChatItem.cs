using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatItem : DynamicScrollViewItem<ChatScrollViewData>
{
    [field: SerializeField] public List<ChatItemView> ChatItems { get; set; }
    [field: SerializeField, ReadOnly] public ChatItemView ChatItemView { get; set; }

    public Image ProfileImage { get => ChatItemView.ProfileImage; }
    public TMP_Text NickNameText { get => ChatItemView.NickNameText; }
    public TMP_Text MsgText { get => ChatItemView.MsgText; }
    public TMP_Text DateTimeText { get => ChatItemView.DateTimeText; }
    public Sprite DefaultSprite { get => ChatItemView.DefaultSprite; }
    public override RectTransform Rect { get => ChatItemView.Rect; }
    public override ContentSizeFitter SizeFitter { get => ChatItemView.SizeFitter; }

    public override void OnUpdate(ChatScrollViewData data)
    {
        ChatType(data.chatType);
        ChatPositionType(data.chatPositionType);

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
            DateTimeText.text = data.dateTime;
        }
        if (ProfileImage)
        {
            ProfileImage.sprite = data.profileImage ?? DefaultSprite;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    }

    [Button]
    private void ChatType(ChatType chatType)
    {
        foreach (var item in ChatItems)
        {
            if (item.ChatType == chatType)
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

    private void ChatPositionType(ChatPositionType chatPositionType)
    {
        switch (chatPositionType)
        {
            case global::ChatPositionType.Solo:
                break;
            case global::ChatPositionType.Head:
                ChatItemView.VisibleDateTime(false);
                break;
            case global::ChatPositionType.Body:
                ChatItemView.VisibleProfileImage(false);
                ChatItemView.VisibleDateTime(false);
                break;
            case global::ChatPositionType.Tail:
                ChatItemView.VisibleProfileImage(false);
                break;
        }
    }
}
