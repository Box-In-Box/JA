using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleDynamicScrollViewData
{
    public string text;
}

public class SampleDynamicScrollView : DynamicScrollView<SampleDynamicScrollViewData, SampleDynamicScrollViewItem>
{
    [field: SerializeField] public override GameObject Prefab { get; set; }
    [field: SerializeField] public override float MinSize { get; set; }
    [field: SerializeField] public override float Space { get; set; }
}
