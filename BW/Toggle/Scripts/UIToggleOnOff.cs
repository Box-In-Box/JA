using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleOnOff : MonoBehaviour
{
    private Toggle toggle;
    [SerializeField] private GameObject[] on_Object;
    [SerializeField] private GameObject[] off_Object;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((value) => ON_OFF_GameObject(value));

        foreach (var on_Obj in on_Object) {
            on_Obj.SetActive(toggle.isOn);
        }

        foreach (var off_Obj in off_Object) {
            off_Obj.SetActive(!toggle.isOn);
        }
    }

    private void ON_OFF_GameObject(bool value)
    {
        foreach (var on_Obj in on_Object) {
            on_Obj.SetActive(value);
        }

        foreach (var off_Obj in off_Object) {
            off_Obj.SetActive(!value);
        }
    }
}