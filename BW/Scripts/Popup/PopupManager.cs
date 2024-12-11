using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

[Serializable]
public class Prefab_Common
{
    [field: SerializeField] public GameObject Dimmed { get; private set; }
    [field: SerializeField] public GameObject NoticePopup { get; private set; }
    [field: SerializeField] public GameObject InputFieldPopup { get; private set; }
    [field: SerializeField] public GameObject QuizPopup { get; private set; }
}

[Serializable]
public class Prefab_Reward
{
    [field: SerializeField] public GameObject RewardPopup { get; set; }
    [field: SerializeField] public GameObject Coin { get; set; }
    [field: SerializeField] public GameObject Exp { get; set; }
    [field: SerializeField] public GameObject Coin_Ex { get; set; }
    [field: SerializeField] public GameObject Exp_Ex { get; set; }
}

[Serializable]
public class Prefab_HudUI
{
    [field: SerializeField] public GameObject Player { get; set; }
    [field: SerializeField] public GameObject NPC { get; set; }
    [field: SerializeField] public GameObject Sign { get; set; }
    [field: SerializeField] public GameObject Chair { get; set; }
    [field: SerializeField] public GameObject Minigame { get; set; }
}

public class PopupManager : MonoBehaviour
{
    private static PopupManager _instance;
    public static PopupManager instance
    { 
        get 
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<PopupManager>();
            }
            return _instance; 
        } 
    }

    [field: Title("[ Canvas ]")]
    [field: SerializeField] public Canvas Canvas_Popup { get; private set; }

    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public Prefab_Common CommonPrefab { get; private set; }
    [field: SerializeField] public Prefab_Reward RewardPrefab { get; private set; }
    [field: SerializeField] public Prefab_HudUI HudUIPrefab { get; private set; }

    [field: Title("[ Popup Dic ]")]
    [SerializeField, ReadOnly] private SerializedDictionary<string, Popup> popupDictionary = new SerializedDictionary<string, Popup>();

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

    #region Popup
    public T Open<T>(GameObject popupPrefab) where T : Popup
    {
        if (popupPrefab == null) return null;

        string key = popupPrefab.name.ToString();

        if (popupDictionary.TryGetValue(key, out Popup value)) return (T)value;

        Instantiate(popupPrefab, Canvas_Popup.transform, false).TryGetComponent<Popup>(out value);

        popupDictionary.Add(key, value);

        return (T)value;
    }

    public void Close<T>(T popup, bool immediately = false) where T : Popup
    {
        string key = popup.GetType().Name;

        if (!popupDictionary.TryGetValue(key, out Popup value)) return;

        popupDictionary.Remove(key);

        value.ClosePopup(immediately: immediately);
    }

    public void Close<T>(bool immediately = false) where T : Popup
    {
        string key = typeof(T).ToString();

        if (!popupDictionary.TryGetValue(key, out Popup popupUI)) return;
        
        popupUI.ClosePopup(immediately : immediately);
        
        popupDictionary.Remove(key);
    }

    public T Get<T>() where T : Popup
    {
        if (popupDictionary.TryGetValue(typeof(T).ToString(), out Popup value)) return (T)value;
        return null;
    }
    #endregion

    #region NoticePopup
    public NoticePopup Popup(string msg)
    {
        return Popup(msg, null, null, "", "");
    }
    public NoticePopup Popup(string msg, Action okAction)
    {
        return Popup(msg, okAction, null, "", "");
    }
    public NoticePopup Popup(string msg, Action okAction, string okText)
    {
        return Popup(msg, okAction, null, okText, "");
    }
    public NoticePopup Popup(string msg, Action okAction, Action cancelAction)
    {
        return Popup(msg, okAction, cancelAction, "", "");
    }
    public NoticePopup Popup(string msg, Action okAction, Action cancelAction, string okText, string cancelText)
    {
        NoticePopup popup = Open<NoticePopup>(CommonPrefab.NoticePopup);

        if (okAction == null) popup.RemoveCancelButton();
        else popup.ShowCancelButton();

        okAction += () => Close(popup);
        cancelAction += () => Close(popup);

        popup.Setting(msg, okAction, cancelAction, okText, cancelText);
        popup.OpenPopup(); // 팝업 재실행 이펙트 
        return popup;
    }

    public void PopupClose()
    {
        Close<NoticePopup>();
    }
    #endregion

    #region InputPopup
    public InputFieldPopup InputPopup(string msg)
    {
        return InputPopup(msg, null, null, "", "");
    }
    public InputFieldPopup InputPopup(string msg, Action<string> okAction)
    {
        return InputPopup(msg, okAction, null, "", "");
    }
    public InputFieldPopup InputPopup(string msg, Action<string> okAction, string okText)
    {
        return InputPopup(msg, okAction, null, okText, "");
    }
    public InputFieldPopup InputPopup(string msg, Action<string> okAction, Action cancelAction)
    {
        return InputPopup(msg, okAction, cancelAction, "", "");
    }
    public InputFieldPopup InputPopup(string msg, Action<string> okAction, Action cancelAction, string okText, string cancelText)
    {
        InputFieldPopup popup = Open<InputFieldPopup>(CommonPrefab.InputFieldPopup);

        okAction += (value) => InputPopupClose(value);
        cancelAction += () => Close(popup);

        popup.Setting(msg, okAction, cancelAction, okText, cancelText);
        popup.OpenPopup(); // 팝업 재실행 이펙트 
        return popup;
    }

    public void InputPopupClose(string value)
    {
        Close<InputFieldPopup>();
    }
    #endregion
}