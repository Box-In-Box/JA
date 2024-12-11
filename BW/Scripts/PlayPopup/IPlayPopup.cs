using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPlayPopup
{
    public string playPopupTitle { get; set; }
    public PlayPopup playPopup { get; set; }
    public ScrollRect scrollRect { get; set; }
    public Button openCloseButton { get; set; }
    public List<Button> buttonList { get; set; }
    public bool isRunning { get; set; }

    public abstract void Finish();
}