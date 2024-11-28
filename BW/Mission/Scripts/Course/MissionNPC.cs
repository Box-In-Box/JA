using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNPC : MonoBehaviour
{
    [Title("[ References ]")]
    [field: SerializeField] public TalkNPC TalkNPC { get; set; }

    [Title("[ Mission Setting ]")]
    [field: SerializeField] private string missionTalk;

    private void Awake()
    {
        TalkNPC.IsAlwaysTalk = true;
        TalkNPC.AlwaysTalkData = missionTalk;
    }
}