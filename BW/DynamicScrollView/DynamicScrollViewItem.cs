using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DynamicScrollViewItem<ScrollViewData> : MonoBehaviour
{
    public abstract RectTransform Rect { get; }
    public abstract ContentSizeFitter SizeFitter { get; }

    public abstract void OnUpdate(ScrollViewData data);
    public float GetSize(bool isVertical) 
    { 
        return isVertical ? Rect.rect.height : Rect.rect.width; 
    }
}