using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gongju.Web;

public class SystemCall : MonoBehaviour
{
    
    private static SystemCall _instance = null;
    public static SystemCall instance 
    {
        get
        {
            _instance = (SystemCall)FindAnyObjectByType(typeof(SystemCall));

            return _instance;
        }
    }

    [field: SerializeField] public GameObject GameManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject PhotonNetworkManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject PhotonVoiceManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject SceneLoadManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject PopupManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject SoundManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject MissionManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject CommunityManagerPrefab { get; private set; }

    private static List<string> instances = new List<string>();


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        Call<GameManager>();
        Call<PhotonNetworkManager>();
        Call<PhotonVoiceManager>();
        Call<SceneLoadManager>();
        Call<PopupManager>();
        Call<SoundManager>();
        Call<MissionManager>();
        Call<CommunityManager>();

        DatabaseConnector.instance.webDatabase = Resources.Load<WebDatabase>("Data/Web Database");
    }

    public T Call<T>()
    {
        if (instances.Contains(typeof(T).Name)) return default;
        else instances.Add(typeof(T).Name);

        if (typeof(T) == typeof(GameManager))
        {
            Instantiate(GameManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        else if (typeof(T) == typeof(PhotonNetworkManager))
        {
            Instantiate(PhotonNetworkManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        else if (typeof(T) == typeof(PhotonVoiceManager))
        {
            Instantiate(PhotonVoiceManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        else if (typeof(T) == typeof(SceneLoadManager))
        {
            Instantiate(SceneLoadManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        else if (typeof(T) == typeof(PopupManager))
        {
            Instantiate(PopupManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        else if (typeof(T) == typeof(SoundManager))
        {
            Instantiate(SoundManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        else if (typeof(T) == typeof(MissionManager))
        {
            Instantiate(MissionManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        else if (typeof(T) == typeof(CommunityManager))
        {
            Instantiate(CommunityManagerPrefab).TryGetComponent(out T instance);
            return instance;
        }
        return default;
    }
}