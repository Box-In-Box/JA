using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Canvas_Scene : Canvas_Control, ICanvasView
{
    private static Canvas_Scene _instance;
    public static new Canvas_Scene instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Canvas_Scene)FindObjectOfType(typeof(Canvas_Scene));
            }
            return _instance;
        }
    }
    [field : SerializeField] public View_Scene View { get; set; }

    public override void Awake()
    {
        _instance = this;
        ViewSetting();
    }

    private void ViewSetting()
    {
        if (View == null) { // For offline Scene
            View = this.AddComponent<View_Scene>();

            View.ControlPanel = CreateView<View_ControlPanel>();
            View.ControlPanel.transform.SetSiblingIndex(0);

            View.HudPanel = CreateView<View_HudPanel>();
            View.HudPanel.transform.SetSiblingIndex(0);
        }
    }

    public T CreateView<T>() where T : View
    {
        var controlPanel = CreatePanel(typeof(T).ToString());
        controlPanel.AddComponent<T>();
        return controlPanel.GetComponent<T>();
    }

    public RectTransform CreatePanel(string name)
    {
        GameObject panelObj = new GameObject(name + "(Clone)", typeof(RectTransform));
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelObj.transform.SetParent(this.transform);
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.localScale = Vector3.one;
        return panelRect;
    }

    public View_Control GetView_Control()
    {
        return View;
    }
}