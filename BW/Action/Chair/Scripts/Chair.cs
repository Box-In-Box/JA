using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public HudTarget HudTarget { get; set; }

    private void Awake()
    {
        if (HudTarget == null)
        {
            HudTarget = GetComponent<HudTarget>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            HudTarget.HudUI.IsVisible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            HudTarget.HudUI.IsVisible = false;
        }
    }

    public void Sit()
    {
        PlayerController player = GameManager.instance.playerController;
        CharacterController controller = player?.CharacterController;

        if (player && controller)
        {
            controller.enabled = false;
            player.transform.position = this.transform.position;
            player.transform.forward = this.transform.forward;
            controller.enabled = true;
        }
    }
}