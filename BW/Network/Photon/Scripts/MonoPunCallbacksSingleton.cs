using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MonoPunCallbacksSingleton<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    protected MonoPunCallbacksSingleton() { }
    private static T _instance = null;
    private static object lockObject = new object();
    private static bool isQuitting = false;

    public static T instance
    {
        get
        {
            lock (lockObject)
            {
                if (isQuitting) return null;
                // Get instance
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    // Remove namespace, only script name
                    string[] typeName = typeof(T).ToString().Split('.');
                    // Get prefab
                    var typeObject = Resources.Load<T>(typeName[typeName.Length - 1]);
                    // Set instance
                    _instance = typeObject != null ? Instantiate(typeObject) : new GameObject().AddComponent<T>();
                    // Set Object Name
                    _instance.name = typeName[typeName.Length - 1];
                    // Don't Destroy OnLoad
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
    }

    public virtual T Call()
    {
        return instance;
    }

    public virtual void OnDestroy()
    {
        _instance = null;
    }

    public virtual void OnApplicationQuit()
    {
        _instance = null;
        isQuitting = true;
    }
}