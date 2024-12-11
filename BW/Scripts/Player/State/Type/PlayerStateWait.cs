using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerStateWait : PlayerStateComponent, IPlayerState, IGravity
{
    [field : Title("[ Gravity ]")]
    [field : SerializeField] public float CurrentGravity { get; set; }
    public float GravityVal => PlayerController.PlayerStatus.Gravity;
    public float GravityMultiplier => PlayerController.PlayerStatus.GravityMultiplier;

    public void Update()
    {
        Gravity();
    }
    
    public void Gravity()
    {
        if (IsGrounded()) {
            CurrentGravity = GravityVal;

            int index = Animator.GetInteger("index");
            if (index < (int)PlayerAnimationTypeIndex.Action) {
                Animator.SetInteger("index", (int)PlayerAnimationIndex.idle);
            }
        }
        else {
            CurrentGravity += GravityVal * GravityMultiplier * Time.deltaTime;
        }

        Vector3 velocity = new Vector3(0f, CurrentGravity, 0f);
        Controller.Move(velocity * Time.deltaTime);
    }

    public void OperateEnter()
    {
        // === Gravity ===
        PlayerController.PlayerStatus.Gravity = -2.0f;
        PlayerController.PlayerStatus.GravityMultiplier = 5f;
    }

    public void OperateExit() { }
}

