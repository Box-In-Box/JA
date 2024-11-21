using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using CoreFunctions;

public class PlayerCharacter : MonoBehaviour
{
    public int Uuid { get; set; }
    public string NickName { get; set; }
    public AvartarControl AvartarControl { get; set; }
    public Animator Animator { get; set; }
    public HudUI_Player HudUI_Player { get; set; }

    /// <summary>
    /// Hud UI ǥ�� ����
    /// </summary>
    /// <param name="nickName">�г��� Hud UI ��� ����</param>
    /// <param name="chatBubble">��ǳ�� Hud UI ��� ����</param>
    public void ShowHud(bool nickName = true, bool chatBubble = true) => HudUI_Player.ShowHud(nickName, chatBubble);
}