using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldPopup : Popup
{
    [SerializeField] private TMP_Text msgText;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private TMP_Text okText;
    [SerializeField] private TMP_Text cancelText;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;

    public void Setting(string msgText, Action<string> okAction, Action cancelAction, string okText, string cancelText)
    {
        this.msgText.text = msgText;
        var rect = this.msgText.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width, msgText == "" ? 70 : 100);

        inputText.text = "";

        if (okText != "") this.okText.text = okText;
        if (cancelText != "") this.cancelText.text = cancelText;

        okButton.onClick.AddListener(() => okAction?.Invoke(inputText.text));
        cancelButton.onClick.AddListener(() => cancelAction?.Invoke());
    }
}