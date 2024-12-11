using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public Image ProfileImage { get; set; }
    [field: SerializeField] public TMP_Text NickNameText { get; set; }
    [field: SerializeField] public TMP_Text MsgText { get; set; }
    [field: SerializeField] public TMP_Text DateTimeText { get; set; }

    [field: Title("[ Default ]")]
    [field: SerializeField] public Sprite DefaultSprite { get; set; }

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public RectTransform Rect { get; set; }
    [field: SerializeField] public ContentSizeFitter SizeFitter { get; set; }
    [field: SerializeField] public ChatType ChatType { get; set; }

    [field: Title("[ Setting ]")]
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    [Button]
    public void VisibleProfileImage(bool value)
    {
        ProfileImage.gameObject.SetActive(value);
    }

    [Button]
    public void VisibleDateTime(bool value)
    {
        DateTimeText.gameObject.SetActive(value);
        verticalLayoutGroup.padding.bottom = value ? (int)DateTimeText.GetComponent<RectTransform>().rect.height : 0;
    }
}
