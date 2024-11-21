using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Web;

/// <summary>
/// Scene, Lobby 공용 사용
/// </summary>
public class View_TopPanel : View
{
    [field : Title("[ Return Button ]")]
    [field : SerializeField] public Button returnButton { get; private set; }

    [field : Title("[ User Info ]")]
    [field : SerializeField] public TMP_Text userNameText { get; private set; }

    [field : Title("[ Currency ]")]
    [field : SerializeField] public TMP_Text goldText { get; private set; }
    [field : SerializeField] public TMP_Text cashText { get; private set; }

    public override void Start()
    {
        base.Start();
        SettingReturn();
    }

    private void OnEnable()
    {
        if (DatabaseConnector.instance) {
            DatabaseConnector.instance.actMemberDataRetrieve.AddListenerInvoke(SetUserName);
            DatabaseConnector.instance.actMemberDataRetrieve.AddListenerInvoke(SetCurrency);
        }
    }

    private void OnDisable()
    {
        if (DatabaseConnector.instance) {
            DatabaseConnector.instance.actMemberDataRetrieve.RemoveListener(SetUserName);
            DatabaseConnector.instance.actMemberDataRetrieve.RemoveListener(SetCurrency);
        }
    }

    public void SetUserName()
    {
        userNameText.text = DatabaseConnector.instance.memberData.nickname;
    }

    public void SetCurrency()
    {
        if (DatabaseConnector.instance.memberIDRetrieved) {
            goldText.text = BW.Tool.GetNumberComma(DatabaseConnector.instance.memberData.coin);
            cashText.text = BW.Tool.GetNumberComma(/*DatabaseConnector.instance.memberData.cash*/ 0);
        }
        else {
            goldText.text = BW.Tool.GetNumberComma(0);
            cashText.text = BW.Tool.GetNumberComma(0);
        }
    }

    /// <summary>
    /// 상단 이전 버튼 지정
    /// </summary>
    private void SettingReturn()
    {
        string targetScene = "Null";
        string targetSceneName = "Null";
        switch (GameManager.instance.currentScene) {
            case "Lobby" :
                targetScene = "Login";
                targetSceneName = "로그인화면";
                break;
            default :
                targetScene = "Lobby";
                targetSceneName = "로비";
                break;
        } 
        returnButton?.onClick.AddListener(() => SceneLoadManager.instance.PopupLoadScene(targetScene, targetSceneName));
    }
}