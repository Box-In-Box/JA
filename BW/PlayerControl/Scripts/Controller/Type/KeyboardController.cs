using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class KeyboardController : MonoBehaviour, IController
{
    public GameObject keyboard;
    public GameObject jump;

    public Image keyboardImage;
    public Sprite[] keyboardSprite;

    public float MoveVal { get; set; }
    public PlayerInputSystem PlayerInputSystem { get; set; }

    public void Setting(PlayerInputSystem playerInputSystem)
    {
        // Set playerInputSystem
        this.PlayerInputSystem = playerInputSystem;
        // Set Keyboard Button
        keyboard.GetComponent<Button>().onClick.AddListener(KeyboardSprite);
        // Set Jump Button
        jump.GetComponent<Button>().onClick.AddListener(() => playerInputSystem.JumpAction.Invoke());
        // Set Transform
        this.gameObject.transform.SetParent(Canvas_Scene.instance.view.controlPanel.rect, false);
        // Set KeyboardSetting
        KeyboardSetting();
    }

    public GameObject GetGameObject()
    {
        return this.gameObject ?? null;
    }

    private void KeyboardSprite()
    {
        for (int i = 0; i < keyboardSprite.Length; ++i) {
            if (keyboardImage.sprite == keyboardSprite[i]) {
                if (i == keyboardSprite.Length -1) {
                    keyboardImage.sprite = keyboardSprite[0];
                    break;
                }
                else {
                    keyboardImage.sprite = keyboardSprite[i + 1];
                    break;
                }
            }
        }
        keyboardImage.SetNativeSize();
    }
    
    public void ShowUI_Move(bool value)
    {
        keyboard?.SetActive(value);
    }

    public void ShowUI_Jump(bool value)
    {
        jump?.SetActive(value);
    }

#region KeyboardControl
    private void KeyboardSetting()
    {
        PlayerInputSystem.controllerList.Add(this);

        PlayerInputSystem.PlayerAction.Player.Movement.started += KeyboardUpdate;
        PlayerInputSystem.PlayerAction.Player.Movement.canceled += KeyboardUpdate;

        PlayerInputSystem.PlayerAction.Player.Sprint.started += KeyboardUpdate;
        PlayerInputSystem.PlayerAction.Player.Sprint.canceled += KeyboardUpdate;

        PlayerInputSystem.PlayerAction.Player.Walk.started += KeyboardUpdate;
        PlayerInputSystem.PlayerAction.Player.Walk.canceled += KeyboardUpdate;
    }

    private void OnDisable()
    {
        PlayerInputSystem.controllerList.Remove(this);

        PlayerInputSystem.PlayerAction.Player.Movement.started -= KeyboardUpdate;
        PlayerInputSystem.PlayerAction.Player.Movement.canceled -= KeyboardUpdate;

        PlayerInputSystem.PlayerAction.Player.Sprint.started -= KeyboardUpdate;
        PlayerInputSystem.PlayerAction.Player.Sprint.canceled -= KeyboardUpdate;

        PlayerInputSystem.PlayerAction.Player.Walk.started -= KeyboardUpdate;
        PlayerInputSystem.PlayerAction.Player.Walk.canceled -= KeyboardUpdate;
    }

    private void KeyboardUpdate(InputAction.CallbackContext value)
    {
        if (PlayerInputSystem.Inputstate != InputState.Keyboard) return;

        SetMoveVal(value);
    }

    public void SetMoveVal(InputAction.CallbackContext value)
    {
        if (value.action.name == PlayerInputSystem.PlayerAction.Player.Movement.name) {
            this.MoveVal = value.ReadValue<Vector2>() == Vector2.zero ? 0f : PlayerInputSystem.IsSprinting ? MoveAnimation.Sprint : PlayerInputSystem.IsWalking ? MoveAnimation.Walk : MoveAnimation.Run;
        }
        if (value.action.name == PlayerInputSystem.PlayerAction.Player.Sprint.name) {
            this.MoveVal = PlayerInputSystem.InputVector == Vector2.zero ? 0f : value.started ? MoveAnimation.Sprint : PlayerInputSystem.IsWalking ? MoveAnimation.Walk : MoveAnimation.Run;
        }
        if (value.action.name == PlayerInputSystem.PlayerAction.Player.Walk.name) {
            this.MoveVal = PlayerInputSystem.InputVector == Vector2.zero ? 0f : value.started ? MoveAnimation.Walk : PlayerInputSystem.IsSprinting ? MoveAnimation.Sprint : MoveAnimation.Run;
        }
        PlayerInputSystem.MoveVal = this.MoveVal;
    }
#endregion
}