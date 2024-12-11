using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatReceiver : ChatBubble, IChatReceiver
{
    private int uuid;
    private string nickName;

    private void Start()
    {
        if (HudUi.HudTarget.TryGetComponent<PlayerCharacter>(out PlayerCharacter player))
        {
            uuid = player.Uuid;
            nickName = player.NickName;
        }
    }

    private void OnEnable()
    {
        if (PhotonNetworkManager.instance)
        {
            PhotonNetworkManager.instance.ChatAction += ChatReceive;
        }
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.instance)
        {
            PhotonNetworkManager.instance.ChatAction -= ChatReceive;
        }
    }

    public void ChatReceive(Chat_Message chat_Message)
    {
        if (uuid == chat_Message.uuid)
        {
            Chat(chat_Message.msg);
        }
    }
}