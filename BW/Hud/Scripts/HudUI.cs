using Sirenix.OdinInspector;
using UnityEngine;

public abstract class HudUI : MonoBehaviour
{
    [field: Title("[ Hud ]")]
    [field: SerializeField] public RectTransform Rect_Flexible { get; set; }
    [field: SerializeField] public RectTransform Rect_Fixed { get; set; }
    [field: SerializeField, ReadOnly] public HudTarget HudTarget { get; set; }
    [field: SerializeField, ReadOnly] public bool IsActive { get; set; }
    [field: SerializeField, ReadOnly] public bool IsVisible { get; set; }
}