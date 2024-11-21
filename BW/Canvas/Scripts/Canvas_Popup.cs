using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_Popup : MonoBehaviour
{
    private static Canvas_Popup _instance;
    public static Canvas_Popup instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Canvas_Popup)FindObjectOfType(typeof(Canvas_Popup));
            }
            return _instance;
        }
    }
    
    public void Awake()
    {
        _instance = this;
    }
}