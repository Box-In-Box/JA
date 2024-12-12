using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    private void Awake()
    {
        this.transform.localScale = Vector3.zero;
    }

    public void GoOff(bool value = true)
    {
        if (value)
        {
            if (this.gameObject.activeSelf)
            {
                DOTween.Kill(this.transform);
                this.transform.localScale = Vector3.zero;
                this.transform.DOScale(1f, 1f).SetEase(Ease.OutQuart);
            }
            else
            {
                this.transform.localScale = Vector3.one;
            }
        }
        else
        {
            if (this.gameObject.activeSelf)
            {
                DOTween.Kill(this.transform);
                this.transform.DOScale(0f, 1f).SetEase(Ease.InQuart);
            }
            else
            {
                this.transform.localScale = Vector3.zero;
            }
        }
    }
}
