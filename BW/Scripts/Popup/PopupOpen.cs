using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupOpen : View
{
    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject popupPrefab { get; private set; }

    public override void Awake()
    {
        base.Awake();

        this.GetComponent<Button>().onClick.AddListener(() => PopupManager.instance.Open<Popup>(popupPrefab));
    }
}