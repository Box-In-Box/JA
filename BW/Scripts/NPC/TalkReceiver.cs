using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkReceiver : ChatBubble, ITalkReceiver
{
    public TalkNPC TalkNPC { get; set; }

    public void Start()
    {
        if (HudUi.HudTarget.TryGetComponent<TalkNPC>(out TalkNPC talkNPC))
        {
            TalkNPC = talkNPC;
        }
    }

    public void TalkReceive(string msg)
    {
        if (TalkNPC.IsAlwaysTalk)
        {
            ChatAlways(msg);
        }
        else
        {
            Chat(msg);
        }
    }   
}
