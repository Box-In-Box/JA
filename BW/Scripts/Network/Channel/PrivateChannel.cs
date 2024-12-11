using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Sirenix.OdinInspector;

public class PrivateChannel : MonoBehaviour
{
    [Serializable] // 개인 채널 생성 창
    public struct CreatePrivateChannel
    {
        public RectTransform createPrivateChannelPanel;
        public Button closeButton;
        public TMP_InputField channelNameInput;
        public TMP_InputField channelPasswordInput;
        public TMP_Text notice;
        public Button createChannelButton;
    }

    [Serializable] // 개인 채널 입장 창 (비밀번호 입력)
    public struct JoinPrivateChannel
    {
        public RectTransform joinPrivateChannelPanel;
        public Button closeButton;
        public TMP_Text channelName;
        public TMP_InputField channelPasswordInput;
        public TMP_Text notice;
        public Button joinChannelButton;
        public string channel;
    }
    
    public RectTransform content;
    private ChannelPopup channelPopup;
    public Dictionary<string, Channel> channelDictionary = new Dictionary<string, Channel>();
    public List<RoomInfo> afterAddChannel = new List<RoomInfo>();
    private RoomInfo currentRoominfo;

    [Title("[ Private Channel ]")]
    public CreatePrivateChannel createPrivateChannel;
    public JoinPrivateChannel joinPrivateChannel;
    public RectTransform emptyPrivateChannel;
    public Button createPrivateChannelButton;

    private void Awake()
    {
        channelPopup = GetComponentInParent<ChannelPopup>();

        // 개인 채널 생성 Setting
        createPrivateChannelButton.onClick.AddListener(() => OnCreatePrivateChannelOpen());
        createPrivateChannel.closeButton.onClick.AddListener(() => OnCreatePrivateChannelClose());
        createPrivateChannel.createChannelButton.onClick.AddListener(() => OnCreatePrivateChannel());

        // 개인 채널 입장 Setting
        joinPrivateChannel.closeButton.onClick.AddListener(() => OnJoinPrivateChannelClose());
        joinPrivateChannel.joinChannelButton.onClick.AddListener(() => OnJoinPrivateChannelEnter());
    }

    public void ReservationAddChannel(RoomInfo roomInfo)
    {
        if (this.gameObject.activeSelf) AddChannel(roomInfo); // 개인 채널 확인 중 즉시 추가
        else afterAddChannel.Add(roomInfo); // 추후 추가
    }

    public void OnEnable() => UpdateChannel(); // 개인 채널 확인 -> 업데이트

    public void UpdateChannel() // 개인 채널 업데이트
    {
        foreach (var roomInfo in afterAddChannel) AddChannel(roomInfo);
        afterAddChannel.Clear();
    }

    public void AddChannel(RoomInfo roomInfo)
    {
        if (channelDictionary.TryGetValue(roomInfo.Name, out Channel value)) return;

        Instantiate(channelPopup.ChannelPrefab).TryGetComponent<Channel>(out Channel channel);
        channel.transform.SetParent(content, false);

        string[] roomNameSplit = roomInfo.Name.Split(PhotonNetworkManager.instance.photonChannelData.channelSplitMark);
        channel.SetChannelName(roomNameSplit[(int)ChannelSplit.RoomName]);
        channel.SetChannelRoomInfo(roomInfo);
        channel.lockIcon.gameObject.SetActive(roomNameSplit[(int)ChannelSplit.Password] == "" ? false : true);
        
        channel.button.onClick.AddListener(() => OnJoinPrivateChannel(roomInfo));
        channelDictionary.Add(roomInfo.Name, channel);

        emptyPrivateChannel.gameObject.SetActive(false);
    }

    public void OnCreatePrivateChannel()
    {
        // 특수문자 이름 불가 추가 필요

        // 0.Channel
        string channelName = PhotonNetworkManager.instance.photonChannelData.privateChannel;
        // 1.Email
        channelName += PhotonNetworkManager.instance.photonChannelData.channelSplitMark;
        channelName += Gongju.Web.DatabaseConnector.instance.memberUUID;
        // 2.Nickname
        channelName += PhotonNetworkManager.instance.photonChannelData.channelSplitMark;
        channelName += Gongju.Web.DatabaseConnector.instance.memberData.nickname;
        // 3.ChannelName
        channelName += PhotonNetworkManager.instance.photonChannelData.channelSplitMark;
        channelName += createPrivateChannel.channelNameInput.text.Trim();
        // 4.Password
        channelName += PhotonNetworkManager.instance.photonChannelData.channelSplitMark;
        channelName += createPrivateChannel.channelPasswordInput.text.Trim();
        PhotonNetworkManager.instance.CreateRoom(channelName);
    }

