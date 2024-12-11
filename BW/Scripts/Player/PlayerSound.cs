using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Animator animator;
    private PlayerCharacter character;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        character = GetComponentInParent<PlayerCharacter>();
    }

    void FootStep(AnimationEvent @event)
    {
        if (@event.animatorClipInfo.weight > 0.5f)
        {
            PlaySound("FootStep");
        }
    }

    void Jump(AnimationEvent @event)
    {
        PlaySound("Jump");
    }

    void JumpLand(AnimationEvent @event)
    {

    }

    void DobbleJump(AnimationEvent @event)
    {
        PlaySound("Jump");
    }

    void DobbleJumpLand(AnimationEvent @event)
    {

    }

    void PlaySound(string clipName)
    {
        if (character.IsMine)
        {
            AudioClip clip = SoundManager.instance.SoundClipData.GetPlayerClip(clipName);
            SoundManager.instance.PlaySFX(clip);
        }
    }
}
