using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : Popup
{
    [SerializeField] private Button editProfile;
    [SerializeField] private Button setting;
    [SerializeField] private Button infomation;
    [SerializeField] private Button signOut;
    [SerializeField] private Button exitGame;

    public void Start()
    {
        // 공통
        editProfile.onClick.AddListener(() => PopupManager.instance.Close(this, true));
        setting.onClick.AddListener(() => PopupManager.instance.Close(this, true));
        infomation.onClick.AddListener(() => PopupManager.instance.Close(this, true));

        setting.onClick.AddListener(() => PopupManager.instance.Open<SettingPopup>());
        
        signOut.onClick.AddListener(() => PopupManager.instance.Popup("로그아웃 하시겠습니까?"
            , () => { SceneLoadManager.instance.LoadScene("Login"); PopupManager.instance.Close<OptionPopup>(); }));
        
        exitGame.onClick.AddListener(() => PopupManager.instance.Popup("게임을 종료 하시겠습니까?", () => Application.Quit()));
    }
}