using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuestbookView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public Transform ContentTransform { get; set; }

    [field: Title("[ Buttons ]")]
    [field: SerializeField] public Button WriteButton { get; set; }
}