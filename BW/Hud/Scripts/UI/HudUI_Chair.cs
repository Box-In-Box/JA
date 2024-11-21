using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class HudUI_Chair : HudUI
{
    [field: Title("[ HudUI_Chair ]")]
    [field: SerializeField] public Button SitButton { get; set; }

    private void Start()
    {
        HudTarget.TryGetComponent<Chair>(out Chair chair);
        SitButton.onClick.AddListener(chair.Sit);
    }
}