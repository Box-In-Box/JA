using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ChatBubble : MonoBehaviour
{
    [field: Title("[ ChatBubble ]")]
    [field: SerializeField] public HudUI HudUi { get; set; }
    [field: SerializeField] public TMP_Text ChatText { get; set; }
    [field: SerializeField, Tooltip("±âº» Ãª ¸»Ç³¼± Áö¼Ó ½Ã°£")] public float ChatBubbleTime { get; set; } = 3f;
    public Coroutine chatCoroutine { get; set; }
    public WaitForSeconds chatBubbleWait { get; set; }

    public void Chat(string msg)
    {
        ChatText.text = msg;

        if (chatCoroutine != null)
        {
            HudUi.Rect_Flexible.DOKill();
            HudUi.Rect_Flexible.localScale = new Vector3(.8f, .8f, .8f);
            HudUi.Rect_Flexible.DOScale(1f, .5f).SetEase(Ease.OutBack);
            StopCoroutine(chatCoroutine);
        }
        chatCoroutine = StartCoroutine(ChatBubbleCoroutine());
    }
    private IEnumerator ChatBubbleCoroutine()
    {
        HudUi.IsVisible = true;
        yield return new WaitForSeconds(ChatBubbleTime);
        HudUi.IsVisible = false;
        chatCoroutine = null;
    }
}
