using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class View_SidePanel : View
{
    [field: Title("[ Left Panel ]")]
    [field: SerializeField] public View_LeftPanel LeftPanel { get; private set; }

    [field: Title("[ Right Panel ]")]
    [field: SerializeField] public View_RightPanel RightPanel { get; private set; }
}