using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class View_HudPanel : View
{
    [field : Title("[ View_HudPanel ]")]
    [field : SerializeField] public HudManager HudManager { get; private set; }

    public override void Awake()
    {
        base.Awake();
        if (HudManager == null)
        {
            HudManager = this.AddComponent<HudManager>();
        }
    }
}