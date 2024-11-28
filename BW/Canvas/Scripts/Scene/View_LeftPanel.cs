using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View_LeftPanel : View
{
    [field: Title("[ Riding ]")]
    [field: SerializeField] public MyRoomBGM MyRoomBGM { get; private set; }

    [field: Title("[ More ]")]
    [field: SerializeField] public MyRoomOut MyRoomOut { get; private set; }
}