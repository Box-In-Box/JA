using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

[Serializable, RequireComponent( typeof(PhotonView) )]
public abstract class PhotonSynchronization_Base : MonoBehaviourPun, IPunObservable
{
    protected PhotonPlayerData photonPlayerData;
    
    private delegate void OnWriteDelegate(PhotonStream stream, PhotonMessageInfo info);
    private delegate void OnReadDelegate(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData);
    private OnWriteDelegate onWriteDelegate;
    private OnReadDelegate onReadDelegate;
    private OnReadDelegate onExceptionReadDelegate;

    public virtual void Awake()
    {
        transform.SetParent(PhotonNetworkManager.instance.transform);
        onWriteDelegate += OnWrite;
        onReadDelegate += OnRead;
        onExceptionReadDelegate += OnExceptionRead;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) {
            // 나의 아바타가 생성 되어있을 때 Stream Send
            if (GameManager.instance.playerCharacter) {
                onWriteDelegate?.Invoke(stream, info);
            }
        }
        else {
            // 로컬에서 해당 캐릭터 Data가 처리 되어있을 때 Stream Read
            if (PhotonNetworkManager.instance.photonPlayerDic.TryGetValue(info.Sender.NickName, out photonPlayerData)) {
                // 같은 씬 일 때 Stream Read
                if (photonPlayerData.sceneName == GameManager.instance.currentScene) {
                    onReadDelegate?.Invoke(stream, info, ref photonPlayerData);
                }
                // 다른 씬이라도 고정으로 읽기
                onExceptionReadDelegate?.Invoke(stream, info, ref photonPlayerData);
            }
        }
    }

    /// <summary>
    /// 동기화할 목록 보내기 ex) stream.SendNext( {object} );
    /// </summary>
    public abstract void OnWrite(PhotonStream stream, PhotonMessageInfo info);
    /// <summary>
    /// 씬이 같을 때 동기화 ex) stream.ReceiveNext();
    /// </summary>
    public abstract void OnRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData);
    /// <summary>
    /// 씬이 달라도 고정으로 동기화 ex) stream.ReceiveNext();
    /// </summary>
    public abstract void OnExceptionRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData);
}
