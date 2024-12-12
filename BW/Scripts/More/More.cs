using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MoreSprite
{
    public Sprite onSprite;
    public Sprite offSprite;
}

public class More : View
{
    private Popup_Remained popup_Remained;

    private Image image;
    [SerializeField] private MoreSprite moreSprite;

    [field: Title("[ Alarm ]")]
    [SerializeField] private Alarm guestbookAlarm;
    [SerializeField] private Alarm dmAlarm;

    public override void Awake()
    {
        base.Awake();
        popup_Remained = GetComponent<Popup_Remained>();
        image = GetComponent<Image>();
        foreach (Transform menu in popup_Remained.popup.transform)
        {
            if (menu.TryGetComponent<Button>(out Button button))
            {
                button.onClick.AddListener(() => popup_Remained.OpenOrClose());
            }
        }
    }

    private void OnEnable()
    {
        if (popup_Remained != null)
        {
            popup_Remained.popupAction += PopupAction;
        }

        if (CommunityManager.instance)
        {
            CommunityManager.instance.GuestboolButtonAlarm = guestbookAlarm;
            CommunityManager.instance.DMButtonAlarm = dmAlarm;
        }
    }

    private void OnDisable()
    {
        if (popup_Remained != null)
        {
            popup_Remained.popupAction -= PopupAction;
        }
    }

    private void PopupAction(bool value)
    {
        image.sprite = value ? moreSprite.offSprite : moreSprite.onSprite;
    }
}