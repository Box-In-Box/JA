using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public abstract class ChatBubble : MonoBehaviour
{
    [field: Title("[ ChatBubble ]")]
    [field: SerializeField] public HudUI HudUi { get; set; }
    [SerializeField] private TMP_Text chatBubbleText;
    [SerializeField, Tooltip("기본 챗 말풍선 지속 시간")] private float chatBubbleTime = 3f;
    [SerializeField, Tooltip("글자 당 추가 지속 시간")] private float chatBubbleTimePerTextSize = .03f;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public void ChatText(string msg)
    {
        chatBubbleText.text = msg;
    }

    public void Chat(string msg)
    {
        ChatTask(msg).Forget();
    }

    public void ChatAlways(string msg)
    {
        chatBubbleText.text = msg;
        HudUi.IsVisible = true;
    }

    public void CancelToken()
    {
        tokenSource?.Cancel();
        tokenSource?.Dispose();
        tokenSource = new CancellationTokenSource();
    }

    private async UniTaskVoid ChatTask(string msg)
    {
        chatBubbleText.text = msg;
        if (HudUi.IsVisible)
        {
            CancelToken();
            HudUi.Rect_Flexible.DOKill();
            HudUi.Rect_Flexible.localScale = Vector3.zero;
            await HudUi.Rect_Flexible.DOScale(1f, .5f).SetEase(Ease.OutBack);
        }
        await ChatBubbleTask(tokenSource.Token);
    }

    private async UniTask ChatBubbleTask(CancellationToken ct)
    {
        HudUi.IsVisible = true;

        float waitTime = chatBubbleTime + (chatBubbleText.text.Length * chatBubbleTimePerTextSize) + 10f;
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: ct);

        HudUi.IsVisible = false;
    }
}
