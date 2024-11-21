using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WorldSign;

public class WorldSignView : MonoBehaviour
{
    [Serializable]
    public struct SignBackgroundSprite
    {
        public Sprite normal;
        public Sprite title;
    }

    [field: SerializeField] public RectTransform rect { get; set; }
    [field: SerializeField] public Button button { get; set; }

    [field: Title("[ BackgroundImage ]")]
    [field: SerializeField] public RectTransform backgroundDeco { get; private set; }
    [field: SerializeField] public RectTransform backgroundImageRect { get; private set; }
    [field: SerializeField] public Image backgroundImage { get; private set; }
    [field: SerializeField] public SignBackgroundSprite signBackgroundSprite { get; private set; }

    [field: Title("[ Title ]")]
    [field: SerializeField] public TMP_Text title { get; private set; }

    [field: Title("[ Sign ]")]
    [field: SerializeField] public Image image { get; private set; }
    [field: SerializeField] public TMP_Text signText { get; private set; }

    public void SettingSign(Sprite sprite, string name)
    {
        image.sprite = sprite;
        signText.text = name;
    }

    [Button]
    public void NormalSign()
    {
        backgroundImage.sprite = signBackgroundSprite.normal;
        backgroundImage.SetNativeSize();
        backgroundDeco.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        rect.sizeDelta = backgroundImageRect.sizeDelta;
    }

    [Button]
    public void TitleSign()
    {
        backgroundImage.sprite = signBackgroundSprite.title;
        backgroundImage.SetNativeSize();
        backgroundDeco.gameObject.SetActive(true);
        title.gameObject.SetActive(true);
        rect.sizeDelta = backgroundImageRect.sizeDelta;
    }
}
