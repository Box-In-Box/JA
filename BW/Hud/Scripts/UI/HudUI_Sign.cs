using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HudUI_Sign : HudUI
{
    [field: Title("[ HudUI_Sign ]")]
    [field: SerializeField] public WorldSignView WorldSignView { get; set; }

    private void Start()
    {
        HudTarget.TryGetComponent<WorldSign>(out WorldSign sign);
        WorldSignView.SettingSign(sign.SignSprite, sign.SignName);
        sign.View = WorldSignView;
        sign.View.Button.onClick.AddListener(sign.SignPopup);
    }
}