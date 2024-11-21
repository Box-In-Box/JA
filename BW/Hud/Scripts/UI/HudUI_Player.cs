using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudUI_Player : HudUI
{
    [field: Title("[ HudUI_Player ]")]
    [field: SerializeField] public RectTransform NickName { get; set; }
    [field: SerializeField] public RectTransform ChatBubble { get; set; }

    private void Start()
    {
        if (HudTarget.TryGetComponent<PlayerCharacter>(out PlayerCharacter player))
        {
            NickName.GetComponent<TMP_Text>().text = player.NickName;
            player.HudUI_Player = this;
        }
    }

    public void ShowHud(bool nickName, bool chatBubble)
    {
        NickName.gameObject.SetActive(nickName);
        ChatBubble.gameObject.SetActive(chatBubble);
    }
    public void ShowHud_NickName(bool nickName)
    {
        NickName.gameObject.SetActive(nickName);
    }
    public void ShowHud_Chat(bool chatBubble)
    {
        ChatBubble.gameObject.SetActive(chatBubble);
    }
}