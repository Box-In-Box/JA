using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomMusicView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text MusicNameText { get; set; }
    [field: SerializeField] public Button PlayButton { get; set; }
}