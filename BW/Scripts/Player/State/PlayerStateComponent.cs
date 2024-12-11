using UnityEngine;

public class PlayerStateComponent : MonoBehaviour
{
    public Transform Player { get; set; }
    public PlayerController PlayerController { get; set; }
    public CharacterController Controller { get; set; }
    public PlayerInputSystem PlayerInput { get; set; }
    public PlayerCharacter PlayerCharacter { get; set; }
    public Animator Animator { get; set; }
    public Transform CameraTransform { get; set; }
    private Ray ray;

    public virtual void Awake()
    {
        Player = GetComponent<Transform>();
        PlayerController = GetComponent<PlayerController>();
        Controller = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInputSystem>();

        PlayerCharacter = GameManager.instance.playerCharacter;
        Animator = GameManager.instance.playerAnimator;
        CameraTransform = Camera.main.transform;
    }

    public IPlayerState ThisState() { return this.GetComponent<IPlayerState>(); }
    public PlayerStateComponent ThisComponent() { return this; }

    public bool IsGrounded()
    {
        return Controller.isGrounded;
    }

    public bool IsStep()
    {
        ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * (0.1f + GetStepOffset()), Color.red);

        if (Physics.Raycast(ray, 0.1f + GetStepOffset())) return true;
        return false; 
    }

    public float GetStepOffset()
    {
        return Controller.stepOffset + (Controller.center.y * 2) - Controller.height;
    }
}
