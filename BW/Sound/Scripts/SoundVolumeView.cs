using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeView : MonoBehaviour
{
    [field : SerializeField] public Toggle masterToggle { get; private set; }
    [field : SerializeField] public Slider masterSlider  { get; private set; }

    [field : SerializeField] public Toggle musicToggle { get; private set; }
    [field : SerializeField] public Slider musicSlider { get; private set; }
    
    [field : SerializeField] public Toggle sfxToggle { get; private set; }
    [field : SerializeField] public Slider sfxSlider { get; private set; }

    private void Awake()
    {
        masterSlider.minValue = 0.0001f;
        musicSlider.minValue = 0.0001f;
        sfxSlider.minValue = 0.0001f;
    }

    public void OnEnable()
    {
        SoundManager.instance.control.SettingSound(AudioType.Master, masterToggle, masterSlider);
        SoundManager.instance.control.SettingSound(AudioType.BGM, musicToggle, musicSlider);
        SoundManager.instance.control.SettingSound(AudioType.SFX, sfxToggle, sfxSlider);
    }
}