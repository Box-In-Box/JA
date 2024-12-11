using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_Lobby : Canvas_Control, ICanvasView
{
    private static Canvas_Lobby _instance;
    public static new Canvas_Lobby instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Canvas_Lobby)FindObjectOfType(typeof(Canvas_Lobby));
            }
            return _instance;
        }
    }
    [field : SerializeField] public View_Lobby View { get; set; }

    public override void Awake()
    {
        _instance = this;
    }

    public View_Control GetView_Control()
    {
        return View;
    }
}
