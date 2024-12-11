using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    public List<NavigatorObserver> observers;

    public void Attach(NavigatorObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(NavigatorObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (NavigatorObserver observer in observers)
        {
            observer.Notify(this);
        }
    }
}
