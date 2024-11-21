using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class More : MonoBehaviour
{
    [Serializable] private struct MoreSprite
    {
        public Sprite onSprite;
        public Sprite offSprite;
    }

    private Popup_Remained popup_Remained;

    [SerializeField] private Image image;
    [SerializeField] private MoreSprite moreSprite;

    private void Awake()
    {
        popup_Remained = GetComponent<Popup_Remained>();
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