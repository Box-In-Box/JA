using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Popup : PopupEffect
{
    public bool isCanceled { get; set; } = false; // Return Or Dimmed로 취소 처리 됨
    [Tooltip("체크 시 배경 딤드처리(배경 클릭 Off)")] public bool isDimmed = true;
    [Tooltip("체크 시 딤드 터치로 닫기(배경 클릭 Off)")] public bool isClosedWithDimmed = true;
    
    public override void Awake()
    {
        base.Awake();
        
        // Setting Dimmed
        if (isDimmed) SettingDimmed();
        
        OpenPopup();
    }

    private void SettingDimmed()
    {
        Dimmed dimmed = Instantiate(Resources.Load<Dimmed>("Dimmed"), this.transform.parent, false);
        this.transform.SetParent(dimmed.transform, false);
        dimmed.name = "Dimmed_" + this.gameObject.name;
        dimmed.isClosedWithDimmed = this.isClosedWithDimmed;
    }
}