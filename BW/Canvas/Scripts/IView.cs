using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IView
{
    public void SetVisible(bool value);
    public void Add(IView component);
    public void Remove(IView component);
}
