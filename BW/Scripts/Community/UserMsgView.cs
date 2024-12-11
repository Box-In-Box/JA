using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserMsgView : MonoBehaviour
{
    [field: Title("[ Edit ]")]
    [field: SerializeField] public Image ProfileImage { get; set; }
    [field: SerializeField] public Image AlarmImage { get; set; }
    [field: SerializeField] public TMP_Text NickNameText { get; set; }
    [field: SerializeField] public TMP_Text MsgText { get; set; }
    [field: SerializeField] public TMP_Text DateText { get; set; }

    [field: Title("[ Default ]")]
    [field: SerializeField] public Sprite DefaultSprite { get; set; }

    [field: Title("[ Buttons ]")]
    [field: SerializeField] public Button Button { get; set; }

    public void DM()
    {

    }

    public void GuestBook()
    {

    }
}