using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;

public class PhotonSynchronization_Voice : PhotonSynchronization_Base
{
    private PhotonVoiceView photonVoiceView;
    private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();
        photonVoiceView = GetComponent<PhotonVoiceView>();
    }

    public override void OnWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        stream.SendNext(PhotonVoiceManager.instance.micVolume);
    }

    public override void OnRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    {
        SetSpeaker(photonPlayerData, (float)stream.ReceiveNext());
    }

    public void SetSpeaker(PhotonPlayerData photonPlayerData, float volume)
    {
        if (photonVoiceView == null || photonVoiceView.SpeakerInUse == false) return;

        if (photonPlayerData.sceneName == GameManager.instance.currentScene) {
            photonVoiceView.SpeakerInUse.enabled = true;
            if (audioSource == null) {
                audioSource = GetComponentInChildren<AudioSource>();
            }
            else {
                audioSource.volume = volume;
            }
        }
        else {
            photonVoiceView.SpeakerInUse.enabled = false;
        }
    }

    public override void OnExceptionRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    {
        
    }
}