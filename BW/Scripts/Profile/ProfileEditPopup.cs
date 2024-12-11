using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileEditPopup : Popup
{
    [field: Title("[ ProfileEditPopup ]")]
    [field: SerializeField] public ProfileEditPopupView View { get; set; }

    public override void Awake()
    {
        base.Awake();

        Setting();
    }
    public void Setting()
    {
        if (GameManager.instance.isGuest) return;

        SettingView(DatabaseConnector.instance.memberData);
    }

    private void SettingView(UserDataView data)
    {
        ((TextMeshProUGUI)View.NickNameInputField.placeholder).text = data.nickname;
        View.ProfileImage.sprite = data.profile_image ?? View.DefaultSprite;
        ((TextMeshProUGUI)View.IntroductionInputField.placeholder).text = data.introduce;

        //View.ChangeProfileImage.onClick.AddListener();
        //View.CheckNickNameButton.onClick.AddListener();
        //View.SaveButton.onClick.AddListener();
        //View.CancelButton.onClick.AddListener();
    }
}