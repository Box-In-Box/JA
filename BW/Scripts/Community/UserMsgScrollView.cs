using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMsgScrollView : DynamicScrollView<UserMsgData, UserMsg>
{
    [field: SerializeField] public override GameObject Prefab { get; set; }
    [field: SerializeField] public override float MinSize { get; set; }
    [field: SerializeField] public override float Space { get; set; }
}