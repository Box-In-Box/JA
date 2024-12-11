using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class NavigatorObserver : MonoBehaviour
{
    [Serializable]
    public class ImageSprite
    {
        public Sprite onSprite;
        public Sprite offSprite;
    }

    public abstract Navigator Navigator { get; set; }
    public abstract Button Button { get; set; }

    public abstract Image Image { get; set; }
    public abstract ImageSprite Sprite { get; set; }

    public abstract void OnClick();
    public abstract void Notify(Navigator navigator);
}