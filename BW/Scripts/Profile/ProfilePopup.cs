using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ProfilePopup : Popup
{
    [field: Title("[ ProfilePopup ]")]
    [field: SerializeField] public ProfilePopupView View { get; set; }
    private bool isMine = false;

    public void Setting(int uuid)
    {
        if (GameManager.instance.isGuest) return;

        if (uuid == DatabaseConnector.instance.memberUUID)
        {
            isMine = true;
            SettingView(uuid, DatabaseConnector.instance.memberData);
        }
        else
        {
            isMine = false;
            DatabaseConnector.instance.GetMemberData((userData) => SettingView(uuid, userData), null, uuid);
        }
    }

    private void SettingView(int uuid, UserDataView data)
    {
        View.NickNameText.text = data.nickname;
        View.ProfileImage.sprite = data.profile_image ?? View.DefaultSprite;
        View.LevelText.text = $"Lv.{data.level}";
        View.FriendText.text = data.friends_uuid.Length > 99 ? "99+" : data.friends_uuid.Length.ToString();
        View.RankText.text = "0";

        View.introductionText.text = data.introduce;

        if (isMine)
        {
            View.FollowButton.gameObject.SetActive(false);
            View.DMButton.gameObject.SetActive(false);
        }
        else
        {
            View.FollowButton.onClick.AddListener(() => Follow(uuid));
            View.DMButton.onClick.AddListener(() => DM(uuid));
        }
        View.MyRoomButton.onClick.AddListener(() => MyRoom(uuid));
    }

    private void Follow(int uuid)
    {

    }

    private void DM(int uuid)
    {

    }

    private void MyRoom(int uuid)
    {

    }
}