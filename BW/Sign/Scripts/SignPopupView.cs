using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SignPopupView : MonoBehaviour
{
    [field: Title("[ Text ]")]
    [field: SerializeField] public TMP_Text TitleText { get; set; }
    [field: SerializeField] public TMP_Text ChatText { get; set; }

    [field: Title("[ Content Buttons ]")]
    [field: SerializeField] public RectTransform ContentButtonRect { get; set; }
    [field: SerializeField] public Button EnterButton { get; set; }
    [field: SerializeField] public Button QuizButton { get; set; }

    [field: Title("[ ETC Buttons ]")]
    [field: SerializeField] public Button YoutubeButton { get; set; }
    [field: SerializeField] public Button HomepageButton { get; set; }
    [field: SerializeField] public Button MapButton { get; set; }

    public void SetEnterButton(bool value, Action action = null)
    {
        EnterButton.gameObject.SetActive(value);
        EnterButton.onClick.AddListener(() => action?.Invoke());
        EnterButton.onClick.AddListener(() => PopupManager.instance.Close<SignPopup>());

        bool iscontentRect = EnterButton.gameObject.activeSelf || QuizButton.gameObject.activeSelf;
        ContentButtonRect.gameObject.SetActive(iscontentRect);
    }

    public void SetQuizButton(bool value, Action action = null)
    {
        QuizButton.gameObject.SetActive(value);
        QuizButton.onClick.AddListener(() => action?.Invoke());
        EnterButton.onClick.AddListener(() => PopupManager.instance.Close<SignPopup>());

        bool iscontentRect = EnterButton.gameObject.activeSelf || QuizButton.gameObject.activeSelf;
        ContentButtonRect.gameObject.SetActive(iscontentRect);
    }
}
