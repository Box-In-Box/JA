using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToggleGroup : MonoBehaviour
{
    [Serializable]
    public struct ToggleColor
    {
        public Color on_Text_Color;
        public Color off_Text_Color;
    }

    [Serializable]
    public struct ToggleBox
    {
        public Toggle toggle;
        public TMP_Text text;
        public Image image;
        public RectTransform content;
    }
    
    public ToggleColor toggleColor;
    public ToggleBox[] toggleBox;

    private void Awake()
    {
        for (int i = 0; i < toggleBox.Length; ++i) {
            int num = i;
            OnToggle(num, toggleBox[i].toggle.isOn);
            toggleBox[i].toggle.onValueChanged.AddListener((value) => OnToggle(num, value));
        }
    }

    private void OnToggle(int index, bool value)
    {
        // Text Color
        if (toggleBox[index].text != null) {
            toggleBox[index].text.color = value ? toggleColor.on_Text_Color : toggleColor.off_Text_Color;
        }
        // Image Sprite
        if (toggleBox[index].image != null) {
            toggleBox[index].image.color = new Color(toggleBox[index].image.color.r, toggleBox[index].image.color.g, toggleBox[index].image.color.b, value ? 1f : 0f);
        }
        // Content Panel
        toggleBox[index].content.gameObject.SetActive(value);
    }
}