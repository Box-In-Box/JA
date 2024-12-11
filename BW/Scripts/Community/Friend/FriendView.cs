using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public Image ProfileImage { get; set; }
    [field: SerializeField] public TMP_Text LevelText { get; set; }
    [field: SerializeField] public TMP_Text NickNameText { get; set; }
    [field: SerializeField] public TMP_Text MsgText { get; set; }
    [field: SerializeField] public TMP_Text FollowText { get; set; }

    [field: Title("[ Default ]")]
    [field: SerializeField] public Sprite DefaultSprite { get; set; }

    [field: Title("[ Buttons ]")]
    [field: SerializeField] public Button ProfileButton { get; set; }
    [field: SerializeField] public Button MyRoomButton { get; set; }
    [field: SerializeField] public Button MoreButton { get; set; }
    [field: SerializeField] public Button FollowButton { get; set; }

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public RectTransform Rect { get; set; }
    [field: SerializeField] public ContentSizeFitter SizeFitter { get; set; }

    public void Friend()
    {
        MyRoomButton.gameObject.SetActive(true);
        MoreButton.gameObject.SetActive(true);
        FollowButton.gameObject.SetActive(false);
    }

    public void UnFollowedSearch()
    {
        MyRoomButton.gameObject.SetActive(false);
        MoreButton.gameObject.SetActive(false);
        FollowButton.gameObject.SetActive(true);
        FollowText.text = "ÆÈ·Î¿ì";
    }

    public void FollowedSearch()
    {
        UnFollowedSearch();
        FollowText.text = "¾ðÆÈ·Î¿ì";
    }
}