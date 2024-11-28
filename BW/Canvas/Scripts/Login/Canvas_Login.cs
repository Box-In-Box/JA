using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_Login : Canvas_Control, ICanvasView
{
    private static Canvas_Login _instance;
    public static new Canvas_Login instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Canvas_Login)FindObjectOfType(typeof(Canvas_Login));
            }
            return _instance;
        }
    }
    [field : SerializeField] public View_Login View { get; set; }

    public override void Awake()
    {
        _instance = this;
    }

    public View_Control GetView_Control()
    {
        return View;
    }
}