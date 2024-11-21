using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PhotonSynchronization_Animation : PhotonSynchronization_Base
{
    [Serializable]
    public class AnimParameter
    {
        public AnimatorControllerParameterType type;
        public object value;

        public AnimParameter(AnimatorControllerParameterType type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }
    
    // 동기화 될 애니메이션
    private Animator animator;
    private List<float> networkAnimLayer = new List<float>();
    private List<AnimParameter> networkAnimParameter = new List<AnimParameter>();
    
    private IEnumerator Start()
    {
        if (this.photonView.IsMine) {
            yield return new WaitUntil(() => GameManager.instance.playerCharacter);
            this.animator = GameManager.instance.playerCharacter.Animator;
        }
    }

    public void Update()
    {
        if (this.photonView.IsMine || animator == null) return;

        // Set Layers
        for (int i = 0; i < this.animator.layerCount; ++i) {
            float value = Mathf.Lerp(this.animator.GetLayerWeight(i), networkAnimLayer[i], Time.deltaTime * 5f);
            this.animator.SetLayerWeight(i, value);
        }

        // Set Parameters
        for (int i = 0; i < this.animator.parameters.Length; ++i) {
            var type = networkAnimParameter[i].type;
            object value = networkAnimParameter[i].value;

            switch (type) {
                case AnimatorControllerParameterType.Bool :
                case AnimatorControllerParameterType.Trigger :
                    this.animator.SetBool(this.animator.parameters[i].name, (bool)value);
                    break;
                case AnimatorControllerParameterType.Int :
                    this.animator.SetInteger(this.animator.parameters[i].name, (int)value);
                    break;
                case AnimatorControllerParameterType.Float :
                    if (animator.parameters[i].name == "moveVal")
                        this.animator.SetFloat("moveVal", (float)value, this.animator.GetFloat("moveVal") < MoveAnimation.Walk ? MoveAnimation.ZoroToMoveDamp : MoveAnimation.MoveDamp, Time.deltaTime);
                    else
                        this.animator.SetFloat(animator.parameters[i].name, (float)value, .05f, Time.deltaTime);
                    break;
            }
        }
    }

    public override void OnWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        // 첫 동기화 (캐시 저장)
        if (this.animator == null) { this.animator = GameManager.instance.playerCharacter.Animator; }

        // Layer Send
        for (int i = 0; i < this.animator.layerCount; ++i) {
            stream.SendNext(this.animator.GetLayerWeight(i));
        }

        // Parameters Send
        for (int i = 0; i < this.animator.parameters.Length; ++i) {
            AnimatorControllerParameterType type = this.animator.parameters[i].type;
            stream.SendNext(type);
            switch (type) {
                case AnimatorControllerParameterType.Bool :
                    stream.SendNext(this.animator.GetBool(this.animator.parameters[i].name));
                    break;
                case AnimatorControllerParameterType.Int :
                    stream.SendNext(this.animator.GetInteger(this.animator.parameters[i].name));
                    break;
                case AnimatorControllerParameterType.Float :
                    stream.SendNext(this.animator.GetFloat(this.animator.parameters[i].name));
                    break; 
                case AnimatorControllerParameterType.Trigger : 
                    stream.SendNext(this.animator.GetBool(this.animator.parameters[i].name));
                    break;
            }
        }
    }

    public override void OnRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    {
        // 같은 씬일 때 (캐릭터가 있을 때)
        if (photonPlayerData.playerCharacter) {
            // 첫 동기화 (캐시 저장)
            if (this.animator == null) { this.animator = photonPlayerData.playerCharacter.Animator; }

            // Layer Receive
            networkAnimLayer.Clear();
            for (int i = 0; i < this.animator.layerCount; ++i) {
                networkAnimLayer.Add((float)stream.ReceiveNext());
            }

            // Parameters Receive
            networkAnimParameter.Clear();
            for (int i = 0; i < this.animator.parameters.Length; ++i) {
                var type = (AnimatorControllerParameterType)stream.ReceiveNext();
                object value = stream.ReceiveNext();
                networkAnimParameter.Add(new AnimParameter(type, value));
            }
        }
    }

    public override void OnExceptionRead(PhotonStream stream, PhotonMessageInfo info, ref PhotonPlayerData photonPlayerData)
    {
        
    }
}