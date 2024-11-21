using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChannelPopup : Popup
{
    [ReadOnly] public string publicChannelName = "채널 ";
    public PublicChannel publicChannel;
    public PrivateChannel privateChannel;
    public Button lobbyButton;

    public void Start()
    {
        RoomListUpdate();
        lobbyButton.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Lobby"));
    }
    
    public void OnEnable()
    {
        if (PhotonNetworkManager.instance == null) return;

        PhotonNetworkManager.instance.photonRoomUpdateAction += RoomListUpdate; 
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.instance == null) return;

        PhotonNetworkManager.instance.photonRoomUpdateAction -= RoomListUpdate;
    }

    public void RoomListUpdate() => StartCoroutine(RoomListUpdateCoroutine());

    private IEnumerator RoomListUpdateCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // 채널 캐시 삭제
        publicChannel.ClearChannelRoomInfo();
        privateChannel.ClearChannelRoomInfo();

        foreach (var room in PhotonNetworkManager.instance.photonRoomDic) {
            // 공용 채널 추가
            if (publicChannel.channelDictionary.ContainsKey(room.Key)) {
                publicChannel.channelDictionary[room.Key].SetChannelRoomInfo(room.Value);
            }
            // 개인 채널
            else {
                // 갱신
                if (privateChannel.channelDictionary.ContainsKey(room.Key)) {
                    privateChannel.channelDictionary[room.Key].SetChannelRoomInfo(room.Value);
                }
                // 추가
                else {
                    privateChannel.ReservationAddChannel(room.Value);
                }
            }
            yield return null;
        }

        // 채널 삭제
        publicChannel.ClearEmptyChannel();
        privateChannel.RemoveChannel();
    }
}