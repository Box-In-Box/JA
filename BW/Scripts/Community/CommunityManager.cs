using ExitGames.Client.Photon;
using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Prefab_Community
{
    [field: SerializeField] public GameObject ProfilePopup { get; private set; }
    [field: SerializeField] public GameObject DMPopup { get; private set; }
}

public class CommunityManager : MonoBehaviour
{
    private static CommunityManager _instance;
    public static CommunityManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<CommunityManager>();
            }
            return _instance;
        }
    }

    public Action<Community_Message> ProfileAction { get; set; } // 프로필 수정 이벤트
    public Action<Community_Message> GuestBookAction { get; set; } // 방명록 이벤트
    public Action<Community_Message> DmAction { get; set; } // DM 이벤트

    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public Prefab_Community CommunityPrefab { get; private set; }

    public void Awake()
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
    }

    private void OnEnable()
    {
        if (PhotonNetworkManager.instance)
        {
            PhotonNetworkManager.instance.CommunityAction += CommunityRouter;
        }
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.instance)
        {
            PhotonNetworkManager.instance.CommunityAction -= CommunityRouter;
        }
    }

    public void CommunityRouter(Community_Message community_Message)
    {
        if (community_Message.code == PhotonCommunityCode.Profile) ProfileAction?.Invoke(community_Message);
        if (community_Message.code == PhotonCommunityCode.GuestBook) GuestBookAction?.Invoke(community_Message);
        if (community_Message.code == PhotonCommunityCode.DM) DmAction?.Invoke(community_Message);
    }
}