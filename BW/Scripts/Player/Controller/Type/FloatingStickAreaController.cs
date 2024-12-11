using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using DG.Tweening;

public class FloatingStickAreaController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform joystickBound;
    [SerializeField] private RectTransform joystickRect;
    [SerializeField] private CanvasGroup joystickCanvasGroup;
    [SerializeField] private OnScreenStick stick;
    [SerializeField] private bool isFloating = true;

    private Vector2 joystickBoundOriginalSize; // 더블 클릭 방지

    private void Awake()
    {
        if (isFloating) {
            joystickCanvasGroup.alpha = 0f;
            joystickBoundOriginalSize = new Vector2(joystickBound.rect.width, joystickBound.rect.height);
        }
        else {
            joystickRect.pivot = Vector2.zero;
            joystickRect.localPosition = Vector3.zero;
            joystickBound.sizeDelta = Vector2.zero;
            this.enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystickCanvasGroup.DOKill();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBound, eventData.pressPosition, eventData.pressEventCamera, out Vector2 localPosition);

        joystickRect.anchoredPosition = localPosition;

        ExecuteEvents.pointerDownHandler(stick, eventData);

        joystickCanvasGroup.alpha = 1f;

        joystickBound.sizeDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ExecuteEvents.dragHandler(stick, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ExecuteEvents.pointerUpHandler(stick, eventData);

        joystickCanvasGroup.DOFade(0f, 1f);

        joystickBound.sizeDelta = joystickBoundOriginalSize;
    }
}