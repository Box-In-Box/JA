using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProfilePopup : Popup
{
    [field: Title("[ ProfilePopup ]")]
    [field: SerializeField] public ProfilePopupView View { get; set; }

    [field: Title("[ Data ]")]
    [field: SerializeField, ReadOnly] public int Uuid { get; set; }
    [field: SerializeField, ReadOnly] public UserDataView Data { get; set; }
    private bool isMine = false;

    private void Start()
    {
        View.FollowButton.onClick.AddListener(() => Follow());
        View.DMButton.onClick.AddListener(() => DM());
        View.MyRoomButton.onClick.AddListener(() => MyRoom());
    }

    public void Setting(int uuid, UserDataView data)
    {
        if (GameManager.instance.isGuest) return;

        Uuid = uuid;
        Data = data;

        isMine = uuid == DatabaseConnector.instance.memberUUID ? true : false;

        SettingView(uuid, data);
    }

    private void SettingView(int uuid, UserDataView data)
    {
        View.NickNameText.text = data.nickname;
        View.ProfileImage.sprite = data.profile_image ?? View.DefaultSprite;
        View.LevelText.text = $"Lv.{data.level}";
        View.FriendText.text = data.friends_uuid != null ? (data.friends_uuid.Length > 99 ? "99+" : data.friends_uuid.Length.ToString()) : "0";
        View.RankText.text = "0";

        View.introductionText.text = data.introduce;

        if (isMine)
        {
            View.FollowButton.gameObject.SetActive(false);
            View.DMButton.gameObject.SetActive(false);
        }
        else
        {
            if (DatabaseConnector.instance.memberData.friends_uuid.Contains(uuid))
            {
                View.FollowButton.gameObject.SetActive(false);
            }
        }
    }

    private void Follow()
    {
        var friendPopup = PopupManager.instance.Get<FriendPopup>();
        DatabaseConnector.instance.SendMemberFriendRequest(() =>
        {
            Setting(Uuid, Data);
            friendPopup?.Setting();
        }, null, true, Uuid);
    }

    private void DM()
    {
        var data = new UserData(Data.profile_image, Uuid, Data.nickname, "");
        var DMPopup = PopupManager.instance.Open<DMPopup>(CommunityManager.instance.CommunityPrefab.DMPopup);
        DMPopup.Setting(data);

        PopupManager.instance.Close<ProfilePopup>(true);
    }

    private void MyRoom()
    {
        PopupManager.instance.Close<FriendPopup>();
        PopupManager.instance.Close<ProfilePopup>();

        // ∏∂¿Ã∑Î ¿Ãµø
    }
}