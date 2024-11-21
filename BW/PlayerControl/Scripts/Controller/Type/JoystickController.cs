using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class JoystickController : MonoBehaviour, IController
{
    public GameObject joystickBound;
    public GameObject joystick;
    public GameObject jump;

    private UnityEngine.InputSystem.OnScreen.OnScreenStick stick;
    private float stickDistance = 0f;

    public float MoveVal { get; set; }
    public PlayerInputSystem PlayerInputSystem { get; set; }

    public void Setting(PlayerInputSystem playerInputSystem)
    {
        // Set playerInputSystem
        this.PlayerInputSystem = playerInputSystem;
        // Set OnScreenStick
        stick = joystick.GetComponent<UnityEngine.InputSystem.OnScreen.OnScreenStick>();
        // Set Jump Button
        jump.GetComponent<Button>().onClick.AddListener(() => playerInputSystem.JumpAction.Invoke());
        // Set Transform
        this.gameObject.transform.SetParent(Canvas_Scene.instance.view.controlPanel.rect, false);
        // Set JoystickSetting
        JoystickSetting();
    }

    public GameObject GetGameObject()
    {
        return this.gameObject ?? null;
    }

#region JoystickControl
    private void JoystickSetting()
    {
        PlayerInputSystem.controllerList.Add(this);

        PlayerInputSystem.PlayerAction.Player.Movement.started += JoystickUpdate;
        PlayerInputSystem.PlayerAction.Player.Movement.performed += JoystickUpdate;
        PlayerInputSystem.PlayerAction.Player.Movement.canceled += JoystickUpdate;
    }

    private void OnDisable()
    {
        PlayerInputSystem.controllerList.Remove(this);

        PlayerInputSystem.PlayerAction.Player.Movement.started -= JoystickUpdate;
        PlayerInputSystem.PlayerAction.Player.Movement.performed -= JoystickUpdate;
        PlayerInputSystem.PlayerAction.Player.Movement.canceled -= JoystickUpdate;
    }

    public void ShowUI_Move(bool value)
    {
        joystickBound?.SetActive(value);
    }

    public void ShowUI_Jump(bool value)
    {
        jump?.SetActive(value);
    }

    private void JoystickUpdate(InputAction.CallbackContext value)
    {
        if (PlayerInputSystem.Inputstate != InputState.Joystick) return;

        var distance = Vector2.Distance(((RectTransform)joystick.transform).anchoredPosition, Vector2.zero);
        stickDistance = Mathf.InverseLerp(0, stick.movementRange, distance);

        SetMoveVal(value);
    }

    public void SetMoveVal(InputAction.CallbackContext value)
    {   
        if (value.ReadValue<Vector2>() == Vector2.zero) {
            this.MoveVal = 0f;
        }
        else if (stickDistance <= MoveAnimation.Walk) {
            this.MoveVal = MoveAnimation.Walk;
        }
        else if (stickDistance <= MoveAnimation.Run) {
            this.MoveVal = MoveAnimation.Run;
        }
        else if (stickDistance <= MoveAnimation.Sprint) {
            this.MoveVal = MoveAnimation.Sprint;
        }
        PlayerInputSystem.MoveVal = this.MoveVal;
    }
#endregion
}