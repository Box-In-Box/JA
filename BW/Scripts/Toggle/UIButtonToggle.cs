using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Button onButton;
    [SerializeField] private Button offButton;
    [Title("체크 시 기본 On"), SerializeField] private bool defaultToggle = true;

    private void Awake()
    {
        onButton.onClick.AddListener(() => On());
        offButton.onClick.AddListener(() => Off());

        if (defaultToggle) On();
        else Off();
    }

    private void On()
    {
        if (!toggle.isOn) toggle.isOn = true;
    }

    private void Off()
    {
        if (toggle.isOn) toggle.isOn = false;
    }
}
