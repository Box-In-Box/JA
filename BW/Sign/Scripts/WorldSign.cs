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
    [field: SerializeField, ReadOnly] public WorldSignView view { get; set; }
    [field: SerializeField] public HudTarget HudTarget { get; set; }
    [field: SerializeField] public Sprite signSprite { get; set; }
    [field: SerializeField] public string signName { get; set; }
    [field: SerializeField] public string sceneName { get; set; }

    private void Start()
    {
        view.button.onClick.AddListener(SignPopup);
    }

    private void SignPopup()
    {
        var popup = PopupManager.instance.Open<SignPopup>();
        var popupView = popup.GetComponent<SignPopupView>();
        Debug.Log(popupView);
        popupView.TitleText.text = signName;

        if (sceneName == "")
        {
            popupView.SetEnterButton(false);
        }
        else
        {
            popupView.SetEnterButton(true, () => SceneLoadManager.instance.LoadScene(sceneName));
        }
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