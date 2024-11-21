using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

public class PopupManager : MonoSingleton<PopupManager>
{
    [SerializeField, ReadOnly] private SerializedDictionary<string, Popup> popupDictionary = new SerializedDictionary<string, Popup>();

    /// <summary>
    /// 알림 팝업 (액션 후 팝업 닫힘)
    /// </summary> 
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
        NoticePopup popup = Open<NoticePopup>();

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

    /// <summary>
    /// ex) PopupManager.instance.Open<Popup>();
    /// 팝업 클래스로 호출 시
    /// </summary>
    public T Open<T>() where T : Popup
    {
        string key = typeof(T).ToString();

        if (popupDictionary.TryGetValue(key, out Popup popupUI)) return (T)popupUI;

        popupUI = Instantiate(Resources.Load<Popup>(key), Canvas_Popup.instance.transform, false);

        popupDictionary.Add(key, popupUI);

        popupUI.OpenPopup();

        return (T)popupUI;
    }

    /// <summary>
    /// ex) PopupManager.instance.Open(Popup);
    /// 팝업 오브젝트로 호출 시
    /// </summary>
    public T Open<T>(T popup) where T : Popup
    {
        if (popup == null) return null;

        string key = popup.GetType().ToString();

        if (popupDictionary.TryGetValue(key, out Popup value)) return (T)value;

        value = Instantiate(Resources.Load<Popup>(key), Canvas_Popup.instance.transform, false);

        popupDictionary.Add(key, value);

        return (T)value;
    }

    /// <summary>
    /// ex) PopupManager.instance.Close<Popup>();
    /// 팝업 클래스로 호출 시
    /// </summary>
    public void Close<T>(bool immediately = false) where T : Popup
    {
        string key = typeof(T).ToString();

        if (!popupDictionary.TryGetValue(key, out Popup popupUI)) return;
        
        popupUI.ClosePopup(immediately : immediately);
        
        popupDictionary.Remove(key);
    }

    /// <summary>
    /// ex) PopupManager.instance.Close(Popup);
    /// 팝업 오브젝트로 호출 시
    /// </summary>
    public void Close<T>(T popup, bool immediately = false) where T : Popup
    {
        string key = popup.GetType().ToString();

        if (!popupDictionary.TryGetValue(key, out Popup popupUI)) return;
        
        popupUI.ClosePopup(immediately : immediately);
        
        popupDictionary.Remove(key);
    }
}