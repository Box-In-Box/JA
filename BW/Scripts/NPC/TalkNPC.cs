using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

[RequireComponent(typeof(HudTarget_NPC))]
public class TalkNPC : MonoBehaviour, ITalkReceiver
{
    public HudTarget HudTarget { get; set; }
    public TalkReceiver TalkReceiver { get; set; }

    [field: Title("[ Talk Data ]")]
    [field: SerializeField, HideIf(nameof(IsAlwaysTalk))] public List<string> TalkData { get; set; }
    [field: SerializeField, ShowIf(nameof(IsAlwaysTalk))] public string AlwaysTalkData { get; set; }

    [field: Title("[ Talk Time ]")]
    [field: SerializeField] public bool IsAlwaysTalk { get; set; }
    [field: SerializeField, Range(0f, 10f), HideIf(nameof(IsAlwaysTalk))] public float DefaultTalkTime { get; set; } = 3f;
    [field: SerializeField, Range(0f, .1f), HideIf(nameof(IsAlwaysTalk))] public float AddedTalkTimeText { get; set; } = .03f;

    [field: Title("[ Collider Setting ]")]
    [field: SerializeField, Range(1f, 10f)] public float TalkColliderRadius { get; set; } = 3f;
    public TalkNPCCollider TalkCollider { get; set; }

    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    private void Awake()
    {
        HudTarget = GetComponent<HudTarget>();
    }

    private void Start()
    {
        TalkSetting();
        ColliderSetting();
    }

    private void OnDisable()
    {
        TalkReceive(null);
    }

    private void TalkSetting()
    {
        if (!IsAlwaysTalk && TalkData.Count == 0)
        {
            (HudTarget.HudUI as HudUI_NPC).ShowHud_Chat(false);
        }
        TalkReceiver = (HudTarget.HudUI as HudUI_NPC).GetComponent<TalkReceiver>();
    }

    private void ColliderSetting()
    {
        GameObject colliderObj = new GameObject("TalkCollider(Clone)");
        colliderObj.transform.SetParent(this.transform);
        colliderObj.transform.localPosition = Vector3.zero;
        TalkCollider = colliderObj.AddComponent<TalkNPCCollider>();
        TalkCollider.Setting(this);
    }

    [Button]
    public void TalkReceive(string msg) // msg = Null로 넘어옴 (패딩 값)
    {
        if (msg == "Talk")
        {
            if (IsAlwaysTalk)
            {
                TalkReceiver.TalkReceive(AlwaysTalkData);
            }
            else
            {
                Talk().Forget();
            }
        }
        else
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            TalkReceiver.CancelToken();
            HudTarget.HudUI.IsVisible = false;
        }
    }

    public async UniTaskVoid Talk()
    {
        foreach (var data in TalkData)
        {
            TalkReceiver.TalkReceive(data);
            await UniTask.Delay(TimeSpan.FromSeconds(DefaultTalkTime + data.Length * AddedTalkTimeText), cancellationToken: tokenSource.Token);
        }
    }
}