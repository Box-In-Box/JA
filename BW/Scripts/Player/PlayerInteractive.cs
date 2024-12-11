using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerInteractive : MonoBehaviour, IInteractive
{
    private List<IInteractive> interactives = new List<IInteractive>();

    public void AddInteractive(IInteractive interactive)
    {
        if (!interactives.Contains(interactive))
        {
            interactives.Add(interactive);
        }
    }

    public void RemoveInteractive(IInteractive interactive)
    {
        if (interactives.Contains(interactive))
        {
            interactives.Remove(interactive);
        }
    }

    public void UndoInteractive()
    {
        foreach(var interactive in interactives)
        {
            interactive.UndoInteractive();
        }
        interactives.Clear();
    }
}