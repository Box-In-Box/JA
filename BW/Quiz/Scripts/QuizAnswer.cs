using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class QuizAnswer : MonoBehaviour
{
    [field : SerializeField] public Button button { get; private set; }
    [field : SerializeField] public TMP_Text numberText { get; private set; }

    [field : Title("[ Selected Text ]")]
    [field : SerializeField] public TMP_Text answerText { get; private set; }
    [field : SerializeField] public Color normalTextColor { get; private set; }
    [field : SerializeField] public Color selectedTextColor { get; private set; }

    [field : Title("[ Selected Image ]")]
    [field : SerializeField] public Image image { get; private set; }
    [field : SerializeField] public Sprite normalSprite { get; private set; }
    [field : SerializeField] public Sprite selectedSprite { get; private set; }

    [field : SerializeField] public bool isCorrectAnswer { get; private set; } = false;

    public void SetAnswer(string answerView, string answer)
    {
        answerText.text = answerView;
        isCorrectAnswer = answerView == answer ? true : false;
    }

    public void SetSelected(bool value)
    {
        image.sprite = value ? selectedSprite : normalSprite;
        answerText.color = value ? selectedTextColor : normalTextColor;
    }
}