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
    [field: SerializeField] public RectTransform Rect { get; set; }
    [field: SerializeField] public Button Button { get; set; }

    [field: Title("[ BackgroundImage ]")]
    [field: SerializeField] public RectTransform DackgroundDeco { get; private set; }
    [field: SerializeField] public RectTransform BackgroundImageRect { get; private set; }
    [field: SerializeField] public Image BackgroundImage { get; private set; }
    [field: SerializeField] public Sprite SignBackgroundSprite_Normal { get; private set; }
    [field: SerializeField] public Sprite SignBackgroundSprite_Title { get; private set; }

    [field: Title("[ Title ]")]
    [field: SerializeField] public TMP_Text Title { get; private set; }

    [field: Title("[ Sign ]")]
    [field: SerializeField] public Image Image { get; private set; }
    [field: SerializeField] public TMP_Text SignText { get; private set; }

    public void SettingSign(Sprite sprite, string name)
    {
        Image.sprite = sprite;
        SignText.text = name;
    }

    [Button]
    public void NormalSign()
    {
        BackgroundImage.sprite = SignBackgroundSprite_Normal;
        BackgroundImage.SetNativeSize();
        DackgroundDeco.gameObject.SetActive(false);
        Title.gameObject.SetActive(false);
        Rect.sizeDelta = BackgroundImageRect.sizeDelta;
    }

    [Button]
    public void TitleSign()
    {
        BackgroundImage.sprite = SignBackgroundSprite_Title;
        BackgroundImage.SetNativeSize();
        DackgroundDeco.gameObject.SetActive(true);
        Title.gameObject.SetActive(true);
        Rect.sizeDelta = BackgroundImageRect.sizeDelta;
    }
}