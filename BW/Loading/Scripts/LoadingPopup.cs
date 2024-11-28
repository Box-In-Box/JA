using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPopup : Popup
{
    [field: SerializeField] public LoadingImage LoadingImage { get; private set; }
    [field: SerializeField] public ProgressView ProgressView { get; private set; }
}