using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Sirenix.OdinInspector;
using System;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    private static PhotonChatManager _instance;
    public static PhotonChatManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<PhotonChatManager>();
            }
            return _instance;
        }
    }

    private ChatClient chatClient;
    [field: SerializeField, ReadOnly(true)] public bool isConnecting = false;
    [field: SerializeField, ReadOnly(true)] public PhotonChatData photonChannelData { get; set; } = new PhotonChatData();
    [field: SerializeField, ReadOnly(true)] public ChatState ChatState { get; private set; } = ChatState.Uninitialized;

    [field: Title("[ Message Action ]")]
    public Action<Chat_Message> ChatAction { get; set; }
    public Action<MyRoom_Message> MyRoomAction { get; set; }
    public Action<Community_Message> CommunityAction { get; set; }


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
    }

    private void OnEnable()
    {
        if (PhotonNetworkManager.instance)
        {
            PhotonNetworkManager.instance.onJoinedRoomAction += Connect;
            PhotonNetworkManager.instance.onLeftRoomAction += Disconnect;
        }
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.instance)
        {
            PhotonNetworkManager.instance.onJoinedRoomAction -= Connect;
            PhotonNetworkManager.instance.onLeftRoomAction -= Disconnect;
        }
    }

    private void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.UseBackgroundWorkerForSending = true;
    }

    public void Connect()
    {
        if (isConnecting || chatClient.State != ChatState.Uninitialized && chatClient.State != ChatState.Disconnected) return;
        isConnecting = true;

        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat
            , Application.version
            , new AuthenticationValues(Gongju.Web.DatabaseConnector.instance.memberUUID.ToString()));
    }

    public void OnConnected()
    {
        Debug.Log($"[ Chat(Ãª) Info ] : {chatClient.State}");
        photonChannelData.roomChannel = PhotonNetwork.CurrentRoom.Name;
        chatClient.Subscribe(new string[] { photonChannelData.publicChannel, photonChannelData.roomChannel });
        isConnecting = false;
    }

    public void Disconnect()
    {
        if (isConnecting || chatClient.State != ChatState.ConnectedToFrontEnd && chatClient.State != ChatState.ConnectedToNameServer) return;
        isConnecting = true;

        chatClient.Disconnect();
    }

    public void OnDisconnected()
    {
        isConnecting = false;
    }

    void Update()
    {
        ChatState = chatClient.State;

        chatClient.Service();
    }


    [Button]
    public void SendMessage(PhotonChatCode code, int sender, string sender_nickname, string msg)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            Hashtable data = new Hashtable();
            data.Add("sender", sender);
            data.Add("sender_nickname", sender_nickname);
            //data.Add("sender_image", Gongju.Web.DatabaseConnector.instance.memberData.profile_image); // Å©±â Á¦ÇÑ ÃÑ 400,000 ~ 500,000 (bytes)
            data.Add("msg", msg);

            switch (code)
            {
                case PhotonChatCode.Public:
                    chatClient.PublishMessage(photonChannelData.publicChannel, data);
                    break;
                case PhotonChatCode.Room:
                    chatClient.PublishMessage(photonChannelData.roomChannel, data);
                    break;
            }
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            Hashtable msgHash = messages[i] as Hashtable;
            Chat_Message chat_Message = new Chat_Message();
            chat_Message.uuid = (int)msgHash["sender"];
            chat_Message.nickName = msgHash["sender_nickname"].ToString();
            chat_Message.msg = msgHash["msg"].ToString();

            ChatAction?.Invoke(chat_Message);
        }
    }

    [Button]
    public void SendMessage(PhotonCommunityCode code, int sender, int receiver, string msg)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            Hashtable data = new Hashtable();
            data.Add("code", code);
            data.Add("sender", sender);
            data.Add("receiver", receiver);
            data.Add("msg", msg);

            chatClient.SendPrivateMessage(receiver.ToString(), data);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (sender == Gongju.Web.DatabaseConnector.instance.memberUUID.ToString()) return;

        Hashtable data = message as Hashtable;
        Community_Message community_Message = new Community_Message();
        community_Message.code = (PhotonCommunityCode)data["code"];
        community_Message.sender = (int)data["sender"];
        community_Message.receiver = (int)data["receiver"];
        community_Message.msg = data["msg"].ToString();

        CommunityAction?.Invoke(community_Message);
    }

    public void OnChatStateChange(ChatState state) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnSubscribed(string[] channels, bool[] results) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }

    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR) Debug.LogError(message);
        else if (level == DebugLevel.WARNING) Debug.LogWarning(message);
        else Debug.Log(message);
    }

    public void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }
}
