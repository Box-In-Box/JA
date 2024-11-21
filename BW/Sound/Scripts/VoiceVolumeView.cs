using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceVolumeView : MonoBehaviour
{
    [field : SerializeField] public Toggle inputToggle { get; private set; }
    [field : SerializeField] public Slider inputSlider  { get; private set; }
    
    [field : SerializeField] public Toggle outputToggle { get; private set; }
    [field : SerializeField] public Slider outputSlider { get; private set; }

    private void Awake()
    {
        inputSlider.minValue = 0.0001f;
        outputSlider.minValue = 0.0001f;
    }

    public void OnEnable()
    {
        // Input은 다른 방식
        PhotonVoiceManager.instance.SettingVoiceIn(inputToggle, inputSlider);
        // Output은 기존 방식
        SoundManager.instance.control.SettingSound(AudioType.Voice, outputToggle, outputSlider);
    }
}