using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendScrollView : DynamicScrollView<FriendData, Friend>
{
    [field: SerializeField] public override GameObject Prefab { get; set; }
    [field: SerializeField] public override float MinSize { get; set; }
    [field: SerializeField] public override float Space { get; set; }
}