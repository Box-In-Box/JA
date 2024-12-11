using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PhotonSynchronization_Data : PhotonSynchronization_Base
{   
    private PhotonTag photonTag;

    public override void Awake()
    {
        base.Awake();
        photonTag = this.GetComponent<PhotonTag>();
    }

    public override void OnWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        stream.SendNext(GameManager.instance.currentScene);
        stream.SendNext(GameManager.instance.playerController.PlayerState);
        stream.SendNext(GameManager.instance.playerController.PlayerMoveState);

        // 나의 Data는 동기화 필요 없지만 보기 좋도록 같이 설정
        PhotonNetworkManager.instance.GetLocalPhotonPlayerData().playerState = GameManager.instance.playerController.PlayerState;
        PhotonNetworkManager.instance.GetLocalPhotonPlayerData().playerMoveState = GameManager.instance.playerController.PlayerMoveState;
    }

    public override void OnRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    {
        
    }

    public override void OnExceptionRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    { 
        photonPlayerData.sceneName = (string)stream.ReceiveNext();
        photonPlayerData.playerState = (PlayerState)stream.ReceiveNext();
        photonPlayerData.playerMoveState = (PlayerMoveState)stream.ReceiveNext();

        photonTag.photonObject.SetActive(photonPlayerData.sceneName == GameManager.instance.currentScene);
    }
}