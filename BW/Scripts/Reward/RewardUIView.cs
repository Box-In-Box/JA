using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIView : MonoBehaviour
{
    [Serializable]
    public class BackgroundSprite
    {
        public Sprite normal;
        public Sprite selected;
    }

    [Serializable]
    public class TitleTextColor
    {
        public Color normal;
        public Color selected;
    }

    [Serializable]
    public class TitleTextFont
    {
        public TMP_FontAsset normal;
        public TMP_FontAsset selected;
    }

    [Serializable]
    public class ValueTextColor
    {
        public Color normal;
        public Color selected;
    }
    [Serializable]
    public class ValueTextFont
    {
        public TMP_FontAsset normal;
        public TMP_FontAsset selected;
    }

    [field: Title("[ Background ]")]
    [field: SerializeField] public Image BackgroundImage { get; set; }
    [SerializeField] private BackgroundSprite backgroundSprite;

    [field: Title("[ Title ]")]
    [field: SerializeField] public TMP_Text TitleText { get; set; }
    [SerializeField] private TitleTextColor titleTextColor;
    [SerializeField] private TitleTextFont titleTextFont;

    [field: Title("[ Value ]")]
    [field : SerializeField] public TMP_Text ValueText { get; set; }
    [SerializeField] private ValueTextColor valueTextColor;
    [SerializeField] private ValueTextFont valueTextFont;

    [field: Title("[ Complated ]")]
    [field: SerializeField] public GameObject CompletedPanel { get; set; }
    [SerializeField] private TMP_Text CompletedTitleText;

    [Button]
    public void Normal()
    {
        BackgroundImage.sprite = backgroundSprite.normal;

        TitleText.color = titleTextColor.normal;
        ValueText.color = valueTextColor.normal;

        TitleText.font = titleTextFont.normal;
        ValueText.font = valueTextFont.normal;

        CompletedPanel.SetActive(false);
    }

    [Button]
    public void Selected()
    {
        BackgroundImage.sprite = backgroundSprite.selected;

        TitleText.color = titleTextColor.selected;
        ValueText.color = valueTextColor.selected;

        TitleText.font = titleTextFont.selected;
        ValueText.font = valueTextFont.selected;

        CompletedPanel.SetActive(false);
    }

    [Button]
    public void Completed()
    {
        Normal();

        CompletedTitleText.text = TitleText.text;
        CompletedPanel.SetActive(true);
    }
}