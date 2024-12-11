using System.Collections;
using System.Collections.Generic;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Photon.Voice.Unity.Demos;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Photon.Pun;

public class PhotonVoiceManager : MonoBehaviourPunCallbacks
{
    private static PhotonVoiceManager _instance;
    public static PhotonVoiceManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<PhotonVoiceManager>();
            }
            return _instance;
        }
    }

    public Recorder recorder { get; private set; }
    public PunVoiceClient punVoiceClient { get; private set; }
    public PhotonMicrophone photonMicrophone { get; private set; }

    [field : Title("[ Mic Volume ]")]
    [field : SerializeField, Range(0f, 1f)] public float micVolume { get; private set; }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance == this)
        {
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }

        recorder = GetComponent<Recorder>();
        punVoiceClient = GetComponent<PunVoiceClient>();
        photonMicrophone = GetComponent<PhotonMicrophone>();
    }

    private void Start()
    {
        // Set Volume
        micVolume = GetMicMute() ? GetMicVolume() : 0f;

        // Set Photon Voice
        recorder.TransmitEnabled = micVolume > 0 ? true : false;
    }

#region Input Sound
    public void SettingVoiceIn(Toggle toggle, Slider slider)
    {
        toggle.SetValue(micVolume > 0 ? GetMicMute() : false);
        toggle.onValueChanged.AddListener((value) => SetMicMute(toggle, slider, value));

        slider.SetValue(toggle.isOn ? GetMicVolume() : 0f);
        slider.onValueChanged.AddListener((value) => SetMicVolume(toggle, slider, value));
    }

    public bool GetMicMute() 
    { 
        return PlayerPrefsData.Get<bool>("MicMute", true, false); 
    }

    public float GetMicVolume() 
    { 
        return PlayerPrefsData.Get<float>("MicVolume", 1f, false); 
    }

    public void SetMicMute(Toggle toggle, Slider slider, bool value)
    {
        // Set Photon Voice
        recorder.TransmitEnabled = value;

        // Set Toggle
        micVolume = value ? GetMicVolume() : slider.minValue;
        slider.SetValueWithoutNotify(micVolume);
        PlayerPrefsData.Set<bool>("MicMute", value, false);

        // Slider Exception
        if (micVolume <= slider.minValue) toggle.SetValue(false);
    }

    public void SetMicVolume(Toggle toggle, Slider slider, float value)
    {
        // Set Volume
        micVolume  = value;
        PlayerPrefsData.Set<float>("MicVolume", value, false);

        // Toggle Exception
        if (value <= slider.minValue) {
            if (GetMicMute()) SetMicMute(toggle, slider, false);
            toggle.SetValue(false);
        }
        else {
            if (!GetMicMute()) SetMicMute(toggle, slider, true);
            toggle.SetValue(true); 
        }
    }
#endregion
}