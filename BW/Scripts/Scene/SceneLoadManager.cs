using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Photon.Realtime;
using Photon.Pun;
using Sirenix.OdinInspector;

public enum SceneType
{
    Online,
    Offline,
    Myroom,
    Null,
}

public class SceneLoadManager : MonoBehaviour
{
    private static SceneLoadManager _instance;
    public static SceneLoadManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<SceneLoadManager>();
            }
            return _instance;
        }
    }

    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject LoadingPopupPrefab { get; private set; }
    [field: SerializeField] public GameObject ChannelPopupPrefab { get; private set; }

    [field : SerializeField, Tooltip("이동할 씬 이름")] public string targetScene { get; set; } = "Null";
    [field : SerializeField, Tooltip("이동할 씬 타입")] public SceneType targetSceneType { get; set; } = SceneType.Null;
    [Tooltip("[ 체크 시 디버그 채널 입장 ]\n[ 해제 시 채널 선택 ]")] public bool isStartDebugChannel = true;
    private Coroutine sceneLoadCoroutine;

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

        SetSceneType(SceneManager.GetActiveScene().name);
    }

    private void SetSceneType(string scene)
    {
        targetScene = scene;

        switch (targetScene.ToString().Split('_')[0]) {
            case "Stage":
                targetSceneType = SceneType.Online;
                break;
            case "OffStage":
                targetSceneType = SceneType.Offline;
                break;
            case "MyRoom":
                targetSceneType = SceneType.Myroom;
                break;
            default :
                targetSceneType = SceneType.Null;
                break;
        }
    }

    // 디버그용 씬에서 시작 시 바로 입장
    private void Start() => sceneLoadCoroutine = StartCoroutine(StartStageCoroutine());

    private IEnumerator StartStageCoroutine()
    {
        if (targetSceneType == SceneType.Online || targetSceneType == SceneType.Myroom) {

            if (isStartDebugChannel) {
                // 로딩 팝업
                PopupManager.instance.Open<LoadingPopup>(LoadingPopupPrefab);
                yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);
                yield return new WaitForEndOfFrame();

                // Debug채널 입장
                if (targetSceneType == SceneType.Online) {
                    PhotonNetworkManager.instance.JoinRoom(PhotonNetworkManager.instance.photonChannelData.debugChannel, isVisible : false);
                }
                // 마이룸채널 입장
                else if (targetSceneType == SceneType.Myroom) {
                    PhotonNetworkManager.instance.JoinRoom(PhotonNetworkManager.instance.GetMyRoomChannel());
                } 
                yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);
            }
            else {
                // 채널 선택 취소 제거
                var channelPopup = PopupManager.instance.Open<ChannelPopup>(ChannelPopupPrefab);
                channelPopup.GetComponentInParent<Dimmed>().isClosedWithDimmed = false;
                channelPopup.GetComponentInChildren<Return>().GetComponent<Button>().interactable = false;

                // 채널 선택
                yield return new WaitUntil(() => targetScene == "Lobby" || PhotonNetwork.NetworkClientState == ClientState.Joining);
                PopupManager.instance.Close<ChannelPopup>();
                PopupManager.instance.Open<LoadingPopup>(LoadingPopupPrefab);
                yield return new WaitUntil(() => targetScene == "Lobby" || PhotonNetwork.NetworkClientState == ClientState.Joined);
            }
            // 캐릭터 기다림
            yield return new WaitUntil(() => GameManager.instance.playerCharacter);
            PopupManager.instance.Close<LoadingPopup>();
        }
        else if (targetSceneType == SceneType.Offline)
        {
            PopupManager.instance.Open<LoadingPopup>(LoadingPopupPrefab);
            yield return new WaitUntil(() => GameManager.instance.playerCharacter);
            PopupManager.instance.Close<LoadingPopup>();
        }
        else if (targetScene == "Lobby")
        {
            PopupManager.instance.Open<LoadingPopup>(LoadingPopupPrefab);
            yield return new WaitUntil(() => GameManager.instance.playerAvartar);
            PopupManager.instance.Close<LoadingPopup>();
        }
    }

    public void PopupLoadScene(string scene, string targetSceneName)
    {
        targetSceneName = targetSceneName == "" ? scene : targetSceneName;
        PopupManager.instance.Popup(targetSceneName + "(으)로 이동합니다.", () => LoadScene(scene));
    }

    public void LoadScene(string scene)
    {
        // 취소 시 Redo를 위한 값 저장
        string preTargetScene = targetScene;
        SceneType preTargetSceneType = targetSceneType;
        SetSceneType(scene);

        // 같은 씬 이동 제외
        if (scene == GameManager.instance.currentScene) {
            Debug.LogWarning($"[ {scene} ] 같은 씬으로 이동할 수 없습니다.");
            targetScene = preTargetScene;
            targetSceneType = preTargetSceneType;
            return;
        }

        // 예외 처리 World 이동
        if (scene == "Null") {
            Debug.LogWarning($"[ {scene} ] 씬으로는 이동할 수 없어, [ Stage_Outside ] 씬으로 이동합니다.");
            targetScene = "Stage_Outside";
        }

        if (sceneLoadCoroutine != null) {
            StopCoroutine(sceneLoadCoroutine);
            PopupManager.instance.Close<ChannelPopup>();
            PopupManager.instance.Close<LoadingPopup>();
        }
        sceneLoadCoroutine = StartCoroutine(SceneLoadCoroutine(preTargetScene, preTargetSceneType));  
    }

    private IEnumerator SceneLoadCoroutine(string preTargetScene, SceneType preTargetSceneType)
    {
        if (targetSceneType == SceneType.Myroom) {
            PopupManager.instance.Open<LoadingPopup>(LoadingPopupPrefab);
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joining);
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);
        }
        // < Online Scene >
        else if (targetSceneType == SceneType.Online) {
            var channelPopup = PopupManager.instance.Open<ChannelPopup>(ChannelPopupPrefab);

            // 채널 선택 후 입장 (마이룸 예외 포함)
            if (PhotonNetwork.NetworkClientState != ClientState.Joined || preTargetSceneType == SceneType.Myroom) {
                yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joining || PhotonNetwork.NetworkClientState == ClientState.Leaving || PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer || channelPopup.isCanceled);
                PopupManager.instance.Open<LoadingPopup>(LoadingPopupPrefab);
            }
            PopupManager.instance.Close<ChannelPopup>();
            // 3. Canceled -> Return
            if (channelPopup.isCanceled) {
                targetScene = preTargetScene;
                targetSceneType = preTargetSceneType;
                PopupManager.instance.Close<LoadingPopup>();
                yield break;
            }
        }
        // 씬 변경 시작 세팅
        OnLoadSceneSetting();

        // 씬 변경 팝업
        var sceneProgress = PopupManager.instance.Open<LoadingPopup>(LoadingPopupPrefab).ProgressView;
        sceneProgress.ShowProgress();
        yield return new WaitForEndOfFrame();
        
        // Load 전 Object OnDisable를 위해 제거
        var objects = SceneManager.GetActiveScene().GetRootGameObjects();
        for(var i= 0; i < objects.Length; i++) {
            if (objects[i].tag != "MainCamera") {
                Destroy(objects[i]);
            }
        }

        // 비동기 씬 변경
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Single);
        op.allowSceneActivation = false;
        float timer = 0.0f;

        while (!op.isDone) {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;

            if (op.progress < 0.9f) {
                sceneProgress.progressbar.value = Mathf.Lerp(sceneProgress.progressbar.value, op.progress, timer);
                sceneProgress.progressText.text = (sceneProgress.progressbar.value * 100f).ToString("F1") + " %";
                if (sceneProgress.progressbar.value >= op.progress) {
                    timer = 0f;
                }
            }
            else {
                sceneProgress.progressbar.value = Mathf.Lerp(sceneProgress.progressbar.value, 1f, timer);
                sceneProgress.progressText.text = "100 %";
                if (sceneProgress.progressbar.value == 1.0f) {
                    op.allowSceneActivation = true;
                    break;
                }
            }
        }
        
        // 씬 병경 완료 세팅
        OnLoadSceneCompletedSetting();

        // 플레이어 생성 기다린 후 팝업 제거
        if (targetSceneType != SceneType.Null)
        {
            yield return new WaitUntil(() => GameManager.instance.playerCharacter);
        }
        else if (targetScene == "Login")
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.PeerCreated || PhotonNetwork.NetworkClientState == ClientState.Disconnected);
        }
        else if (targetScene == "Lobby")
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby && GameManager.instance.playerAvartar);
        }
        PopupManager.instance.Close<LoadingPopup>();
    }

    private void OnLoadSceneSetting()
    {
        GameManager.instance.OnLoadScene(targetScene);

        PhotonNetworkManager.instance.PhotonDestroyAll();
        if (PhotonNetworkManager.instance.GetLocalPhotonPlayerData() != null) {
            PhotonNetworkManager.instance.LocalPlayerCustomProperties(targetScene);
            PhotonNetworkManager.instance.GetLocalPhotonPlayerData().sceneName = targetScene;
        }
    }

    private void OnLoadSceneCompletedSetting()
    {
        GameManager.instance.PhotonNetworkConnect();
    }
}