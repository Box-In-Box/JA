using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBubbleController : MonoBehaviour
{
    [Title("[ References ]")]
    [SerializeField] private Button button;
    [SerializeField] private Image chatImage;
    [SerializeField] private Image tailImage;

    [SerializeField] private bool isClickable = false;
    public bool IsClickable
    {
        get => isClickable;
        set
        {
            isClickable = value;
            chatImage.raycastTarget = value;
        }
    }

    private Action onClickAction;

    private void Awake()
    {
        IsClickable = IsClickable;
        button.onClick.AddListener(() => onClickAction?.Invoke());
    }

    #region Button
    public void AddListener(Action action)
    {
        onClickAction += action;
    }

    public void RemoveListener(Action action)
    {
        onClickAction -= action;
    }

    public void RemoveAllListener()
    {
        onClickAction = null;
    }
    #endregion

    #region Color
    public void ChatBubbleColor(Color? color = null)
    {
        chatImage.color = color ?? Color.white;
        tailImage.color = color ?? Color.white;
    }

    public void EnterColor() => tailImage.DOColor(button.image.color * button.colors.highlightedColor, 0.1f);
    public void PressedColor() => tailImage.DOColor(button.image.color * button.colors.pressedColor, 0.1f);
    public void NormalColor() => tailImage.DOColor(button.image.color * button.colors.normalColor, 0.1f);
    public void SelectedColor() => tailImage.DOColor(button.image.color * button.colors.selectedColor, 0.1f);
    #endregion
}