using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TalkCollider : MonoBehaviour, ITalkReceiver
{
    public TalkNPC TalkNPC { get; set; }
    private SphereCollider sphereCollider;

    private void Awake()
    {
        sphereCollider = this.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
    }

    public void Setting(TalkNPC talkNPC)
    {
        TalkNPC = talkNPC;
        sphereCollider.radius = talkNPC.TalkColliderRadius;
        sphereCollider.center = new Vector3(0f, talkNPC.TalkColliderRadius / 2, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            TalkReceive(TalkNPC.TalkData);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            TalkNPC.HudTarget_NPC.HudUI.IsVisible = false;
        }
    }

    public void TalkReceive(string msg)
    {
        TalkNPC.TalkReceiver.TalkReceive(msg);
    }
}
