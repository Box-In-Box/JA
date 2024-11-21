using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PhotonSynchronization_Transform : PhotonSynchronization_Base
{
    [Serializable]
    public class Synchronize_Settings
    {
        public bool synchronize_Position = true;
        public bool synchronize_Rotation = true;
        public bool synchronize_Scale = false;
    }

    // 동기화할 값
    [SerializeField] private Synchronize_Settings synchronize_Settings;

    // 동기화될 캐릭터
    private PlayerCharacter playerCharacter;
    
    // 받은 데이터 값
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkLocalScale;

    // 네트워크 조정 값
    private Vector3 direction;
    private Vector3 storedPosition;
    private float distance;
    private float angle;
    private bool isFirstTake = false;

    void OnEnable()
    {
        storedPosition = transform.localPosition;
        networkPosition = Vector3.zero;
        networkRotation = Quaternion.identity;
        isFirstTake = true;
    }

    public void LateUpdate()
    {
        if (this.photonView.IsMine) {
            if (this.playerCharacter == null) { 
                this.playerCharacter = GameManager.instance.playerCharacter; 
                return;
            }
            this.transform.position = playerCharacter.transform.position;
            this.transform.rotation = playerCharacter.transform.rotation;
        }

        if (this.photonView.IsMine || playerCharacter == null) return;

        if (synchronize_Settings.synchronize_Position) {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.networkPosition, this.distance * Time.deltaTime * PhotonNetwork.SerializationRate);
        }
        if (synchronize_Settings.synchronize_Rotation) {
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, this.networkRotation, this.angle * Time.deltaTime *  PhotonNetwork.SerializationRate);
        }
        if (synchronize_Settings.synchronize_Scale) {
            this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, this.networkLocalScale, this.distance * Time.deltaTime * PhotonNetwork.SerializationRate);
        }

        if (photonPlayerData != null && photonPlayerData.playerCharacter != null) { // 플레이어 캐릭터 동기화
            if (synchronize_Settings.synchronize_Position) {
                photonPlayerData.playerCharacter.transform.position = this.transform.position;
            }
            if (synchronize_Settings.synchronize_Rotation) {
                photonPlayerData.playerCharacter.transform.rotation = this.transform.rotation;
            }
            if (synchronize_Settings.synchronize_Scale) {
                photonPlayerData.playerCharacter.transform.localScale = this.transform.localScale;
            }
        }
    }

    public override void OnWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        if (this.playerCharacter == null) { this.playerCharacter = GameManager.instance.playerCharacter; }

        // Position
        if (this.synchronize_Settings.synchronize_Position) {
            this.direction = playerCharacter.transform.position - this.storedPosition;
            this.storedPosition = playerCharacter.transform.position;
            stream.SendNext(playerCharacter.transform.position);
            stream.SendNext(this.direction);
        }
        // Rotation
        if (this.synchronize_Settings.synchronize_Rotation) {
            stream.SendNext(playerCharacter.transform.rotation);
        }
        // Scale
        if (this.synchronize_Settings.synchronize_Scale) {
            stream.SendNext(playerCharacter.transform.localScale);
        }
    }

    public override void OnRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    {
        if (this.playerCharacter == null) { this.playerCharacter = photonPlayerData.playerCharacter; isFirstTake = true; }

        // Position
        if (this.synchronize_Settings.synchronize_Position) {
            this.networkPosition = (Vector3)stream.ReceiveNext();
            this.direction = (Vector3)stream.ReceiveNext();

            if (isFirstTake) {
                this.transform.position = this.networkPosition;
                this.distance = 0f;
            }
            else {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                this.networkPosition += this.direction * lag;
                this.distance = Vector3.Distance(this.transform.position, this.networkPosition);
            }
        }

        // Roation
        if (this.synchronize_Settings.synchronize_Rotation) {
            this.networkRotation = (Quaternion)stream.ReceiveNext();

            if (isFirstTake) {
                this.angle = 0f;
                this.transform.rotation = this.networkRotation;
            }
            else {
                this.angle = Quaternion.Angle(this.transform.rotation, this.networkRotation);
            }
        }

        // Scale
        if (this.synchronize_Settings.synchronize_Scale) {
            this.networkLocalScale = (Vector3)stream.ReceiveNext();
        }

        // For First Connect
        if (isFirstTake) {
            isFirstTake = false;
        }
    }

    public override void OnExceptionRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    {
        
    }
}