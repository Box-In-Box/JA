using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class ChatScrollViewData
{
    public ChatType chatType;
    public ChatPositionType chatPositionType;
    public string userName;
    public string message;
    public string dateTime;
    public Sprite profileImage;

    public ChatScrollViewData(ChatType chatType, ChatPositionType chatPositionType, string userName, string message, string dateTime, Sprite profileImage)
    {
        this.chatType = chatType;
        this.chatPositionType = chatPositionType;
        this.userName = userName;
        this.message = message;
        this.dateTime = dateTime;
        this.profileImage = profileImage;
    }
}

public class ChatDynamicScrollView : DynamicScrollView<ChatScrollViewData, ChatItem>
{
    [field: SerializeField] public override GameObject Prefab { get; set; }
    [field: SerializeField] public override float MinSize { get; set; }
    [field: SerializeField] public override float Space { get; set; }
}
