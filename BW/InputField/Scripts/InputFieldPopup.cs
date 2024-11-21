using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldPopup : Popup
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private TMP_Text okText;
    [SerializeField] private TMP_Text cancelText;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;

    public void Setting(string titleText, string msgText, Action okAction, Action cancelAction, string okText, string cancelText)
    {
        this.titleText.text = titleText;
        this.inputText.text = msgText;

        if (okText != "") this.okText.text = okText;
        if (cancelText != "") this.cancelText.text = cancelText;

        okButton.onClick.AddListener(() => okAction?.Invoke());
        cancelButton.onClick.AddListener(() => cancelAction?.Invoke());
    }
}