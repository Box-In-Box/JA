using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;

public class QuizAnswer : MonoBehaviour
{
    [Serializable]
    public struct TextColor
    {
        public Color normal;
        public Color selected;
    }

    [Serializable]
    public struct StateSprite
    {
        public Sprite normal;
        public Sprite selected;
    }

    [field: SerializeField] public Button Button { get; private set; }
    [field: SerializeField] public TMP_Text NumberText { get; private set; }

    [field: Title("[ Selected Text ]")]
    [field: SerializeField] public TextColor textColor;
    [field: SerializeField] public TMP_Text AnswerText { get; private set; }

    [field: Title("[ Selected Image ]")]
    [field: SerializeField] public StateSprite stateSprite;
    [field: SerializeField] public Image image { get; private set; }
    

    [field : SerializeField, ReadOnly] public bool isCorrectAnswer { get; private set; } = false;

    [Button]
    public void SetAnswer(string answerView, string answer)
    {
        AnswerText.text = answerView;
        isCorrectAnswer = answerView == answer ? true : false;
    }

    [Button]
    public void SetSelected(bool value)
    {
        image.sprite = value ? stateSprite.selected : stateSprite.normal;
        image.SetNativeSize();
        AnswerText.color = value ? textColor.selected : textColor.normal;
    }
}