using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.InputSystem.XR;

public enum InputState
{
    Keyboard,
    Joystick,
    Touch,
}

public class PlayerInputSystem : MonoBehaviour
{
    [field: Title("[ Input Action ]")]
    public PlayerAction PlayerAction { get; private set; }

    [field: Title("[ Control Type ]")]
    [field: SerializeField] public InputState Inputstate { get; private set; }
    public List<IController> controllerList = new List<IController>();

    [field: Title("[ Movement ]")]
    [field: SerializeField] public Vector2 InputVector { get; private set; } // 입력 값
    [field: SerializeField] public Vector2 MoveVector { get; private set; } // 움직임 방향
    [field: SerializeField] public float MoveVal { get; set; } // 움직임 강도

    [field: Title("[ Value (Bool) ]")]
    [field: SerializeField] public bool IsMoveing { get; private set; }
    [field: SerializeField] public bool IsWalking { get; private set; }
    [field: SerializeField] public bool IsSprinting { get; private set; }
    [field: SerializeField] public bool IsJumping { get; private set; }

    // === Action ===
    public Action JumpAction { get; set; }

    private void Awake()
    {
        PlayerAction = new PlayerAction();
        SwitchInputSystem();
        PlayerControlSetting();
    }

    private void OnEnable()
    {
        PlayerAction.Player.Enable();
    }

    private void OnDisable()
    {
        PlayerAction.Player.Disable();
    }

    private void OnDestroy()
    {
        for (int i = controllerList.Count - 1; i >= 0; --i)
        {
            Destroy(controllerList[i].GetGameObject());
        }
    }

    private void SwitchInputSystem()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            // Windows, Mac Editor
            var keyboard = SetInputState(InputState.Keyboard);
            keyboard.ShowUI_Move(false);
            keyboard.ShowUI_Jump(false);
            SetInputState(InputState.Joystick);
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            // Windows, MacOS
            SetInputState(InputState.Keyboard);
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Android, iOS
            SetInputState(InputState.Joystick);
        }
    }

    private IController SetInputState(InputState inputState)
    {
        IController controller = null;
        switch (inputState)
        {
            case InputState.Keyboard:
                controller = Instantiate(Resources.Load<KeyboardController>("KeyboardController"));
                break;
            case InputState.Joystick:
                controller = Instantiate(Resources.Load<JoystickController>("JoystickController"));
                break;
            case InputState.Touch:
                break;
        }
        controller.Setting(this);
        return controller;
    }

    public void ShowUI(bool moveUI, bool jumpUI)
    {
        foreach (var controller in controllerList)
        {
            controller.ShowUI_Move(moveUI);
            controller.ShowUI_Jump(jumpUI);
        }
    }

    #region PlayerControl
    private void PlayerControlSetting()
    {
        // Device
        PlayerAction.Player.Movement.started += SetInputState;
        // Movement
        PlayerAction.Player.Movement.started += OnMovement;
        PlayerAction.Player.Movement.performed += OnMovement;
        PlayerAction.Player.Movement.canceled += OnMovement;
        PlayerAction.Player.Movement.started += val => IsMoveing = BW.Tool.IsInputFieldFocused() ? false : true;
        PlayerAction.Player.Movement.canceled += val => IsMoveing = false;

        // Jump
        PlayerAction.Player.Jump.started += val => OnControl(() => JumpAction?.Invoke());
        PlayerAction.Player.Jump.started += val => IsJumping = true;
        PlayerAction.Player.Jump.canceled += val => IsJumping = false;

        // Sprint
        PlayerAction.Player.Sprint.started += val => IsSprinting = true;
        PlayerAction.Player.Sprint.canceled += val => IsSprinting = false;

        // Wlak
        PlayerAction.Player.Walk.started += val => IsWalking = true;
        PlayerAction.Player.Walk.canceled += val => IsWalking = false;
    }

    private void SetInputState(InputAction.CallbackContext value)
    {
        if (value.control.device.name == "Keyboard")
        {
            this.Inputstate = InputState.Keyboard;
        }
        else if (value.control.device.name == "Gamepad")
        {
            this.Inputstate = InputState.Joystick;
        }
    }

    private void OnMovement(InputAction.CallbackContext value)
    {
        if (BW.Tool.IsInputFieldFocused()) return;

        InputVector = value.ReadValue<Vector2>();
        MoveVector = new Vector2(InputVector.x, InputVector.y).normalized;
    }

    private void OnControl(Action action = null)
    {
        if (BW.Tool.IsInputFieldFocused()) return;

        action?.Invoke();
    }
    #endregion
}