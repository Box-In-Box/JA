using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_Scene : View_Control
{
    [field : Title("[ HudPanel ]")]
    [field : SerializeField] public View_HudPanel HudPanel { get; set; }

    [field : Title("[ TopPanel ]")]
    [field : SerializeField] public View_TopPanel TopPanel { get; set; }

    [field : Title("[ SidePanel ]")]
    [field : SerializeField] public View_SidePanel SidePanel { get; set; }

    [field : Title("[ BottomPanel ]")]
    [field : SerializeField] public View_BottomPanel BottomPanel { get; set; }

    [field : Title("[ ControlPanel ]")]
    [field : SerializeField] public View_ControlPanel ControlPanel { get; set; }

    [field: Title("[ Test Stage ]")]
    [field: SerializeField] public Test_Stage TestStage { get; set; }
}