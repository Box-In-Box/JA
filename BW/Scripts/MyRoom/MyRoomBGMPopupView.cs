using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomBGMPopupView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text CurrentMusicText { get; set; }

    [field: Title("[ Buttons ]")]
    [field: SerializeField] public Button PlayAndPauaeButton { get; set; }
    [field: SerializeField] public Button PreviousButton { get; set; }
    [field: SerializeField] public Button NextButton { get; set; }

    [field: Title("[ PlayAndPauae ]")]
    [SerializeField] private Image playAndPauaeImage;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;
    private bool isPlaySprite;

    private void Awake()
    {
        PlayAndPauaeButton.onClick.AddListener(OnPlayAndPauae);
    }

    [Button]
    public void OnPlayAndPauae()
    {
        playAndPauaeImage.sprite = isPlaySprite ? playSprite : pauseSprite;
        playAndPauaeImage.SetNativeSize();
        isPlaySprite = !isPlaySprite;
    }
}
