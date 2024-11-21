using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Test_Lobby : MonoBehaviour
{
    public Button gotoLogin;
    public Button gotoScene_World;
    public Button gotoStage_0;
    public Button gotoStage_1;

    private void Awake()
    {
        var go = GameManager.instance;
    }

    private void Start()
    {
        gotoLogin.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Login"));

        gotoScene_World.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Stage_Outside"));

        gotoStage_0.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Stage_0"));

        gotoStage_1.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Stage_1")); 
    }
}