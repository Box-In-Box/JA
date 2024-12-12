using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuestbookPopup : Popup
{
    [field: Title("[ View ]")]
    [field: SerializeField] public GuestbookView View { get; set; }

    [field: Title("[ Dynamic Scroll ]")]
    [field: SerializeField] public UserMsgScrollView UserMsgScrollView { get; set; }

    [field: Title("[ Data ]")]
    [field: SerializeField] public int Uuid { get; set; }
    [field: SerializeField] public UserDataView Data { get; set; }

    [field: SerializeField] public int TESTUuid { get; set; }
    [field: SerializeField] public UserDataView TESTData { get; set; }

    public void Start()
    {
        UserMsgScrollView.Init(new List<UserMsgData>());

        View.WriteButton.onClick.AddListener(() => WriteGuestbook());
    }

    [Button]
    public void TESTSetting()
    {
        Setting(TESTUuid, TESTData);
    }

    public void Setting(int uuid, UserDataView data)
    {
        if (GameManager.instance.isGuest) return;

        Uuid = uuid;
        Data = data;

        UserMsgScrollView.ResetData();

        View.WriteButton.gameObject.SetActive(uuid == DatabaseConnector.instance.memberUUID ? false : true);

        DatabaseConnector.instance.GetGuestbookData((data) => SettingGuestbook(data), null, uuid);
    }

    private void SettingGuestbook(GuestbookItems[] data)
    {
        foreach (var guestbook in data)
        {
            Guestbook(guestbook);
        }
    }

    [Button]
    public void Guestbook(GuestbookItems guestbook)
    {
        var userData = new UserData(guestbook.profile_image, guestbook.writer_uuid, guestbook.writer_nickname, guestbook.writed_info);
        var data = new UserMsgData(UserMsgType.Guestbook, userData, BW.Tool.GetDateTime(guestbook.writer_date));
        UserMsgScrollView.AddData(data, true, true);
    }

    private void WriteGuestbook()
    {
        if (Uuid > 0)
        {
            PopupManager.instance.InputPopup($"{Data.nickname}님의 방명록 글쓰기", (value) => UploadGuestbook(value));
        }
    }

    private void UploadGuestbook(string msg)
    {
        DatabaseConnector.instance.SendGuestbookData(() => Setting(Uuid, Data), null, true, Uuid, msg);
    }
}