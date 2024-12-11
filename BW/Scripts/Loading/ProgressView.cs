using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressView : MonoBehaviour
{
    [field : SerializeField] public Slider progressbar { get; private set; }
    [field : SerializeField] public TMP_Text progressText { get; private set; }

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowProgress()
    {
        progressbar.value = 0f;
        progressText.text = "0 %";
        this.gameObject.SetActive(true);
    }
}