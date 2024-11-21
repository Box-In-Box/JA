using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class View_HudPanel : View
{
    [field : Title("[ View_HudPanel ]")]
    [field : SerializeField] public HudManager HudManager { get; private set; }
    [field: SerializeField] public RectTransform PlayerHudPanel { get; private set; }
    [field: SerializeField] public RectTransform NPCHudPanel { get; private set; }
    [field: SerializeField] public RectTransform SignHudPanel { get; private set; }
    [field: SerializeField] public RectTransform ChairHudPanel { get; private set; }

    private void Awake()
    {
        if (HudManager == null)
        {
            HudManager = this.AddComponent<HudManager>();
        }
        if (PlayerHudPanel == null)
        {
            PlayerHudPanel = Canvas_Scene.instance.CreatePanel("PlayerHudPanel");
            PlayerHudPanel.SetParent(this.transform);
        }
        if (NPCHudPanel == null)
        {
            NPCHudPanel = Canvas_Scene.instance.CreatePanel("NPCHudPanel");
            NPCHudPanel.SetParent(this.transform);
        }
        if (SignHudPanel == null)
        {
            SignHudPanel = Canvas_Scene.instance.CreatePanel("SignHudPanel");
            SignHudPanel.SetParent(this.transform);
        }
        if (ChairHudPanel == null)
        {
            ChairHudPanel = Canvas_Scene.instance.CreatePanel("ChairHudPanel");
            ChairHudPanel.SetParent(this.transform);
        }
    }
}