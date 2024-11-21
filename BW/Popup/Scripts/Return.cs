using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Return : MonoBehaviour
{
    public void Awake()
    {
        if (this.TryGetComponent<Button>(out Button button)) {
            Popup popup = this.GetComponentInParent<Popup>();
            button.onClick.AddListener(() => { popup.isCanceled = true; PopupManager.instance.Close(popup); });
        }
    }
}