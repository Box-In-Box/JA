using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(PhotonView))]
public class PhotonObject : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private Action<PhotonMessageInfo> onPhotonInstantiateAction = null;

    public void Subscribe_OnPhotonInstantiate(Action<PhotonMessageInfo> action)
    {
        onPhotonInstantiateAction += action;
    }

    public void UnSubscribe_OnPhotonInstantiate(Action<PhotonMessageInfo> action)
    {
        onPhotonInstantiateAction += action;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //Debug.Log($"[ OnPhotonInstantiate ({this.gameObject.name}) ] => Sender : " + info.Sender.NickName);
        Transform target = ((GameObject)info.Sender.TagObject).GetComponent<PhotonTag>().photonObject.transform;
        this.transform.SetParent(target);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        onPhotonInstantiateAction?.Invoke(info);
    }
}