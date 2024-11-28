using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Myroom;

public class MyRoomBGM : View
{
    [field: SerializeField, ReadOnly] public MyroomController MyroomController { get; set; }
    [field: SerializeField] public Button MyRoomBGMButton { get; set; }

    public override void Awake()
    {
        base.Awake();
        if (MyroomController = FindObjectOfType<MyroomController>())
        {
            //MyRoomBGMButton.onClick.AddListener(() => PopupManager.instance.Open<BGMPopup>());
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}