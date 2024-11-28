using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gongju.Web;

public class Test_Login : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button guestButton;

    private void Start()
    {
        loginButton.onClick.AddListener(() => Login());
        guestButton.onClick.AddListener(() =>
        {
            GameManager.instance.Guest();
            SceneLoadManager.instance.LoadScene("Lobby");
        });
    }

    public void Login()
    {
        string id = idInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (id == "" || password == "") return;

        PopupManager.instance.Open<LoadingPopup>();

        DatabaseConnector.instance.GetMemberUUID(() => {
            DatabaseConnector.instance.GetMemberData(() => {
                DatabaseConnector.instance.GetAvartarData(() => LoginSuccess(), (errorCode) => {}); // Avartar Data
                DatabaseConnector.instance.GetMissionData(() => MissionManager.instance.isMissionData = true, null); // All Mission Data
                DatabaseConnector.instance.GetMemberMissionData(() => MissionManager.instance.isMemberMissionData = true, null); // My Mission Progress Data
                DatabaseConnector.instance.GetQuizData(null, null); // All Quiz Datas
            }, (errorCode) => {});
        }, (errorCode) => {
            PopupManager.instance.Close<LoadingPopup>();
            PopupManager.instance.Popup("아이디 혹은 비밀번호가 일치하지 않습니다.");
        }, id, password);
    }

    public void LoginSuccess()
    {
        SceneLoadManager.instance.LoadScene("Lobby");
    }

    public void LoginFail()
    {
        
    }
}
