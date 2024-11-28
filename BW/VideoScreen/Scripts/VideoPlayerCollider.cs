using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoPlayerCollider : MonoBehaviour
{
    private IVideoHandler handler;

    private void Awake()
    {
        handler = GetComponentInParent<IVideoHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            handler.Visible();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            handler.InVisible();
        }
    }
}