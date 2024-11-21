using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gongju.Web;

public class SystemCall : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.Call();
        PhotonNetworkManager.instance.Call();
        PhotonVoiceManager.instance.Call();
        SceneLoadManager.instance.Call();
        PopupManager.instance.Call();
        SoundManager.instance.Call();
        MissionManager.instance.Call();
        DatabaseConnector.instance.Call();
        DatabaseConnector.instance.webDatabase = Resources.Load<WebDatabase>("Data/Web Database");
    }
}