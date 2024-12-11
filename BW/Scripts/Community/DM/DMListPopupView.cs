using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DMListPopupView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public TMP_InputField SearchInputField { get; set; }
    [field: SerializeField] public Transform DMListContentPanel { get; set; }
}