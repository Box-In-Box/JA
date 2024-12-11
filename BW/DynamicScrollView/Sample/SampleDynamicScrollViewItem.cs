using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleDynamicScrollViewItem : DynamicScrollViewItem<SampleDynamicScrollViewData>
{
    public TMPro.TextMeshProUGUI textMeshProUGUI;

    public RectTransform rect;
    public override RectTransform Rect { get => rect; }

    public ContentSizeFitter sizeFitter;
    public override ContentSizeFitter SizeFitter { get => sizeFitter; }

    public override void OnUpdate(SampleDynamicScrollViewData data)
    {
        textMeshProUGUI.text = data.text;
    }
}
