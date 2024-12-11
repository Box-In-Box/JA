using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Preset : MonoBehaviour
{
    [Serializable]
    public struct PresetBackgroundSprite
    {
        public Sprite nullSprite;
        public Sprite defaultSprite;
        public Sprite selectedSprite;
    }

    [Serializable]
    public struct NumberTextColor
    {
        public Color defaultColor;
        public Color selectedColor;
    }

    [field : SerializeField] public Image backgroundImage { get; private set; }
    [field : SerializeField] public Image saveImage { get; private set; }
    [field : SerializeField] public TMP_Text numberText { get; private set; }
    public PresetBackgroundSprite presetBackgroundSprite;
    public NumberTextColor numberTextColor;


    public void SetPresetNumber(int index) // 프리셋 번호 저장
    {
        numberText.text = index.ToString();
    }

    [Button]
    public void SaveImage() // 프리셋 저장
    {
        backgroundImage.sprite = presetBackgroundSprite.selectedSprite;
        numberText.color = numberTextColor.selectedColor;
        saveImage.gameObject.SetActive(true);
        numberText.gameObject.SetActive(false);
    }

    [Button]
    public void DefaultImage() // 저장 되어 있는 프리셋
    {
        backgroundImage.sprite = presetBackgroundSprite.defaultSprite;
        numberText.color = numberTextColor.defaultColor;
        saveImage.gameObject.SetActive(false);
        numberText.gameObject.SetActive(true);
    }

    [Button]
    public void NullImage() // 비어 있는 프리셋
    {
        backgroundImage.sprite = presetBackgroundSprite.nullSprite;
        numberText.color = numberTextColor.defaultColor;
        saveImage.gameObject.SetActive(false);
        numberText.gameObject.SetActive(true);
    }

    [Button]
    public void SelectedImage() // 선택 된 프리셋
    {
        backgroundImage.sprite = presetBackgroundSprite.selectedSprite;
        numberText.color = numberTextColor.selectedColor;
        saveImage.gameObject.SetActive(false);
        numberText.gameObject.SetActive(true);
    }
}
