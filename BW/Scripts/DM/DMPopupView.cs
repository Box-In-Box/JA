using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DMPopupView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public TMP_Text NickNameText { get; set; }
    [field: SerializeField] public Transform ContentTransform { get; set; }
    [field: SerializeField] public TMP_InputField ChatInputField { get; set; }

    [field: Title("[ Button ]")]
    [field: SerializeField] public Button SendButton { get; set; }
}
