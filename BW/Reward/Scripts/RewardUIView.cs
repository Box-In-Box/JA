using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIView : MonoBehaviour
{
    [field : SerializeField] public Image rewardImage { get; set; }
    [field : SerializeField] public TMP_Text valueText { get; set; }
}