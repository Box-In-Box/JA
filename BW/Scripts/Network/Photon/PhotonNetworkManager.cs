using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;
using System.Linq;
using UnityEditor;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks, ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback
{
    /// <summary>
    /// *** 에디터와 빌드 버전 포톤 연동 안될 때 ***
    /// Connect()의 PhotonNetwork 설정이 이루어 지는가 확인, Build Settings에 Development Build 체크 확인
    /// </summary>
     
    private static PhotonNetworkManager _instance;
    public static PhotonNetworkManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<PhotonNetworkManager>();
            }
            return _instance;
        }
    }

    [field : Title("[ Photon Info ]", "포톤 현재 정보")]
    [field : SerializeField, ReadOnly(true)] public ClientState photonState { get; private set; } = ClientState.PeerCreated;
    [ReadOnly(true)] public ConnectionProtocol photonProtocol = ConnectionProtocol.Udp;
    [ShowIf(nameof(photonProtocol), ConnectionProtocol.Udp), ReadOnly] public int photonNetPort_UDP = 5058;
    [ShowIf(nameof(photonProtocol), ConnectionProtocol.Tcp), ReadOnly] public int photonNetPort_TCP = 4533;
    private  DefaultPool photonPool = PhotonNetwork.PrefabPool as DefaultPool;
    [SerializeField, ReadOnly] private bool isConnecting = false;

    [field: Title("[ Prefab ]", "프리팹")]
    [field: SerializeField] public GameObject ChannelPopupPrefab { get; private set; }

    [field : Title("[ Photon Pool ]", "온라인 생성 프리팹")]
    [field : SerializeField] public List<GameObject> photonPoolList { get; private set; } = new List<GameObject>();
    
    [field : Title("[ Current Photon Object ]", "현재 생성된 온라인 프리팹 (List))")]
    [field : SerializeField, ReadOnly] public List<GameObject> currentPhotonObjectList { get; private set; } = new List<GameObject>();

    [field : Title("[ Room Info ]", "포톤 채널 정보 (For 로비)")]
    [field : SerializeField] public SerializedDictionary <string, RoomInfo> photonRoomDic { get; set; } = new SerializedDictionary<string, RoomInfo>();
    [field : SerializeField] public Action photonRoomUpdateAction { get; set; }
    
    [field : Title("[ Player Info ]", "현재 채널 플레이어 정보 (Dic)"), Searchable]
    [field : SerializeField] public SerializedDictionary<string, PhotonPlayerData> photonPlayerDic { get; set; } = new SerializedDictionary<string, PhotonPlayerData>();

    [field : Title("[ Channel Info ]")]
    public PhotonChannelData photonChannelData { get; set; } = new PhotonChannelData();
    public  TypedLobby defaultLobby { get; set; } = new TypedLobby("lobby", LobbyType.Default);

    [field : Title("[ Message Info ]")]
    public Action<Chat_Message> ChatAction { get; set; }
    public Action<MyRoom_Message> MyRoomAction { get; set; }
    public Action<Community_Message> CommunityAction { get; set; }

    [field : Title("[ Event Action ]")]
    public Action onJoinedLobbyAction { get; set; }
    public Action onJoinedRoomAction { get; set; }
    public Action<EventData> photonOnEventAction { get; set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance == this)
        {
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }

        AddPhotonPool(photonPoolList);
    }

    private void Start()
    {
        StartCoroutine(UpdateCoroutine());
    }

#region Login -> Lobby
    // 1. 접속
    public void Connect()
    {
        if (isConnecting || photonState != ClientState.PeerCreated && photonState != ClientState.Disconnected) return;
        isConnecting = true;

        PhotonNetwork.SendRate = 10; //초당 Package 전송횟수
        PhotonNetwork.SerializationRate = 10; // 초당 OnPhotonSerialize 실행 횟수
        PhotonNetwork.GameVersion = Application.version; //게임버전에 따라서 사람들을 매칭

        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = photonProtocol;
        if(PhotonNetwork.PhotonServerSettings.AppSettings.Protocol == ConnectionProtocol.Udp) {
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = photonNetPort_UDP;
        }
        else if(PhotonNetwork.PhotonServerSettings.AppSettings.Protocol == ConnectionProtocol.Tcp) {
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = photonNetPort_TCP;
        }

        PhotonNetwork.NickName = Gongju.Web.DatabaseConnector.instance.memberData.nickname;
        PhotonNetwork.ConnectUsingSettings();
    }

    // 1.1 접속확인
    public override void OnConnectedToMaster() => JoinLobby();

    // 2. 로비
    public void JoinLobby() => PhotonNetwork.JoinLobby(defaultLobby);

    // 2.1 로비확인
    public override void OnJoinedLobby()
    {
        photonChannelData.currentChannel = PhotonNetwork.CurrentLobby.Name;
        photonRoomDic.Clear();
        isConnecting = false;

        onJoinedLobbyAction?.Invoke();
        Debug.Log($"[ Lobb(로비) Info ] : {PhotonNetwork.CurrentLobby} + region( {PhotonNetwork.CloudRegion} )");
    }
