using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePopupView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public TMP_Text NickNameText { get; set; }
    [field: SerializeField] public Image ProfileImage { get; set; }
    [field: SerializeField] public TMP_Text LevelText { get; set; }
    [field: SerializeField] public TMP_Text FriendText { get; set; }
    [field: SerializeField] public TMP_Text RankText { get; set; }
    [field: SerializeField] public TMP_Text introductionText { get; set; }

    [field: Title("[ Default ]")]
    [field: SerializeField] public Sprite DefaultSprite { get; set; }

    [field: Title("[ Button ]")]
    [field: SerializeField] public Button FollowButton { get; set; }
    [field: SerializeField] public Button DMButton { get; set; }
    [field: SerializeField] public Button MyRoomButton { get; set; }
}