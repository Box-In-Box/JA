using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignPopup : Popup
{
    [field: SerializeField] public SignPopupView SignPopupView { get; set; }
}