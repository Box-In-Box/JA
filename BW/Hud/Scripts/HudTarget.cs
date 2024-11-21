using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HudTarget : MonoBehaviour
{
    [field: Title("[ Target ]")]
    [field: SerializeField] public Transform Target { get; set; }
    [field: SerializeField] public Vector3 Offset { get; set; }

    [field: Title("[ UI ]")]
    [field: SerializeField, ReadOnly] public HudUI HudUI { get; set; }

    public abstract void Awake();
    public abstract void Start();
    public abstract void Setting();

    public Vector3 GetPosition()
    {
        return Target.position + Offset;
    }

    public HudUI GetHudUI()
    {
        return Canvas_Scene.instance.view.hudPanel.HudManager.AddHud(this);
    }
}