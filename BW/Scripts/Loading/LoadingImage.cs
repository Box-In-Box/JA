using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingImage : MonoBehaviour
{
    [field: SerializeField] public Image LoadingBackgroundImgage { get; private set; }
    [field: SerializeField] public Image LoadingImgage { get; private set; }
    [field: SerializeField] public TMP_Text LoadingText { get; private set; }
    [Range(0f, 10f)] public float loadingImageRotateSpeed = 1f;

    private void OnEnable()
    {
        LoadingImgage.transform.DORotate(Vector3.forward * -360f, loadingImageRotateSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    private void OnDisable()
    {
        LoadingImgage.DOKill();
    }
}
