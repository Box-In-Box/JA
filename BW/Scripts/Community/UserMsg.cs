using Gongju.Web;
using Microsoft.SqlServer.Server;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static NormalMissionManager;

public class UserMsg : MonoBehaviour
{
    [field: Title("[ View ]")]
    [field: SerializeField] public UserMsgView View { get; set; }

    [field: Title("[ Data ]")]
    [field: SerializeField, ReadOnly] public int Uuid { get; set; }
    [field: SerializeField, ReadOnly] public string Msg { get; set; }
    [field: SerializeField, ReadOnly] public string Time { get; set; }
    [field: SerializeField, ReadOnly] public DateTime DateTime { get; set; }
    [field: SerializeField, ReadOnly] public UserDataView Data { get; set; }
    [field: SerializeField, ReadOnly] public bool IsDM { get; set; }

    public void Setting()
    {
        View.ProfileImage.sprite = Data.profile_image ?? View.DefaultSprite;
        View.NickNameText.text = Data.nickname;
        View.MsgText.text = Msg;
        View.DateText.text = BW.Tool.GetDateTime(Time);
        DateTime = DateTime.ParseExact(Time, "yyyy-MM-dd_HH:mm:ss", null);

        View.Button.onClick.AddListener(() => Popup(Uuid));
    }

    [Button]
    public void DmView()
    {
        View.DM();
        IsDM = true;
    }

    [Button]
    public void GuestBookView()
    {
        View.GuestBook();
        IsDM = false;
    }

    private void Popup(int uuid)
    {
        if (IsDM)
        {
            var popup = PopupManager.instance.Open<DMPopup>(CommunityManager.instance.CommunityPrefab.DMPopup);
            popup.Setting(uuid, Data);
        }
        else
        {

        }
    }
}
