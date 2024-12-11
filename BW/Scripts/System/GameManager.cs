using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;
using CoreFunctions;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<GameManager>();
            }
            return _instance;
        }
    }

    [field : Title("[ Scene ]", "씬 정보")]
    [field : SerializeField, ReadOnly] public string currentScene { get; private set; }
    [field : SerializeField, ReadOnly] public string previousScene { get; private set; }

    [field : Title("[ Player ]", "내 플레이어 정보")]
    [field : SerializeField, ReadOnly] public PlayerCharacter playerCharacter { get; private set; } // 캐릭터
    [field: SerializeField, ReadOnly] public AvartarControl playerAvartar { get; private set; } // 캐릭터 아바타
    [field : SerializeField, ReadOnly] public PlayerController playerController { get; private set; } // 컨트롤러
    [field : SerializeField, ReadOnly] public Animator playerAnimator { get; private set; } // 애니메이터
    [field : SerializeField, ReadOnly] public CameraController PlayerCameraController { get; private set; } // 카메라
    [field : SerializeField, ReadOnly] public bool isGuest { get; set; }

    [field : Title("[ PlayerList ]", "현재 씬 플레이어들 정보"), Searchable]
    [field : SerializeField] public List<PlayerCharacter> playerCharacterList { get; private set; } = new List<PlayerCharacter>();

    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject PlayerCharacterPrefab { get; private set; }
    [field: SerializeField] public GameObject CameraControllerPrefab { get; private set; }
    [field: SerializeField] public GameObject JoystickControllerPrefab { get; private set; }
    [field: SerializeField] public GameObject KeyboardControllerPrefab { get; private set; }

    private Coroutine photonCoroutine;
    private Coroutine playersCoroutine;

    public void Awake()
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

        currentScene = "Null";
        previousScene = "Null";

    #if UNITY_STANDALONE
        Screen.SetResolution(1280, 720, false); // 테스트용
    #elif UNITY_ANDROID || UNITY_IOS
        Screen.fullScreen = true;
    #endif

        string activeScene = SceneManager.GetActiveScene().name;
        try { currentScene = activeScene; }
        catch (ArgumentException e){ Debug.LogError($"{ e }\n Build Settings에 씬( {activeScene} )을(를) 추가해 주세요."); }

        // Get Guest ID
        if (currentScene != "Login") { 
            Guest();
        }
    }
    
    // 씬 전환 시 포톤 동기화 이벤트 설정
    public void Start()
    {
        PhotonNetworkConnect();
    }

    public void OnLoadScene(string scene)
    {
        previousScene = currentScene;
        currentScene = scene;

        if (playersCoroutine != null) StopCoroutine(playersCoroutine);
    }

    // 랜덤 게스트
    public string Guest()
    {
        int random = Random.Range(100000000, 999999999);
        int uuid = random;
        string nickname = "Guest_" + random;

        Gongju.Web.DatabaseConnector.instance.memberUUID = uuid;
        Gongju.Web.DatabaseConnector.instance.memberData.nickname = nickname;
        isGuest = true;

        return nickname;
    }

    public void PhotonNetworkConnect()
    {
        if (photonCoroutine != null) StopCoroutine(photonCoroutine);
        photonCoroutine = StartCoroutine(PhotonNetworkServer());
    }

    // 씬 <-> 포톤 서버 동기화
    private IEnumerator PhotonNetworkServer()
    {
        while (true) {
            yield return new WaitForEndOfFrame();

            // < Login >
            if (currentScene == "Login") {
                if (PhotonNetwork.NetworkingClient.IsConnected) {
                    PhotonNetworkManager.instance.Disconnect();
                    continue;
                }
            }
            // < Loading >
            else if (currentScene == "Loading") {
                if (PhotonNetwork.NetworkingClient.IsConnected == false) {
                    PhotonNetworkManager.instance.Connect();
                    continue;
                }
            }
            // < Lobby >
            else if (currentScene == "Lobby") {
                if (PhotonNetwork.NetworkingClient.IsConnected == false) {
                    PhotonNetworkManager.instance.Connect();
                    continue;
                }

                if (PhotonNetwork.NetworkClientState == ClientState.Joined){
                    PhotonNetworkManager.instance.GotoLobby();
                    continue;
                }

                if (PhotonNetwork.NetworkingClient.InLobby == false) {
                    continue;
                }
                else {
                    // Create Player CharacterIn Lobby
                    CreateLobbyPlayerCharacter();
                    yield break;
                }
            }
            // < Online Scene >
            else if (SceneLoadManager.instance.targetSceneType == SceneType.Online || SceneLoadManager.instance.targetSceneType == SceneType.Myroom) {
                if (PhotonNetwork.NetworkingClient.IsConnected == false) {
                    PhotonNetworkManager.instance.Connect();
                    continue;
                }

                if (PhotonNetwork.NetworkingClient.InRoom == false) {
                    continue;
                }
                else {
                    // Create Player Character
                    if (playersCoroutine != null) StopCoroutine(playersCoroutine);
                    playersCoroutine = StartCoroutine(PhotonNetworkPlayers());
                    yield break;
                }
            }
            // < Offline Scene >
            else if (SceneLoadManager.instance.targetSceneType == SceneType.Offline) {
                if (PhotonNetwork.NetworkingClient.IsConnected == false) {
                    PhotonNetworkManager.instance.Connect();
                    continue;
                }
                
                if (PhotonNetwork.NetworkClientState == ClientState.Joined) {
                    PhotonNetworkManager.instance.GotoLobby();
                    continue;
                }

                if (PhotonNetwork.NetworkingClient.InLobby == false) {
                    continue;
                }
                else {
                    // Create Player Character
                    if (playersCoroutine != null) StopCoroutine(playersCoroutine);
                    playersCoroutine = StartCoroutine(OfflinePlayer());
                    yield break;
                }
            }
            yield break;
        }
    }

    // 동기화된 씬 서버 <-> 캐릭터 동기화
    public IEnumerator PhotonNetworkPlayers()
    {
        while (true) {
            if (!MapInfo.instance || !PhotonNetwork.IsConnected) {
                yield return null;
                continue;
            }
            
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.NetworkingClient.CurrentRoom.Players) {
                // Get Base Data
                PhotonNetworkManager.instance.photonPlayerDic.TryGetValue(player.Value.NickName, out PhotonPlayerData photonPlayerData);
                player.Value.CustomProperties.TryGetValue("uuid", out object uuid);
                player.Value.CustomProperties.TryGetValue("nickName", out object nickName);
                player.Value.CustomProperties.TryGetValue("sceneName", out object sceneName);
                // Null ? => return
                if (uuid == null || nickName == null || sceneName == null || player.Value.TagObject as GameObject == null) continue;
                // Get Avartar Data
                player.Value.CustomProperties.TryGetValue("avartar_dataSpecified", out object avartar_dataSpecified);
                player.Value.CustomProperties.TryGetValue("avartar_gender", out object avartar_gender);
                player.Value.CustomProperties.TryGetValue("avartar_settingString", out object avartar_settingString);

                // New Player
                if (photonPlayerData == null) {
                    photonPlayerData = new PhotonPlayerData();
                    // Set Base Data
                    photonPlayerData.uuid = (int)uuid;
                    photonPlayerData.nickName = nickName.ToString();
                    photonPlayerData.photonView = ((GameObject)player.Value.TagObject).GetComponent<PhotonView>();
                    photonPlayerData.sceneName = sceneName.ToString();
                    // Set Avartar Data
                    photonPlayerData.avartar_dataSpecified = (bool)avartar_dataSpecified;
                    photonPlayerData.avartar_gender = (int)avartar_gender;
                    photonPlayerData.avartar_settingString = (string)avartar_settingString;
                    // <===Create Mine Charator===>
                    if (nickName.ToString() == Gongju.Web.DatabaseConnector.instance.memberData.nickname) {
                        PhotonNetworkManager.instance.photonPlayerDic.Add(player.Value.NickName, photonPlayerData);
                        CreatePlayerCharacter(photonPlayerData);
                    }
                    // <===Create Other Charator===>
                    else {
                        if (avartar_settingString as string == "null") continue;
                        PhotonNetworkManager.instance.photonPlayerDic.Add(player.Value.NickName, photonPlayerData);
                        CreateOtherCharacter(photonPlayerData);
                        // Invisible Cahrator
                        if (sceneName.ToString() != currentScene) {
                            RemovePlayerCharacter(player.Value);
                        }
                    }
                }
                // Remove Player (Different Scene)
                else {
                    if (photonPlayerData.playerCharacter) {
                        if (photonPlayerData.sceneName != currentScene) {
                            if (nickName.ToString() != Gongju.Web.DatabaseConnector.instance.memberData.nickname) {
                                RemovePlayerCharacter(player.Value);
                            }
                        }
                    }
                    else {
                        if (photonPlayerData.sceneName == currentScene) {
                            // <===Create Mine Charator===>
                            if (nickName.ToString() == Gongju.Web.DatabaseConnector.instance.memberData.nickname) {
                                CreatePlayerCharacter(photonPlayerData);
                            }
                            // <===Create Other Charator===>
                            else {
                                if (avartar_settingString as string == "null") continue;
                                CreateOtherCharacter(photonPlayerData);
                            }
                        }
                    }
                }
            }
            // Not Join ? => Break
            if (!PhotonNetwork.NetworkingClient.InRoom) yield break;
            else if (SceneLoadManager.instance.targetSceneType == SceneType.Null) yield break;
            yield return new WaitForEndOfFrame();
        }
    }

    // 오프라인 캐릭터 생성
    public IEnumerator OfflinePlayer()
    {
        yield return null;
        // Get Base Data
        PhotonPlayerData photonPlayerData = new PhotonPlayerData();
        photonPlayerData.uuid = Gongju.Web.DatabaseConnector.instance.memberUUID;
        photonPlayerData.nickName = Gongju.Web.DatabaseConnector.instance.memberData.nickname;
        photonPlayerData.sceneName = this.currentScene;

        // Get Set Avartar
        string defaultData = "";
        if (!PlayerPrefsData.HasKey("GuestAvartarData")) {
            AvartarBaseData baseData = Resources.Load<AvartarBaseData>("Data/Avartar Base");
            defaultData = JsonUtility.ToJson(new AvartarSettingPack() { data = baseData.baseMale });
        }
        photonPlayerData.avartar_dataSpecified = true;
        photonPlayerData.avartar_gender = PlayerPrefsData.Get<int>("GuestAvartarGender", 0);
        photonPlayerData.avartar_settingString = PlayerPrefsData.Get<string>("GuestAvartarData", defaultData);
        CreatePlayerCharacter(photonPlayerData);
    }

    #region Player Character
    /// <summary>
    /// Local Player Create
    /// </summary>
    public GameObject CreatePlayerCharacter(PhotonPlayerData photonPlayerData)
    {
        // Instantiate Player
        Instantiate(PlayerCharacterPrefab
            ,MapInfo.instance.GetSpawnTransform(previousScene).position
            ,MapInfo.instance.GetSpawnTransform(previousScene).rotation)
            .TryGetComponent(out PlayerCharacter playerCharacter);
        playerCharacter.gameObject.name = $"[Player]({photonPlayerData.uuid})";

        playerCharacter.Uuid = photonPlayerData.uuid;
        playerCharacter.NickName = photonPlayerData.nickName;
        playerCharacter.IsMine = true;

        // Instantiate Player Avatar
        AvartarControl avartar = Instantiate(Resources.Load<AvartarControl>("Prefabs/Avartar"));
        avartar.transform.SetParent(playerCharacter.transform, false);
        avartar.data.dataSpecified = photonPlayerData.avartar_dataSpecified;
        avartar.data.gender = photonPlayerData.avartar_gender;
        avartar.data.settingString = photonPlayerData.avartar_settingString;
        playerAvartar = avartar;

        if (!isGuest) {
            avartar.SetAvartarData();
            PhotonNetworkManager.instance.SetLocalPlayerAvartarCustomProperties(
                avartar.data.dataSpecified, avartar.data.gender, avartar.data.settingString);
        }
        else {
            avartar.SetAvartarGuest();
            PhotonNetworkManager.instance.SetLocalPlayerAvartarCustomProperties(
                true
                , PlayerPrefsData.Get<int>("GuestAvartarGender", avartar.data.gender)
                , PlayerPrefsData.Get<string>("GuestAvartarData", avartar.data.settingString));
        }
        avartar.AddComponent<PlayerSound>();
        playerCharacter.AvartarControl = avartar;

        // Set Animator
        Animator animator = playerCharacter.AvartarControl.GetComponent<Animator>();

        animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Player/PlayerAnimator"));
        playerCharacter.Animator = animator;
        playerAnimator = animator;

        // Set Data
        photonPlayerData.playerCharacter = playerCharacter;
        playerCharacterList.Add(playerCharacter);

        // Set Local Player CustomCharacter
        this.playerCharacter = playerCharacter;

        // Instantiate Player Camera
        Instantiate(CameraControllerPrefab).TryGetComponent(out CameraController cameraController);
        PlayerCameraController = cameraController;

        // Instantiate Player Input
        playerController = playerCharacter.gameObject.AddComponent<PlayerController>();

        return playerCharacter.gameObject;
    }

    /// <summary>
    /// Other Player Create
    /// </summary>
    public GameObject CreateOtherCharacter(PhotonPlayerData photonPlayerData)
    {
        // Instantiate Player
        Instantiate(PlayerCharacterPrefab
            , new Vector3(0f, 10000f, 0f)
            , Quaternion.identity)
            .TryGetComponent(out PlayerCharacter playerCharacter);
        playerCharacter.gameObject.name = $"[Player]({photonPlayerData.uuid})";

        playerCharacter.Uuid = photonPlayerData.uuid;
        playerCharacter.NickName = photonPlayerData.nickName;
        playerCharacter.IsMine = false;

        // Instantiate Player Avatar
        AvartarControl avartar = Instantiate(Resources.Load<AvartarControl>("Prefabs/Avartar"));
        avartar.transform.SetParent(playerCharacter.transform, false);
        avartar.data.dataSpecified = photonPlayerData.avartar_dataSpecified;
        avartar.data.gender = photonPlayerData.avartar_gender;
        avartar.data.settingString = photonPlayerData.avartar_settingString;
        avartar.SetAvartarOthers();
        avartar.AddComponent<PlayerSound>();
        playerCharacter.AvartarControl = avartar;

        // Set Animator
        Animator animator = avartar.GetComponent<Animator>();
        animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Player/PlayerAnimator"));
        playerCharacter.Animator = animator;

        // Set Data
        photonPlayerData.playerCharacter = playerCharacter;
        photonPlayerData.playerCharacter.AvartarControl = avartar;
        playerCharacterList.Add(playerCharacter);

        return playerCharacter.gameObject;
    }

    /// <summary>
    /// Remove Character
    /// </summary>
    public void RemovePlayerCharacter(Player player)
    {
        // Get PlayerData
        PhotonNetworkManager.instance.photonPlayerDic.TryGetValue(player.NickName, out PhotonPlayerData photonPlayerData);
        if (photonPlayerData != null) {
            if (photonPlayerData.playerCharacter != null) {
                Destroy(photonPlayerData.playerCharacter?.gameObject);
            }
        }
        // Remove
        playerCharacterList.Remove(photonPlayerData.playerCharacter);
        if (player.NickName == Gongju.Web.DatabaseConnector.instance.memberData.nickname) {
            if (photonPlayerData.playerCharacter != null) {
                Destroy(photonPlayerData.playerCharacter?.gameObject);
            }
        }
    }

    /// <summary>
    /// All Remove Character
    /// </summary>
    public void AllRemovePlayerCharacter()
    {
        if (playersCoroutine != null) StopCoroutine(playersCoroutine);

        foreach(var customCharacter in playerCharacterList) {
            if (customCharacter != null) {
                Destroy(customCharacter?.gameObject);
            }
        }
        playerCharacterList.Clear(); 
        if (PlayerCameraController != null) {
            Destroy(PlayerCameraController?.gameObject);
        }
    }
    #endregion

    #region Player Character (Lobby)
    public AvartarControl CreateLobbyPlayerCharacter()
    {
        // Get Default Avartar
        string defaultData = "";
        if (!PlayerPrefsData.HasKey("GuestAvartarData"))
        {
            AvartarBaseData baseData = Resources.Load<AvartarBaseData>("Data/Avartar Base");
            defaultData = JsonUtility.ToJson(new AvartarSettingPack() { data = baseData.baseMale });
        }

        // Instantiate Player Avatar
        AvartarControl avartar = Instantiate(Resources.Load<AvartarControl>("Prefabs/Avartar"));
        avartar.SetPreviewAvartar(isGuest);
        playerAvartar = avartar;

        // Set Animator
        Animator animator = avartar.GetComponent<Animator>();
        animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Player/PlayerAnimator"));
        playerAnimator = animator;

        return avartar;
    }

    #endregion
}