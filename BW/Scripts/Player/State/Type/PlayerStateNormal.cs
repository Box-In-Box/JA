using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerStateNormal : PlayerStateComponent, IPlayerState, IGravity, IMovable, IJumpable
{
    [field : Title("[ Movement ]")]
    [field : SerializeField] public float CurrentSpeed { get; set; }
    public float MaximumSpeed => PlayerController.PlayerStatus.MaximumSpeed;
    public float RotationSpeed => PlayerController.PlayerStatus.RotationSpeed;

    [field : Title("[ Jump ]")]
    [field : SerializeField] public int JumpCount { get; set; }
    [field : SerializeField] public bool IsJumping { get; set; }
    public int JumpMaxCount => PlayerController.PlayerStatus.JumpMaxCount;
    public float JumpPower => PlayerController.PlayerStatus.JumpPower;
    

    [field : Title("[ Gravity ]")]
    [field : SerializeField] public float CurrentGravity { get; set; }
    public float GravityVal => PlayerController.PlayerStatus.Gravity;
    public float GravityMultiplier => PlayerController.PlayerStatus.GravityMultiplier;

    public void Update()
    {
        Gravity();
        Move();
    }
    
    public void Gravity()
    {
        if (IsGrounded() && !IsJumping) {
            CurrentGravity = GravityVal;
        }
        else {
            CurrentGravity += GravityVal * GravityMultiplier * Time.deltaTime;
        }
    }
    
    public void Move()
    {
        // Movement
        Vector3 movementDirection = new Vector3(PlayerInput.MoveVector.x, 0f, PlayerInput.MoveVector.y);

        CurrentSpeed = PlayerInput.MoveVal * MaximumSpeed;
        movementDirection = Quaternion.AngleAxis(CameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;

        Vector3 velocity = new Vector3(movementDirection.x * CurrentSpeed, CurrentGravity, movementDirection.z * CurrentSpeed);
        Controller.Move(velocity * Time.deltaTime);

        if (PlayerInput.IsMoveing) {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
        }
        
        // Animation
        if (PlayerInput.IsMoveing) {
            if (IsGrounded()) {
                Animator.SetInteger("index", (int)PlayerAnimationIndex.move);
            }
            else if (!IsJumping){
                if (!IsStep()) {
                    Animator.SetInteger("index", (int)PlayerAnimationIndex.fall);
                } 
            }
        }
        else { // 현재 움직임이 애니메이션이 활성화 일 때
            int index = Animator.GetInteger("index");
            if (IsGrounded() && index < (int)PlayerAnimationTypeIndex.Motion) {
                Animator.SetInteger("index", (int)PlayerAnimationIndex.idle);
            }
            else if (!IsGrounded() && !IsJumping && index >= (int)PlayerAnimationTypeIndex.Move && index < (int)PlayerAnimationTypeIndex.Motion) {
                Animator.SetInteger("index", (int)PlayerAnimationIndex.fall);
            }
        }
        
        Animator.SetFloat("moveVal", PlayerInput.MoveVal, Animator.GetFloat("moveVal") < MoveAnimation.Walk ? MoveAnimation.ZoroToMoveDamp : MoveAnimation.MoveDamp, Time.deltaTime);
    }

    public void Jump()
    {
        if (JumpCount >= JumpMaxCount) return;
        
        if (JumpCount == 0) {
            StartCoroutine(WaitForLanding());
            Animator.SetInteger("index", (int)PlayerAnimationIndex.jump);
            IsJumping = true;
        }
        else {
            Animator.SetInteger("index", (int)PlayerAnimationIndex.jump_More);
        }
        ++JumpCount;
        CurrentGravity = JumpPower;
    }

    private IEnumerator WaitForLanding()
    {
        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(IsGrounded);

        JumpCount = 0;
        IsJumping = false;
    }

    public void OperateEnter()
    {
        // === Move ===
        PlayerController.PlayerStatus.MaximumSpeed = 4f;
        PlayerController.PlayerStatus.RotationSpeed = 800f;

        // === Jump ===
        PlayerController.PlayerStatus.JumpMaxCount = 2;
        PlayerController.PlayerStatus. JumpPower = 5f;

        // === Gravity ===
        PlayerController.PlayerStatus.Gravity = -2.0f;
        PlayerController.PlayerStatus.GravityMultiplier = 5f;

        PlayerInput.JumpAction += Jump;
    }

    public void OperateExit()
    {
        PlayerInput.JumpAction -= Jump;
    }
}