using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_Scene : View_Control, IView
{
    [field : Title("[ HudPanel ]")]
    [field : SerializeField] public View_HudPanel hudPanel { get; set; }

    [field : Title("[ TopPanel ]")]
    [field : SerializeField] public View_TopPanel topPanel { get; set; }

    [field : Title("[ SidePanel ]")]
    [field : SerializeField] public View_SidePanel sidePanel { get; set; }

    [field : Title("[ BottomPanel ]")]
    [field : SerializeField] public View_BottomPanel bottomPanel { get; set; }

    [field : Title("[ ControlPanel ]")]
    [field : SerializeField] public View_ControlPanel controlPanel { get; set; }

    [field: Title("[ Test Stage ]")]
    [field: SerializeField] public Test_Stage testStage { get; set; }
}