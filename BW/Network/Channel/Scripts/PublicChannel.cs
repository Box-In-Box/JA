using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PublicChannel : MonoBehaviour
{
    public RectTransform content;
    private ChannelPopup channelPopup;
    public Dictionary<string, Channel> channelDictionary = new Dictionary<string, Channel>();

    private void Awake()
    {
        channelPopup = GetComponentInParent<ChannelPopup>();

        for (int i = 0; i < PhotonNetworkManager.instance.photonChannelData.publicChannelCount; ++i) {
            // Setting UI
            Channel channel = Instantiate(Resources.Load<Channel>("Channel"));
            channel.transform.SetParent(content, false);
            channel.lockIcon.gameObject.SetActive(false);
            channel.SetChannelName(channelPopup.publicChannelName + (i + 1));
            channel.SetChannelPlayerCount(0, PhotonNetworkManager.instance.photonChannelData.maxPlayerCount_Public);

            // Setting Connect Photon
            string photonChannel = PhotonNetworkManager.instance.photonChannelData.publicChannel + PhotonNetworkManager.instance.photonChannelData.channelSplitMark + i;
            channel.button.onClick.AddListener(() => PhotonNetworkManager.instance.JoinRoom(photonChannel));
            channelDictionary.Add(photonChannel, channel);
        }
    }

    // 업데이트 전 RoomInfo 제거
    public void ClearChannelRoomInfo()
    {
        foreach (var channel in channelDictionary) {
            channel.Value.ClearRoomInfo();
        }
    }

    // 업데이트 후 RoomInfo 없으면 제거
    public void ClearEmptyChannel()
    {
        foreach (var channel in channelDictionary) {
            if (channel.Value.roomInfo == null) {
                channel.Value.ClearRoomPlayerCount();
            }
        }
    }
}
