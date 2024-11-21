using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudTarget_Sign : HudTarget
{
    public override void Awake()
    {
        Setting();
    }

    public override void Start()
    {
        
    }

    public override void Setting()
    {
        AutoCreateTarget();
        HudUI = GetHudUI();
    }

    public void AutoCreateTarget()
    {
        Target = new GameObject("HudTarget").transform;
        Target.SetParent(this.transform);

        MeshRenderer meshRenderer = GetComponentInParent<MeshRenderer>();
        Target.localPosition = new Vector3(0f, meshRenderer?.bounds.size.y ?? 0f, 0f);
    }
}