    private void OnJoinPrivateChannel(RoomInfo roomInfo)
    {
        currentRoominfo = roomInfo; 

        string[] channelSplit = roomInfo.Name.Split(PhotonNetworkManager.instance.photonChannelData.channelSplitMark);
        string channelName = GetChannelNameWithOutPassword(channelSplit);

        // 비밀 번호가 없을 시 바로 입장
        if (channelSplit[(int)ChannelSplit.Password] == "") {
            PhotonNetworkManager.instance.JoinRoom(channelName);
        }
        // 비밀 번호가 있을 시 입력창 출력
        else {
            OpenJoinRoomPassword();
        }
    }

    public void OpenJoinRoomPassword()
    {
        string[] channelSplit = currentRoominfo.Name.Split(PhotonNetworkManager.instance.photonChannelData.channelSplitMark);

        joinPrivateChannel.channel = GetChannelNameWithOutPassword(channelSplit);
        joinPrivateChannel.channelPasswordInput.text = "";
        joinPrivateChannel.channelName.text = channelSplit[(int)ChannelSplit.RoomName];
        
        joinPrivateChannel.joinPrivateChannelPanel.gameObject.SetActive(true);
    }

    // Get 패스워드 제외 포톤 룸 이름
    private string GetChannelNameWithOutPassword(string[] channelSplit)
    {
        return String.Join(PhotonNetworkManager.instance.photonChannelData.channelSplitMark, channelSplit.SkipLast(1).ToArray()) + PhotonNetworkManager.instance.photonChannelData.channelSplitMark;
    }

    // 업데이트 전 RoomInfo 제거
    public void ClearChannelRoomInfo()
    {
        foreach (var channel in channelDictionary) {
            channel.Value.ClearRoomInfo();
        }
    }

    // RoomInfo 제거
    public void RemoveChannel()
    {
        List<string> removeChannel = new List<string>();

        foreach (var channel in channelDictionary) {
            if (channel.Value.roomInfo == null) {
                removeChannel.Add(channel.Key);
            }
        }

        foreach (var remove in removeChannel) {
            Destroy(channelDictionary[remove].gameObject);
            channelDictionary.Remove(remove);
        }

        if (channelDictionary.Count == 0) {
            emptyPrivateChannel.gameObject.SetActive(true);
        }
    }

#region CreatePrivateChannel
    // 개인 채널 만들기 창 Open
    private void OnCreatePrivateChannelOpen()
    {
        ResetCreatePrivateChannel();
        createPrivateChannel.createPrivateChannelPanel.gameObject.SetActive(true);
    }
    // 개인 채널 만들기 창 Close
    private void OnCreatePrivateChannelClose()
    {
        ResetCreatePrivateChannel();
        createPrivateChannel.createPrivateChannelPanel.gameObject.SetActive(false);
    }
    // 캐시 리셋
    private void ResetCreatePrivateChannel()
    {
        createPrivateChannel.channelNameInput.text = "";
        createPrivateChannel.channelPasswordInput.text = "";
        createPrivateChannel.notice.text = "";
    }
#endregion

#region JoinPrivateChannel
    // 개인 채널 입장 창 (비밀번호 입력) Close
    private void OnJoinPrivateChannelClose()
    {
        ResetJoinPrivateChannel();
        joinPrivateChannel.joinPrivateChannelPanel.gameObject.SetActive(false);
    }
    // 캐시 리셋
    private void ResetJoinPrivateChannel()
    {
        joinPrivateChannel.channelName.text = "";
        joinPrivateChannel.channelPasswordInput.text = "";
        joinPrivateChannel.notice.text = "";
    }
    // 개인 채널 입장 확인
    private void OnJoinPrivateChannelEnter()
    {
        PhotonNetworkManager.instance.JoinRoom(joinPrivateChannel.channel + joinPrivateChannel.channelPasswordInput.text);
        joinPrivateChannel.joinPrivateChannelPanel.gameObject.SetActive(false);
    }
#endregion
}
