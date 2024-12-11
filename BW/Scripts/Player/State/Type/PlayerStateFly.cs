using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class PlayerStateFly : PlayerStateComponent, IPlayerState, IGravity, IMovable, IJumpable
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

    // Fly
    public float flySpeed => PlayerController.PlayerStatus.FlySpeed;
    public float cameraViewWeight = 1f;

    public void Update()
    {
        Gravity();
        Move();
    }

    public void Move()
    {
        // Movement
        Vector3 movementDirection = new Vector3(PlayerInput.MoveVector.x, 0f, PlayerInput.MoveVector.y);

        CurrentSpeed = PlayerInput.MoveVal * (IsJumping ? flySpeed : MaximumSpeed);
        movementDirection = Quaternion.AngleAxis(CameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;

        Vector3 velocity = new Vector3(movementDirection.x * CurrentSpeed, CurrentGravity, movementDirection.z * CurrentSpeed);
        Controller.Move(velocity * Time.deltaTime);

        if (PlayerInput.IsMoveing) {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
        }

        // Animation
        if (PlayerInput.IsMoveing && !IsJumping) {
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
            if (IsGrounded() && index < (int)PlayerAnimationTypeIndex.Action) {
                Animator.SetInteger("index", (int)PlayerAnimationIndex.idle);
            }
            else if (!IsGrounded() && !IsJumping && index >= (int)PlayerAnimationTypeIndex.Move && index < (int)PlayerAnimationTypeIndex.Motion) {
                Animator.SetInteger("index", (int)PlayerAnimationIndex.fall);
            }
        }
        
        Animator.SetFloat("moveVal", PlayerInput.MoveVal, Animator.GetFloat("moveVal") < MoveAnimation.Walk ? MoveAnimation.ZoroToMoveDamp : MoveAnimation.MoveDamp, Time.deltaTime);
    }

    public void Gravity()
    {
        if (IsGrounded() && !IsJumping) {
            CurrentGravity = GravityVal;
        }
        else if (!IsJumping){
            CurrentGravity += GravityVal * GravityMultiplier * Time.deltaTime;
        }
        else if (IsJumping) {
            // 올라감
            if (Keyboard.current[Key.E].isPressed) {
                CurrentGravity += -GravityVal * GravityMultiplier * Time.deltaTime;
            }
            else if (Keyboard.current[Key.E].wasReleasedThisFrame) {
                CurrentGravity = 0;
            }
            // 내려감
            if (Keyboard.current[Key.Q].isPressed) {
                CurrentGravity += GravityVal * GravityMultiplier * Time.deltaTime;
            }
            else if (Keyboard.current[Key.Q].wasReleasedThisFrame) {
                CurrentGravity = 0;
            }
        }
    }

    public void Jump()
    {
        GameManager.instance.PlayerCameraController.cameraView -= this.cameraViewWeight;
        if (!IsJumping) {
            Animator.SetInteger("index", (int)PlayerAnimationIndex.fly);
            this.cameraViewWeight = 2f;
            CurrentGravity = 0f;
        }
        else {
            Animator.SetInteger("index", (int)PlayerAnimationIndex.idle);
            this.cameraViewWeight = 1f;
        }
        GameManager.instance.PlayerCameraController.cameraView += this.cameraViewWeight;
        IsJumping = !IsJumping;
    }

    public void OperateEnter()
    {
        // === Move ===
        PlayerController.PlayerStatus.MaximumSpeed = 20f;
        PlayerController.PlayerStatus.RotationSpeed = 800f;

        // === Jump ===
        PlayerController.PlayerStatus.JumpMaxCount = 0;
        PlayerController.PlayerStatus. JumpPower = 0f;

        // === Gravity ===
        PlayerController.PlayerStatus.Gravity = -2.0f;
        PlayerController.PlayerStatus.GravityMultiplier = 5f;

        // === Fly === 
        PlayerController.PlayerStatus.FlySpeed = 30f;

        PlayerInput.JumpAction += Jump;
        GameManager.instance.PlayerCameraController.cameraView += this.cameraViewWeight;
    }

    public void OperateExit()
    {
        PlayerInput.JumpAction -= Jump;
        GameManager.instance.PlayerCameraController.cameraView -= this.cameraViewWeight;
    }
}