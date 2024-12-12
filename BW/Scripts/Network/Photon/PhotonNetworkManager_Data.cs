using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;

[System.Serializable]
public class PhotonPlayerData
{
    [Title("[ Infomation ]")]
    public int uuid;
    public string nickName;
    public PhotonView photonView;
    public string sceneName;
    public PlayerState playerState;
    public PlayerMoveState playerMoveState;

    [Title("[ Avata ]")]
    public PlayerCharacter playerCharacter;
    public bool avartar_dataSpecified;
    public int avartar_gender;
    public string avartar_settingString;
}

[System.Serializable]
public class PhotonChannelData
{
    [ReadOnly] public string currentChannel = ""; // 현재 채널
    [ReadOnly] public string publicChannel = "ch_Public"; // 공용 채널 이름 
    [ReadOnly] public string privateChannel = "ch_Private"; // 개인 채널 이름
    [ReadOnly] public string myRoomChannel = "ch_Myroom"; // 마이룸 채널 이름
    [ReadOnly] public string debugChannel = "ch_Debug"; // Editor 개발자용 채널 이름
    [ReadOnly] public char channelSplitMark = '&'; // RoomInfo Name Split용 (0. channel & 1.Email & 2.Nickname & 3.RoomName & 4.Password) 
    [ReadOnly] public int publicChannelCount = 16; // 공용채널 개수
    [ReadOnly] public int maxPlayerCount_Public = 16; // 채널 최대 수용 인원
    [ReadOnly] public int maxPlayerCount_Private = 4; // 개인 채널 최대 수용 인원
    [ReadOnly] public int maxPlayerCount_MyRoom = 4; // 마이룸 채널 최대 수용 인원
    [ReadOnly] public int currentPlayerCount = 0; // 현재 채널 인원
}

[System.Serializable]
public class PhotonChatData
{
    [ReadOnly] public string publicChannel = "ch_Public"; // 공용 채널 
    [ReadOnly] public string roomChannel = "ch_Room"; // 포톤 방 채널
}

public enum ChannelSplit : byte
{
    Channel,
    UUID,
    NickName,
    RoomName,
    Password,
}

[System.Serializable]
public class Chat_Message
{
    public int uuid;
    public string nickName;
    public string msg;
}

[System.Serializable]
public class MyRoom_Message
{
    public PhotonMyRoomCode code;
}

[System.Serializable]
public class Community_Message
{
    public PhotonCommunityCode code;
    public int sender;
    public int receiver;
    public string msg;
}

public enum PhotonChatCode : byte
{
    Public,
    Room,
};

/// <summary>
/// Photon.IOnEventCallback Event Code
/// </summary>
public enum PhotonEventCode : byte
{
    Chat,
    MyRoom,
    Community,
};

/// <summary>
/// Photon.IOnEventCallback Event Code
/// </summary>
public enum PhotonMyRoomCode : byte
{
    Close, // 마이룸 닫힘 (마이룸 수정 등으로 인해)
};

public enum PhotonCommunityCode : byte
{
    Profile,
    GuestBook,
    DM,
};
