using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dimmed : MonoBehaviour, IPointerClickHandler
{
    public bool isClosedWithDimmed = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClosedWithDimmed) return;

        // Dimmed 앞쪽 UI제외 (현재 컨텐츠 제외)
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results[0].gameObject != this.gameObject) return;

        Popup popup = this.GetComponentInChildren<Popup>();
        popup.isCanceled = true;
        PopupManager.instance.Close(popup);
    }
}