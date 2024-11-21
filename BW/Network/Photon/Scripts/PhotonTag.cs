using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonTag : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{   
    [field : SerializeField] public GameObject photonObject { get; set; }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;
    }
}