#endregion

#region Lobby -> Room
    // 3 채널 (Join Or CreateRoom) for Public Channel & My Room & Private Join
    public void JoinRoom(string channel, bool isOpen = true, bool isVisible = true)
    {
        if (isConnecting || photonState != ClientState.JoinedLobby && photonState != ClientState.Joined) return;
        isConnecting = true;
        
        string[] channelSplit = channel.Split(photonChannelData.channelSplitMark);
        // Lobby -> Room
        if (photonState == ClientState.JoinedLobby) {
            // Public Channel
            if (channelSplit[(int)ChannelSplit.Channel] == photonChannelData.publicChannel)
                JoinOrCreateRoom(channel, photonChannelData.maxPlayerCount_Public, isOpen, isVisible);
            // Debug Channel
            else if (channelSplit[(int)ChannelSplit.Channel] == photonChannelData.debugChannel) {
                JoinOrCreateRoom(channel, photonChannelData.maxPlayerCount_Public, isOpen, false);
            }
            // MyRoom Channel
            else if (channelSplit[(int)ChannelSplit.Channel] == photonChannelData.myRoomChannel) {
                JoinOrCreateRoom(channel, photonChannelData.maxPlayerCount_MyRoom, isOpen, false);
            }
            // Private Channel
            else if (channelSplit[(int)ChannelSplit.Channel] == photonChannelData.privateChannel) {
                PhotonNetwork.JoinRoom(channel);
            }
        }

        // Room -> Room (Change Channel)
        if (photonState == ClientState.Joined) {
            if (channel == photonChannelData.currentChannel) {
                Debug.LogWarning($"[ {channel} ] 같은 채널로 이동할 수 없습니다.");
                isConnecting = false;
                return;
            }
            StartCoroutine(ChangeRoom(channel));
        }
    }

    private void JoinOrCreateRoom(string channel, int maxPlayerCount, bool isOpen, bool isVisible)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayerCount;
        roomOptions.IsOpen = isOpen;
        roomOptions.IsVisible = isVisible;
        roomOptions.CustomRoomProperties = new Hashtable { {"null", "null"}, }; // 룸 커스텀 프로퍼티 (로비에서 안보임)
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "null" }; // 로비에서 보이도록 설정
        PhotonNetwork.JoinOrCreateRoom(channel, roomOptions, defaultLobby);
    }

    // 3.1 채널 (Create) for Private Channel
    public void CreateRoom(string channel, bool isOpen = true, bool isVisible = true)
    {
        if (isConnecting || photonState != ClientState.JoinedLobby && photonState != ClientState.Joined) return;
        isConnecting = true;

        // Lobby -> Room
        if (photonState == ClientState.JoinedLobby) {
            CreateRoom(channel, photonChannelData.maxPlayerCount_Private, isOpen, isVisible);
        }
    }

    private void CreateRoom(string channel, int maxPlayerCount, bool isOpen, bool isVisible)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayerCount;
        roomOptions.IsOpen = isOpen;
        roomOptions.IsVisible = isVisible;
        roomOptions.CustomRoomProperties = new Hashtable { {"null", "null"}, };
        PhotonNetwork.CreateRoom(channel, roomOptions, defaultLobby);
    }
    
    // 3.2 채널 (Change)
    private IEnumerator ChangeRoom(string channel)
    {
        GotoLobby();
        yield return new WaitUntil(() => photonState == ClientState.JoinedLobby);

        JoinRoom(channel);
        yield return new WaitUntil(() => photonState == ClientState.Joined);
    }

    // 3.3 채널확인
    public override void OnJoinedRoom()
    {
        LocalPlayerCustomProperties();
        InstantiatePhotonPlayer();

        photonChannelData.currentChannel = PhotonNetwork.CurrentRoom.Name;
        isConnecting = false;

        onJoinedRoomAction?.Invoke();
        Debug.Log($"[ Room(채널) Info ] : {PhotonNetwork.CurrentRoom}");
    }

    // 3.2 Photon 플레이어 프로퍼티
    public void LocalPlayerCustomProperties(string sceneName = "Null")
    {
        if (PhotonNetwork.NetworkingClient.InRoom) {
            Hashtable hash = new Hashtable();
            hash.Add("uuid", Gongju.Web.DatabaseConnector.instance.memberUUID);
            hash.Add("nickName", PhotonNetwork.NickName);
            hash.Add("sceneName", sceneName == "Null" ? SceneLoadManager.instance.targetScene : sceneName);
            hash.Add("avartar_dataSpecified", false);
            hash.Add("avartar_gender", -1);
            hash.Add("avartar_settingString", "null");
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void SetLocalPlayerAvartarCustomProperties(bool avartar_dataSpecified, int avartar_gender, string avartar_settingString)
    {
        if (PhotonNetwork.NetworkingClient.InRoom) {
            Hashtable hash = new Hashtable();
            hash.Add("avartar_dataSpecified", avartar_dataSpecified);
            hash.Add("avartar_gender", avartar_gender);
            hash.Add("avartar_settingString", avartar_settingString);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void SetLocalPlayerSpawnCustomProperties(Vector3 position, Quaternion rotation)
    {
        if (PhotonNetwork.NetworkingClient.InRoom) {
            Hashtable hash = new Hashtable();
            hash.Add("spawn_position", position);
            hash.Add("spawn_rotation", rotation);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public object[] GetLocalPlayerAvartarCustomProperties()
    {
        object[] avartarData = new object[3];
        avartarData[0] = PhotonNetwork.LocalPlayer.CustomProperties["avartar_dataSpecified"];
        avartarData[1] = PhotonNetwork.LocalPlayer.CustomProperties["avartar_gender"];
        avartarData[2] = PhotonNetwork.LocalPlayer.CustomProperties["avartar_settingString"];
        return avartarData;
    }

    // 3.3 Photon 플레이어 동기화
    public void InstantiatePhotonPlayer()
    {
        PhotonNetwork.Instantiate("PhotonView", Vector3.zero, Quaternion.identity);
    }
#endregion

#region Player Update
    public IEnumerator UpdateCoroutine()
    {
        while (true) {
            yield return new WaitForEndOfFrame();

            photonState = PhotonNetwork.NetworkingClient.State;

            photonChannelData.currentPlayerCount = PhotonNetwork.NetworkingClient.InRoom ? PhotonNetwork.NetworkingClient.CurrentRoom.Players.Count : -1;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GameManager.instance.RemovePlayerCharacter(otherPlayer);
        RemoveRoomPlayer(otherPlayer);
    }

    public void RemoveRoomPlayer(Player player)
    {
        PhotonPlayerData photonPlayerData = null;
        photonPlayerDic.TryGetValue(player.NickName, out photonPlayerData);

        if (photonPlayerData != null) {
            photonPlayerDic.Remove(player.NickName);
        }
    }
    public void AllRemoveRoomPlayer()
    {
        photonPlayerDic.Clear();
    }

    public PhotonPlayerData GetLocalPhotonPlayerData()
    {
        photonPlayerDic.TryGetValue(PhotonNetwork.NickName, out PhotonPlayerData value);
        return value;
    }
#endregion

#region Room -> Lobby
    public void GotoLobby()
    {
        if (isConnecting && photonState != ClientState.Joined) return;
        isConnecting = true;
        GameManager.instance.AllRemovePlayerCharacter();
        AllRemoveRoomPlayer();
        photonChannelData.currentPlayerCount = 0;

        PhotonNetwork.LeaveRoom();
    }
#endregion

#region Room, Lobby -> Login
    // 4. 
    public void Disconnect()
    {
        if (isConnecting || photonState == ClientState.PeerCreated || photonState == ClientState.Disconnected) return;
        isConnecting = true;
        GameManager.instance.AllRemovePlayerCharacter();
        AllRemoveRoomPlayer();
        photonChannelData.currentPlayerCount = 0;

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        photonChannelData.currentChannel = "login";
        isConnecting = false;
    }
#endregion

#region Duplicate login (중복 로그인)
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(targetPlayer.IsLocal == false && targetPlayer.NickName == "ServerData.Instance.user_info.uuid") {
            //"중복로그인이 감지되어 로그아웃 됩니다"
            //Disconnect();
        }         
    }
#endregion

#region Room Update
    public override void OnRoomListUpdate(List<RoomInfo> roomList) => StartCoroutine(RoomListUpdateCoroutine(roomList));
    IEnumerator RoomListUpdateCoroutine(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            // 룸 삭제
            if (room.RemovedFromList) {
                photonRoomDic.Remove(room.Name);
            }
            // 룸 갱신
            else if (photonRoomDic.ContainsKey(room.Name)) {
                photonRoomDic[room.Name] = room;
            }
            // 룸 추가
            else {
                photonRoomDic.Add(room.Name, room);
            }
            yield return null;
        }
        photonRoomUpdateAction?.Invoke();
    }
#endregion

#region 채널 입장 실패
    // 3.3 채널 입장 실패
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("OnJoinRoomFailed" + " : " + returnCode + " / " + message);
        PopupManager.instance.Close<LoadingPopup>();

        if(returnCode == 32765) { // Game full
            ChannelFull();
        }
        else if (returnCode == 32758) { // Game Does Not Exist (OR 비밀 번호가 틀릴 경우)
            WrongPassword();
        }
        else if (returnCode == 32764) { // Game Closed

        }
    }

    // 3.3.1 채널 입장 실패 (Full)
    public void ChannelFull()
    {
        PopupManager.instance.Popup("채널에 인원이 풀입니다.다른채널을 사용하세요");
        if (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)  {
            JoinLobby();
        }
    }

    public void WrongPassword()
    {
        var channel = PopupManager.instance.Open<ChannelPopup>(ChannelPopupPrefab);
        channel.privateChannel.OpenJoinRoomPassword();
        PopupManager.instance.Popup("패스워드가 일치하지 않습니다.");
        if (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)  {
            JoinLobby();
        }
    }
#endregion

#region Photon Pool Object
    public GameObject PhotonInstantiate(string photonOjbectName, bool isPlayerTransform = true)
    {
        int parentViewID = ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetPhotonView().ViewID;
        string word = "example";
        object[] myCustomInitData = new object[3];
        myCustomInitData[0] = parentViewID;
        myCustomInitData[1] = word;

        GameObject photonOjbect = PhotonNetwork.Instantiate(photonOjbectName, new Vector3(0f, 0f, 0f), Quaternion.identity, 0, myCustomInitData);
        currentPhotonObjectList.Add(photonOjbect);
        return photonOjbect;
    }

    // Instantiate
    public void PhotonDestroy(GameObject photonOjbect)
    {
        if (currentPhotonObjectList.Contains(photonOjbect)) {
            currentPhotonObjectList.Remove(photonOjbect);
            PhotonNetwork.Destroy(photonOjbect);
        }
    }
    public void PhotonDestroyAll()
    {
        for (int i = currentPhotonObjectList.Count - 1; i >= 0; i--) {
            PhotonDestroy(currentPhotonObjectList[i]);
        }
    }
    // Add
    public void AddPhotonPool(GameObject pool)
    {
        if (this.photonPool.ResourceCache.ContainsKey(pool.name)) return;
        if (!pool.GetComponent<PhotonObject>()) pool.AddComponent<PhotonObject>();
        this.photonPool.ResourceCache.Add(pool.name, pool);
    }
    public void AddPhotonPool(List<GameObject> poolList)
    {
        foreach (var pool in poolList) {
            AddPhotonPool(pool);
        }
    }
#endregion

#region Photon Event
    public void OnEvent(EventData photonEvent) => photonOnEventAction?.Invoke(photonEvent);

    public override void OnEnable()
    {
        base.OnEnable();
        photonOnEventAction += GetChatMessage;
        photonOnEventAction += GetMyRoomMessage;
        photonOnEventAction += GetCommunityMessage;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        photonOnEventAction -= GetChatMessage;
        photonOnEventAction -= GetMyRoomMessage;
        photonOnEventAction -= GetCommunityMessage;
    }
#endregion

#region Chat Message
    public void SendChatMessage(int uuid, string nickName, string message)
    {
        if (message == "") return;

        Hashtable eventData = new Hashtable();
        eventData.Add("uuid", uuid);
        eventData.Add("nickName", nickName);
        eventData.Add("msg", message);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)PhotonEventCode.Chat, eventData, raiseEventOptions, SendOptions.SendReliable);
    }

    public void GetChatMessage(EventData photonEvent)
    {
        if (photonEvent.Code != (byte)PhotonEventCode.Chat) return;

        Hashtable msgHash = photonEvent.CustomData as Hashtable;
        Chat_Message chat_Message = new Chat_Message();
        chat_Message.uuid = (int)msgHash["uuid"];
        chat_Message.nickName = msgHash["nickName"].ToString();
        chat_Message.msg = msgHash["msg"].ToString();

        ChatAction?.Invoke(chat_Message);
    }
#endregion

#region MyRoom
    public void SendMyRoomMessage(PhotonMyRoomCode code)
    {
        if (GameManager.instance.currentScene != "MyRoom") return;

        Hashtable eventData = new Hashtable();
        eventData.Add("code", code);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)PhotonEventCode.MyRoom, eventData, raiseEventOptions, SendOptions.SendReliable);
    }

    public void GetMyRoomMessage(EventData photonEvent)
    {
        if (GameManager.instance.currentScene != "MyRoom") return;
        if (photonEvent.Code != (byte)PhotonEventCode.MyRoom) return;

        Hashtable msgHash = photonEvent.CustomData as Hashtable;
        MyRoom_Message myRoom_Message = new MyRoom_Message();
        myRoom_Message.code = (PhotonMyRoomCode)msgHash["code"];

        MyRoomAction?.Invoke(myRoom_Message);
    }

    public void MyRoomOut(UnityEngine.Events.UnityEvent callback)
    {
        if (GameManager.instance.currentScene != "MyRoom") return;
        if (isConnecting || photonState != ClientState.Joined) return;

        Action onJoinedLobbyActionHandler = () => callback?.Invoke();
        onJoinedLobbyAction += onJoinedLobbyActionHandler;
        onJoinedLobbyAction += () => onJoinedLobbyAction -= onJoinedLobbyActionHandler;

        SendMyRoomMessage(PhotonMyRoomCode.Close);
        GotoLobby();
    }

    public void MyRoomIn(UnityEngine.Events.UnityEvent callback)
    {
        if (GameManager.instance.currentScene != "MyRoom") return;
        if (isConnecting || photonState != ClientState.JoinedLobby) return;

        Action onJoinedRoomActionHandler = () => callback?.Invoke();
        onJoinedRoomAction += onJoinedRoomActionHandler;
        onJoinedRoomAction += () => onJoinedRoomAction -= onJoinedRoomActionHandler;

        JoinRoom(GetMyRoomChannel());
        GameManager.instance.PhotonNetworkConnect();
    }
#endregion

#region Community
    public void SendCommunityMessage(PhotonCommunityCode code, string sender, string receiver)
    {
        Hashtable eventData = new Hashtable();
        eventData.Add("code", code);
        eventData.Add("sender", sender);
        eventData.Add("receiver", receiver);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)PhotonEventCode.Community, eventData, raiseEventOptions, SendOptions.SendReliable);
    }

    public void GetCommunityMessage(EventData photonEvent)
    {
        if (photonEvent.Code != (byte)PhotonEventCode.Community) return;

        Hashtable msgHash = photonEvent.CustomData as Hashtable;
        Community_Message community_Message = new Community_Message();
        community_Message.code = (PhotonCommunityCode)msgHash["code"];
        community_Message.sender = msgHash["sender"].ToString();
        community_Message.receiver = msgHash["receiver"].ToString();

        CommunityAction?.Invoke(community_Message);
    }
#endregion

#region Get
    public string GetMyRoomChannel()
    {
        string myRoomChannel = photonChannelData.myRoomChannel;
        myRoomChannel += photonChannelData.channelSplitMark;
        myRoomChannel += Gongju.Web.DatabaseConnector.instance.memberUUID;
        return myRoomChannel;
    }
#endregion
}