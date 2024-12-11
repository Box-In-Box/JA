using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNPC : MonoBehaviour
{
    [Title("[ References ]")]
    [field: SerializeField] public TalkNPC TalkNPC { get; set; }

    [Title("[ Mission Setting ]")]
    [field: SerializeField] public string missionBeforeTalk { get; set; }
    [field: SerializeField] public string missionTalk { get; set; }
    [field: SerializeField] public string missionAfterTalk { get; set; }

    private void Awake()
    {
        TalkNPC.IsAlwaysTalk = true;
    }
}