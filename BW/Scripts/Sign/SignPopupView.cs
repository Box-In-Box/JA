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

    public void SetEnterButton(string sceneName)
    {
        
        EnterButton.gameObject.SetActive(sceneName == "" ? false : true);

        if (sceneName != "")
        {
            EnterButton.onClick.AddListener(() => SceneLoadManager.instance.LoadScene(sceneName));
        }
        EnterButton.onClick.AddListener(() => PopupManager.instance.Close<SignPopup>());
        bool iscontentRect = EnterButton.gameObject.activeSelf || QuizButton.gameObject.activeSelf;
        ContentButtonRect.gameObject.SetActive(iscontentRect);
    }

    public void SetQuizButton(bool isMission, Action action = null)
    {
        QuizButton.gameObject.SetActive(isMission);
        if (isMission)
        {
            QuizButton.onClick.AddListener(() => action?.Invoke());
        }
        EnterButton.onClick.AddListener(() => PopupManager.instance.Close<SignPopup>());
        bool iscontentRect = EnterButton.gameObject.activeSelf || QuizButton.gameObject.activeSelf;
        ContentButtonRect.gameObject.SetActive(iscontentRect);
    }

    public void SetYoutubeURL(string url)
    {
        YoutubeButton.onClick.RemoveAllListeners();
        YoutubeButton.interactable = url != "" ? true : false;

        if (url != "")
        {
            YoutubeButton.onClick.AddListener(() => Application.OpenURL(url));
        }
    }

    public void SetHomepageURL(string url)
    {
        HomepageButton.onClick.RemoveAllListeners();
        HomepageButton.interactable = url != "" ? true : false;

        if (url != "")
        {
            HomepageButton.onClick.AddListener(() => Application.OpenURL(url));
        }
    }

    public void SetMapURL(string url)
    {
        MapButton.onClick.RemoveAllListeners();
        MapButton.interactable = url != "" ? true : false;

        if (url != "")
        {
            MapButton.onClick.AddListener(() => Application.OpenURL(url));
        }
    }
}
