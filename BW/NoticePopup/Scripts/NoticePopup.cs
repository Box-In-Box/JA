using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticePopup : Popup
{
    [field : SerializeField] public TMP_Text msgText { get; private set; }
    [field : SerializeField] public TMP_Text okText { get; private set; }
    [field : SerializeField] public TMP_Text cancelText { get; private set; }
    [field : SerializeField] public Button okButton { get; private set; }
    [field : SerializeField] public Button cancelButton { get; private set; }

    [field : SerializeField] private float okOriginalPosX;

    public override void Awake()
    {
        base.Awake();
        okOriginalPosX = okButton.GetComponent<RectTransform>().anchoredPosition.x;
    }

    public void Setting(string msgText, Action okAction, Action cancelAction, string okText, string cancelText)
    {
        this.msgText.text = msgText;

        if (okText != "") this.okText.text = okText;
        if (cancelText != "") this.cancelText.text = cancelText;

        okButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        
        okButton.onClick.AddListener(() => okAction?.Invoke());
        cancelButton.onClick.AddListener(() => cancelAction?.Invoke());
    }

    public void RemoveCancelButton()
    {
        cancelButton.gameObject.SetActive(false);
        RectTransform okRect = okButton.GetComponent<RectTransform>();
        okRect.anchoredPosition = new Vector2(0f, okRect.anchoredPosition.y);
    }

    public void ShowCancelButton()
    {
        cancelButton.gameObject.SetActive(true);
        RectTransform okRect = okButton.GetComponent<RectTransform>();
        okRect.anchoredPosition = new Vector2(okOriginalPosX, okRect.anchoredPosition.y);
    }
}