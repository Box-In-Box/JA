using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_Control : MonoBehaviour
{
    private static Canvas_Control _instance;
    public static Canvas_Control instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Canvas_Control)FindObjectOfType(typeof(Canvas_Control));
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        _instance = this;
    }
}
