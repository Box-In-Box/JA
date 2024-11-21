using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class View_Lobby : View_Control, IView
{
    [field : Title("[ TopPanel ]")]
    [field : SerializeField] public View_TopPanel topPanel { get; set; }

    [field : Title("[ SidePanel ]")]
    [field : SerializeField] public View_SidePanel_Lobby sidePanel { get; set; }

    [field : Title("[ BottomPanel ]")]
    [field : SerializeField] public View_BottomPanel_Lobby bottomPanel { get; set; }
}
