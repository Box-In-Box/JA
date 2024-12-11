using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyProfileButton : View
{
    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject ProfilePopupPrefab { get; private set; }

    public override void Awake()
    {
        base.Awake();

        this.GetComponent<Button>().onClick.AddListener(() => Click());
    }

    public void Click()
    {
        var popup = PopupManager.instance.Open<ProfilePopup>(ProfilePopupPrefab);
        popup.Setting(DatabaseConnector.instance.memberUUID);
    }
}
