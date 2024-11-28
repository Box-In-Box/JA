using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View_Control : MonoBehaviour, IView
{
    private List<IView> viewList = new List<IView>();

    public void Add(IView view)
    {
        if (!viewList.Contains(view)) viewList.Add(view);
    }

    public void Remove(IView view)
    {
        if (viewList.Contains(view)) viewList.Remove(view);
    }

    public void SetVisible(bool value)
    {
        foreach(var view in viewList)
        {
            view.SetVisible(value);
        }
    }

    public void SetActive(bool value)
    {
        foreach (var view in viewList)
        {
            view.SetActive(value);
        }
    }
}