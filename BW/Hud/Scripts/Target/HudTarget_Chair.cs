using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudTarget_Chair : HudTarget
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
        if (Target == null)
        {
            Target = new GameObject("HudTarget").transform;
            Target.SetParent(this.transform);
            Target.localPosition = Vector3.zero;
        }
    }
}
