using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Test_Stage : MonoBehaviour
{
    public Button channel_0;
    public Button channel_1;

    public Button gotoLobby;
    public Button gotoLogin;

    public Button gotoScene_World;
    public Button gotoStage_0;
    public Button gotoStage_1;

    public Button gotoScene_0;

    public Button myRommOut;
    public Button myRommIn;

    public UnityEvent myRoomInEvent = new UnityEvent();
    public UnityEvent myRoomOutEvent = new UnityEvent();

    private void Start()
    {
        channel_0.onClick.AddListener(() => PhotonNetworkManager.instance.JoinRoom("channel_0"));
        channel_1.onClick.AddListener(() => PhotonNetworkManager.instance.JoinRoom("channel_1"));

        gotoLobby.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Lobby"));
        gotoLogin.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Login"));

        // 온라인 씬
        gotoScene_World.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Stage_Outside"));
        gotoStage_0.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Stage_0"));
        gotoStage_1.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Stage_1"));

        // 오프라인 씬
        gotoScene_0.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("OffStage_0"));

        // 마이룸
        myRommOut.onClick.AddListener(() => PhotonNetworkManager.instance.MyRoomOut(myRoomOutEvent));
        myRommIn.onClick.AddListener(() => PhotonNetworkManager.instance.MyRoomIn(myRoomInEvent));
    }
}
