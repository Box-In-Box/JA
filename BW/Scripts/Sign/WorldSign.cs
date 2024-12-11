using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using Photon.Pun.Demo.Procedural;

public class WorldSign : MonoBehaviour
{
    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject SignPopupPrefab { get; private set; }

    [field: Title("[ Setting ]")]
    [field: SerializeField, ReadOnly] public WorldSignView View { get; set; }
    [field: SerializeField] public HudTarget HudTarget { get; set; }
    [field: SerializeField] public Sprite SignSprite { get; set; }
    [field: SerializeField] public string SceneName { get; set; }
    [field: SerializeField] public string SignName { get; set; }
    [field: SerializeField] public string SignNameSub { get; set; }
    [field: SerializeField] public string SignDescription { get; set; }

    [field: Title("[ URL ]")]
    [field: SerializeField] public string YoutubeUrl { get; set; }
    [field: SerializeField] public string HomepageUrl { get; set; }
    [field: SerializeField] public string MapUrl { get; set; }

    public void SignPopup()
    {
        var signPopup = PopupManager.instance.Open<SignPopup>(SignPopupPrefab);
        var popupView = signPopup.GetComponent<SignPopupView>();
        popupView.TitleText.text = SignName;
        popupView.ChatText.text = SignDescription;

        popupView.SetEnterButton(SceneName);
        popupView.SetYoutubeURL(YoutubeUrl);
        popupView.SetHomepageURL(HomepageUrl);
        popupView.SetMapURL(MapUrl);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            HudTarget.HudUI.IsVisible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);
        
        if (player == GameManager.instance.playerCharacter)
        {
            HudTarget.HudUI.IsVisible = false;
        }
    }
}