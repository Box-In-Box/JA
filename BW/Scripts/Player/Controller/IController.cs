using UnityEngine;
using UnityEngine.InputSystem;

public interface IController
{
    public float MoveVal { get; set; }
    public PlayerInputSystem PlayerInputSystem { get; set; }

    public void Setting(PlayerInputSystem playerInputSystem);
    public void SetMoveVal(InputAction.CallbackContext value);
    public void ShowUI_Move(bool value);
    public void ShowUI_Jump(bool value);
    public GameObject GetGameObject();
}