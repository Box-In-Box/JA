using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(HudTarget_NPC))]
public class TalkNPC : MonoBehaviour
{
    public HudTarget_NPC HudTarget_NPC { get; set; }
    public TalkReceiver TalkReceiver { get; set; }

    [field: Title("[ Talk Data ]")]
    [field: SerializeField] public string TalkData { get; set; }

    [field: Title("[ Talk Time ]")]
    [field: SerializeField] public bool IsAlwaysTalk { get; set; }
    [field: SerializeField, HideIf(nameof(IsAlwaysTalk))] public float TalkTime { get; set; } = 5f;

    [field: Title("[ Collider Setting ]")]
    [field: SerializeField, Range(1f, 10f)] public float TalkColliderRadius { get; set; } = 3f;
    public TalkCollider TalkCollider { get; set; }

    private void Awake()
    {
        HudTarget_NPC = GetComponent<HudTarget_NPC>();
    }

    private void Start()
    {
        TalkSetting();
        ColliderSetting();
    }

    private void TalkSetting()
    {
        if (TalkData == "")
        {
            (HudTarget_NPC.HudUI as HudUI_NPC).ShowHud_Chat(false);
        }
        TalkReceiver = (HudTarget_NPC.HudUI as HudUI_NPC).GetComponent<TalkReceiver>();
    }

    private void ColliderSetting()
    {
        GameObject colliderObj = new GameObject("TalkCollider(Clone)");
        colliderObj.transform.SetParent(this.transform);
        colliderObj.transform.localPosition = Vector3.zero;
        TalkCollider = colliderObj.AddComponent<TalkCollider>();
        TalkCollider.Setting(this);
    }
}