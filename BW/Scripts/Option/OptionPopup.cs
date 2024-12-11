using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : Popup
{
    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject ProfileEditPopupPrefab { get; private set; }
    [field: SerializeField] public GameObject AttendancePopupPrefab { get; private set; }
    [field: SerializeField] public GameObject SettingPopupPrefab { get; private set; }

    [field: Title("[ Buttons ]")]
    [SerializeField] private Button editProfile;
    [SerializeField] private Button attendance;
    [SerializeField] private Button setting;
    [SerializeField] private Button infomation;
    [SerializeField] private Button signOut;
    [SerializeField] private Button exitGame;

    public void Start()
    {
        editProfile.onClick.AddListener(() => PopupManager.instance.Close(this, true));
        attendance.onClick.AddListener(() => PopupManager.instance.Close(this, true));
        setting.onClick.AddListener(() => PopupManager.instance.Close(this, true));
        infomation.onClick.AddListener(() => PopupManager.instance.Close(this, true));

        // Top
        editProfile.onClick.AddListener(() => PopupManager.instance.Open<ProfileEditPopup>(ProfileEditPopupPrefab));
        attendance.onClick.AddListener(() => PopupManager.instance.Open<AttendancePopup>(AttendancePopupPrefab));
        setting.onClick.AddListener(() => PopupManager.instance.Open<SettingPopup>(SettingPopupPrefab));
        
        // Bottom
        signOut.onClick.AddListener(() => PopupManager.instance.Popup("로그아웃 하시겠습니까?"
            , () => { SceneLoadManager.instance.LoadScene("Login"); PopupManager.instance.Close<OptionPopup>(); }));
        
        exitGame.onClick.AddListener(() => PopupManager.instance.Popup("게임을 종료 하시겠습니까?", () => Application.Quit()));
    }
}