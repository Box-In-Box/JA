using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class View_SidePanel : View
{
    [field : Title("[ Option ]")]
    [field : SerializeField] public GameObject option { get; private set; }

    [field : Title("[ Mission ]")]
    [field : SerializeField] public GameObject mission { get; private set; }

    [field : Title("[ Teleport ]")]
    [field : SerializeField] public GameObject teleport { get; private set; }

    [field : Title("[ MyRoom ]")]
    [field : SerializeField] public GameObject myRoom { get; private set; }

    [field : Title("[ Riding ]")]
    [field : SerializeField] public GameObject riding { get; private set; }
    
    [field : Title("[ Community ]")]
    [field : SerializeField] public GameObject community { get; private set; }
}