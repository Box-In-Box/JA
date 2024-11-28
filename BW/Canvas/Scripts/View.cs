using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class View : MonoBehaviour, IView
{
    public RectTransform rect { get; private set; }
    public CanvasGroup canvasGroup { get; private set; }
    public CanvasGroupOption canvasGroupOption { get; private set; }

    public virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroupOption = new CanvasGroupOption(canvasGroup);

        if (Canvas_Control.instance is Canvas_Scene)
        {
            Canvas_Scene.instance.View.Add(this);
        }
        else if (Canvas_Control.instance is Canvas_Lobby)
        {
            Canvas_Lobby.instance.View.Add(this);
        }
        else if (Canvas_Control.instance is Canvas_Login)
        {
            Canvas_Login.instance.View.Add(this);
        }
    }

    public virtual void OnDestroy()
    {
        if (Canvas_Control.instance is Canvas_Scene)
        {
            Canvas_Scene.instance.View.Remove(this);
        }
        else if (Canvas_Control.instance is Canvas_Lobby)
        {
            Canvas_Lobby.instance.View.Remove(this);
        }
        else if (Canvas_Control.instance is Canvas_Login)
        {
            Canvas_Login.instance.View.Remove(this);
        }
    }

    public void Add(IView component) { }
    public void Remove(IView component) { }

    public void SetVisible(bool value)
    {
        canvasGroup.alpha = value ? canvasGroupOption.alpha : 0f;
        canvasGroup.interactable = value ? canvasGroupOption.interactable : false;
        canvasGroup.blocksRaycasts = value ? canvasGroupOption.blocksRaycasts : false;
    }

    public void SetActive(bool value)
    {
        rect.gameObject.SetActive(value);
    }
}

public class CanvasGroupOption
{
    public float alpha;
    public bool interactable;
    public bool blocksRaycasts;
    public bool ignoreParentGroups;
    
    public CanvasGroupOption(CanvasGroup canvasGroup)
    {
        alpha = canvasGroup.alpha;
        interactable = canvasGroup.interactable;
        blocksRaycasts = canvasGroup.blocksRaycasts;
        ignoreParentGroups = canvasGroup.ignoreParentGroups;
    }
}