using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupOpen : View
{
    [SerializeField] private Popup popup;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() => PopupManager.instance.Open(popup));
    }
}