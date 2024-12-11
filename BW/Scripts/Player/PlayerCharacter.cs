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
    public bool IsMine { get; set; }

    /// <summary>
    /// Hud UI 표시 여부
    /// </summary>
    /// <param name="nickName">닉네임 Hud UI 출력 여부</param>
    /// <param name="chatBubble">말풍선 Hud UI 출력 여부</param>
    public void ShowHud(bool nickName = true, bool chatBubble = true) => HudUI_Player.ShowHud(nickName, chatBubble);
}