using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudUI_NPC : HudUI
{
    [field: Title("[ HudUI_NPC ]")]
    [field: SerializeField] public RectTransform NPCName { get; set; }
    [field: SerializeField] public RectTransform ChatBubble { get; set; }

    private void Start()
    {
        NPCName.GetComponent<TMP_Text>().text = HudTarget.gameObject.name;
    }

    public void ShowHud(bool nickName, bool chatBubble)
    {
        NPCName.gameObject.SetActive(nickName);
        ChatBubble.gameObject.SetActive(chatBubble);
    }
    public void ShowHud_NPCName(bool nickName)
    {
        NPCName.gameObject.SetActive(nickName);
    }
    public void ShowHud_Chat(bool chatBubble)
    {
        ChatBubble.gameObject.SetActive(chatBubble);
    }
}