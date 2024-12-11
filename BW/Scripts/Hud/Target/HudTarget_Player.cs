using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudTarget_Player : HudTarget
{
    public override void Awake()
    {
        
    }

    public override void Start()
    {
        Setting();
    }

    public override void Setting()
    {
        TryGetComponent<PlayerCharacter>(out PlayerCharacter player);
        Transform head = player.Animator.GetBoneTransform(HumanBodyBones.Head);
        Target = head.GetChild(0);
        HudUI = CreateHudUI();
    }
}
