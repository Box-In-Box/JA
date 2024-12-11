using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundVolumeControl : MonoBehaviour
{
    private void Start()
    {
        foreach (AudioType type in Enum.GetValues(typeof(AudioType))) {
            SetAudioMixer(type, GetUse(type) ? GetVolume(type) : 0.0001f);
        }
    }

    // Set 오디오 믹서
    public void SetAudioMixer(AudioType type, float level)
    {
        SoundManager.instance.AudioMixer.SetFloat(type.ToString() + "Volume", Mathf.Log10(level) * 20f);
    }
    
    // UI 세팅 
    public void SettingSound(AudioType type, Toggle toggle, Slider slider)
    {
        toggle.isOn = GetVolume(type) > slider.minValue ? GetUse(type) : false;
        toggle.onValueChanged.AddListener((value) => SetUse(type, toggle, slider, value));

        slider.value = toggle.isOn ? GetVolume(type) : slider.minValue;
        slider.onValueChanged.AddListener((value) => SetVolume(type, toggle, slider, value));
        slider.onValueChanged.AddListener((value) => SetAudioMixer(type, value));
    }

    // Get 사용여부(음소거)
    private bool GetUse(AudioType type)
    { 
        return PlayerPrefsData.Get<bool>(type.ToString(), true, false);
    }

    // Get 사운드 볼륨
    private float GetVolume(AudioType type)
    { 
        return PlayerPrefsData.Get<float>(type.ToString() + "Volume", 1f, false);
    }

    // Set 사용여부(음소거)
    private void SetUse(AudioType type, Toggle toggle, Slider slider, bool value)
    {
        // Set Toggle
        PlayerPrefsData.Set<bool>(type.ToString(), value, false);
        SetAudioMixer(type, value ? GetVolume(type) : slider.minValue);
        
        // Slider Exception
        slider.SetValueWithoutNotify(value ? GetVolume(type) : slider.minValue);
    }

    // Set 사운드 볼륨
    private void SetVolume(AudioType type, Toggle toggle, Slider slider, float value)
    {
        // Set Volume
        PlayerPrefsData.Set<float>(type.ToString() + "Volume", value, false);

        // Toggle Exception
        if (value <= slider.minValue) {
            if (GetUse(type)) SetUse(type, toggle, slider, false);
            toggle.isOn = false;
        }
        else {
            if (!GetUse(type)) SetUse(type, toggle, slider, true);
            toggle.isOn = true;
        }
    }
}