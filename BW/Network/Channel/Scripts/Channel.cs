using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class Channel : MonoBehaviour
{
    public Button button;
    public Image lockIcon;
    public TMP_Text channelName;
    public TMP_Text playerCount;
    public RoomInfo roomInfo { get; private set; }

    public void SetChannelRoomInfo(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        this.playerCount.text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
    }

    public void SetChannelName(string channelName)
    {
        this.channelName.text = channelName;
    }

    public void SetChannelPlayerCount(int playerCount, int maxPlayerCount)
    {
        this.playerCount.text = playerCount + " / " + maxPlayerCount;
    }

    public void ClearRoomInfo()
    {
        this.roomInfo = null;
    }

    public void ClearRoomPlayerCount()
    {
        this.playerCount.text = "0/" + PhotonNetworkManager.instance.photonChannelData.maxPlayerCount_Public;
    }
}
