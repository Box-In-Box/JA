using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerStateRiding : PlayerStateComponent, IPlayerState, IGravity, IMovable, IJumpable
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
        CurrentSpeed = PlayerInput.MoveVal * MaximumSpeed;
        Vector3 velocity = new Vector3();

        float xSign = Mathf.Sign(PlayerInput.InputVector.x);
        float ySign = Mathf.Sign(PlayerInput.InputVector.y);

        float moveVal = 0f;

        if (PlayerInput.Inputstate == InputState.Keyboard) {
            velocity = transform.forward * PlayerInput.InputVector.y * CurrentSpeed;
        }
        else if (PlayerInput.Inputstate == InputState.Joystick) {
            velocity = transform.forward * (PlayerInput.InputVector.y + xSign * ySign * PlayerInput.InputVector.x) * CurrentSpeed;
        }
        
        velocity.y = CurrentGravity;
        Controller.Move(velocity * Time.deltaTime);

        if (PlayerInput.IsMoveing && PlayerInput.InputVector.y != 0) {
            if (PlayerInput.InputVector.y > 0f) {
                transform.Rotate(0f, PlayerInput.InputVector.x * RotationSpeed * Time.deltaTime, 0f);
                moveVal = PlayerInput.MoveVal;
            }
            else {
                transform.Rotate(0f, -PlayerInput.InputVector.x * RotationSpeed * Time.deltaTime, 0f);
                moveVal = PlayerInput.MoveVal * -1f;
            }
        }
        
        // Animation
        Animator.SetFloat("moveVal", moveVal, Animator.GetFloat("moveVal") < MoveAnimation.Walk ? MoveAnimation.ZoroToMoveDamp : MoveAnimation.MoveDamp, Time.deltaTime);

        float angle = Mathf.InverseLerp(-1f, 1f, PlayerInput.InputVector.x);
        Animator.SetFloat("angleVal", angle, MoveAnimation.MoveDamp, Time.deltaTime);
    }

    public void Jump()
    {
        if (JumpCount >= JumpMaxCount) return;
        
        if (JumpCount == 0) {
            StartCoroutine(WaitForLanding());
            IsJumping = true;
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
        PlayerController.PlayerStatus.MaximumSpeed = 10f;
        PlayerController.PlayerStatus.RotationSpeed = 100f;

        // === Jump ===
        PlayerController.PlayerStatus.JumpMaxCount = 1;
        PlayerController.PlayerStatus. JumpPower = 5f;

        // === Gravity ===
        PlayerController.PlayerStatus.Gravity = -2.0f;
        PlayerController.PlayerStatus.GravityMultiplier = 5f;
        
        PlayerInput.JumpAction += Jump;
        GameManager.instance.PlayerCameraController.cameraView += 1f;
    }

    public void OperateExit()
    {
        PlayerInput.JumpAction -= Jump;
        GameManager.instance.PlayerCameraController.cameraView -= 1f;
    }
}