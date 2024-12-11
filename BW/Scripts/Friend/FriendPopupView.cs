using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendPopupView : MonoBehaviour
{
    [field: Title("[ Friend List ]")]
    [field: SerializeField] public Transform FriendListContentPanel { get; set; }

    [field: Title("[ Friend Search ]")]
    [field: SerializeField] public TMP_InputField FriendSearchInputField { get; set; }
    [field: SerializeField] public Transform FriendSearchContentPanel { get; set; }
}