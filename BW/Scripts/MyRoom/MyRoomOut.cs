using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Myroom;

public class MyRoomOut : View
{
    [field: SerializeField, ReadOnly] public MyroomController MyroomController { get; set; }
    [field: SerializeField] public Button MyRoomOutButton { get; set; }

    public override void Awake()
    {
        base.Awake();
        if (MyroomController = FindObjectOfType<MyroomController>())
        {
            MyRoomOutButton.onClick.AddListener(() => Out());
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Out()
    {
        if (MyroomController)
        {
            if (!MyroomController.isEditScene)
            {
                PhotonNetworkManager.instance.MyRoomOut(MyroomController.MyRoomOutEvent);
            }
        }
    }
}