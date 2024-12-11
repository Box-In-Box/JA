using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View_RightPanel : View
{
    [field: Title("[ Riding ]")]
    [field: SerializeField] public Riding Riding { get; private set; }

    [field: Title("[ More ]")]
    [field: SerializeField] public More More { get; private set; }
}