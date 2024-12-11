using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractive
{
    public void AddInteractive(IInteractive interactive);
    public void RemoveInteractive(IInteractive interactive);
    public void UndoInteractive();
}