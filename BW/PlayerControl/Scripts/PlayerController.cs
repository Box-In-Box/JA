using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;

public enum PlayerState
{
    Normal,
    Wait,
    Riding,
    Fly,
    ETC,
}

public enum PlayerMoveState
{
    Idle,
    Move,
}

[Serializable]
public class PlayerStatus
{
    [field : Title("[ Move ]")]
    [field : SerializeField] public float MaximumSpeed { get; set; } // 최대 스피드
    [field : SerializeField] public float RotationSpeed { get; set; } // 회전 스피드

    [field : Title("[ Jump ]")]
    [field : SerializeField] public int JumpMaxCount { get; set; } // 최대 점프 횟수
    [field : SerializeField] public float JumpPower { get; set; } // 점프 파워

    [field : Title("[ Gravity ]")]
    [field : SerializeField] public float Gravity { get; set; } // 중력
    [field : SerializeField] public float GravityMultiplier { get; set; } // 가속도

    [field : Title("[ Fly ]")]
    [field : SerializeField] public float FlySpeed { get; set; } // Fly 스피드
}

public class PlayerController : PlayerInteractive
{
    [field : Title("[ Reference ]")]
    [field : SerializeField] public PlayerInputSystem PlayerInputSystem { get; set; }
    [field : SerializeField] public PlayerStateMachine PlayerStateMachine { get; set; }
    [field: SerializeField] public CharacterController CharacterController { get; set; }

    [field : Title("[ Status & State ]")]
    [field: SerializeField] public PlayerStatus PlayerStatus { get; set; } = new PlayerStatus();
    [field : SerializeField] public PlayerState PlayerState { get; private set; } = PlayerState.Wait;
    public PlayerMoveState PlayerMoveState 
    { 
        get 
        {
            if (PlayerInputSystem.IsMoveing) return PlayerMoveState.Move;
            else return PlayerMoveState.Idle;
        }
    }

    private void Awake()
    {
        if (this.GetComponent<PlayerInputSystem>() == false) {
            PlayerInputSystem = this.AddComponent<PlayerInputSystem>();
        }

        if (this.GetComponent<PlayerStateMachine>() == false) {
            PlayerStateMachine = this.AddComponent<PlayerStateMachine>();
            ChangePlayerState(PlayerState.Normal);
        }

        CharacterController = GetComponent<CharacterController>();
    }

    // 공통 사용 상태
    public void ChangePlayerState(PlayerState state)
    {
        if (state == PlayerState) return;

        UndoInteractive();

        switch (state) {
            case PlayerState.Normal :
                PlayerStateMachine.SetState<PlayerStateNormal>();
                break;
            case PlayerState.Wait :
                PlayerStateMachine.SetState<PlayerStateWait>();
                break;
            case PlayerState.Riding :
                PlayerStateMachine.SetState<PlayerStateRiding>();
                break;
            case PlayerState.Fly :
                PlayerStateMachine.SetState<PlayerStateFly>();
                break;
        }
        PlayerState = state;
    }

    // 개별 씬 상태
    public void ChangePlayerState<T>(T state) where T : IPlayerState
    {
        PlayerStateMachine.SetStates<T>(state);
        PlayerState = PlayerState.ETC;
    }

    /// <summary>
    /// 컨트롤러 UI 표시 여부
    /// </summary>
    /// <param name="move">왼쪽 움직임 UI 출력 여부</param>
    /// <param name="jump">오른쪽 점프 UI 출력 여부</param>
    public void ShowUI(bool move = true, bool jump = true) =>  PlayerInputSystem.ShowUI(move, jump);

#if UNITY_EDITOR
    [ButtonGroup, GUIColor(0f, 1f, 0f)] public void Normal() => ChangePlayerState(PlayerState.Normal);
    [ButtonGroup, GUIColor(0f, 1f, 0f)] public void Wait() => ChangePlayerState(PlayerState.Wait);
    [ButtonGroup, GUIColor(0f, 1f, 0f)] public void Riding() => ChangePlayerState(PlayerState.Riding);
    [ButtonGroup, GUIColor(0f, 1f, 0f)] public void Fly() => ChangePlayerState(PlayerState.Fly);

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) {

            if (Input.GetKeyDown(KeyCode.Alpha1)) Normal();

            if (Input.GetKeyDown(KeyCode.Alpha2)) Wait();

            if (Input.GetKeyDown(KeyCode.Alpha3)) Riding();

            if (Input.GetKeyDown(KeyCode.Alpha4)) Fly();
        }
    }
#endif
}