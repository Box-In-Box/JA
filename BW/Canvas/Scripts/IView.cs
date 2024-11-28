using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IView
{
    public void Add(IView view);
    public void Remove(IView view);
    public void SetVisible(bool value);
    public void SetActive(bool value);
}