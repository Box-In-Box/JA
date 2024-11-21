using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View_Control : MonoBehaviour
{
    private List<IView> viewList = new List<IView>();

    public void Add(IView component)
    {
        if (!viewList.Contains(component)) viewList.Add(component);
    }

    public void Remove(IView component)
    {
        if (viewList.Contains(component)) viewList.Remove(component);
    }

    public void SetVisible(bool value)
    {
        foreach(var view in viewList) {
            view.SetVisible(value);
        }
    }
}
