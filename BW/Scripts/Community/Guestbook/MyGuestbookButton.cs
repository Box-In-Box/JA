using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGuestbookButton : View
{
    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject GuestbookPopupPrefab { get; private set; }

    public override void Awake()
    {
        base.Awake();

        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            var guestbookPopup = PopupManager.instance.Open<GuestbookPopup>(GuestbookPopupPrefab);
            guestbookPopup.Setting(DatabaseConnector.instance.memberUUID, DatabaseConnector.instance.memberData);
        });
    }
}
