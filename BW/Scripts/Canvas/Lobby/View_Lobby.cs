using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class View_Lobby : View_Control
{
    [field : Title("[ TopPanel ]")]
    [field : SerializeField] public View_TopPanel TopPanel { get; set; }

    [field : Title("[ SidePanel ]")]
    [field : SerializeField] public View_SidePanel_Lobby SidePanel { get; set; }

    [field : Title("[ BottomPanel ]")]
    [field : SerializeField] public View_BottomPanel_Lobby BottomPanel { get; set; }
}