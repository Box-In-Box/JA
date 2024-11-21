using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Remained : MonoBehaviour
{
    [SerializeField] private Button openCloseButton;
    [field: SerializeField] public PopupEffect popup { get; set; }
    public Action<bool> popupAction { get; set; }

    private void Awake()
    {
        openCloseButton.onClick.AddListener(() => OpenOrClose());
        popup.Awake();
        popup.gameObject.SetActive(false);
    }

    public void OpenOrClose()
    {
        bool isOpen = !popup.gameObject.activeSelf;
        if (isOpen) popup.OpenPopup();
        else popup.ClosePopup();

        popupAction?.Invoke(isOpen);
    }
}