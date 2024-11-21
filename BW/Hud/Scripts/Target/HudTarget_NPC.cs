using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudTarget_NPC : HudTarget
{
    public override void Awake()
    {
        Setting();
    }

    public override void Start()
    {
        
    }

    public override void Setting()
    {
        if (Target == null)
        {
            Animator animator = GetComponent<Animator>();
            if (animator.isHuman)
            {
                Target = animator.GetBoneTransform(HumanBodyBones.Head).GetChild(0);
            }
            else
            {
                Transform[] childs = this.transform.GetComponentsInChildren<Transform>();
                foreach (Transform child in childs)
                {
                    if (child.name == "Bip001 HeadNub")
                    {
                        Target = child;
                        break;
                    }
                }
            }
            if (Target == null) Target = this.transform;
        }
        HudUI = GetHudUI();
    }
}
