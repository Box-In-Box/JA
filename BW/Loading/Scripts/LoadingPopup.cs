using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPopup : Popup
{
    [field : SerializeField] public ProgressView progressView { get; private set; }
    [field : SerializeField] public Image loadingBackgroundImgage { get; private set; }
    [field : SerializeField] public Image loadingImgage { get; private set; }
    [field : SerializeField] public TMP_Text loadingText { get; private set; }

    [Space(10f)]
    [Range(0f, 10f)] public float loadingImageRotateSpeed = 1f;

    private void Start()
    {
        loadingImgage.transform.DORotate(Vector3.forward * -360f, loadingImageRotateSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

}
