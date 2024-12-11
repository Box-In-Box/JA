using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEditPopupView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public Image ProfileImage { get; set; }
    [field: SerializeField] public TMP_InputField NickNameInputField { get; set; }
    [field: SerializeField] public TMP_InputField IntroductionInputField { get; set; }

    [field: Title("[ Default ]")]
    [field: SerializeField] public Sprite DefaultSprite { get; set; }

    [field: Title("[ Button ]")]
    [field: SerializeField] public Button ChangeProfileImage { get; set; }
    [field: SerializeField] public Button CheckNickNameButton { get; set; }
    [field: SerializeField] public Button SaveButton { get; set; }
    [field: SerializeField] public Button CancelButton { get; set; }
}