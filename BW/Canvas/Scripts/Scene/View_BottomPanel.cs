using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class View_BottomPanel : View
{
    [field : Title("[ ChatManager ]")]
    [field : SerializeField] public ChatManager chatManager { get; private set; }

    [field: Title("[ ScreenShot ]")]
    [field: SerializeField] public GameObject screenShot { get; private set; }

    [field: Title("[ Emotion ]")]
    [field: SerializeField] public GameObject emotion { get; private set; }

    [field: Title("[ Action ]")]
    [field: SerializeField] public GameObject action { get; private set; }
}
