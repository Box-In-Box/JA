using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ScreenShotView : MonoBehaviour
{
    public RectTransform screenshotFrame;
    public RectTransform interactionPanel;
    public RectTransform photoFrame;

    public Button returnButton;
    public Image blink;

    [Title("[ Frame ]")]
    public TMP_Text zoomText;
    public TMP_Text resolutionText;
    public TMP_Text colorText;
    public TMP_Text timerText;
    public Button clickButton;
    public Toggle swapToggle;
    public Toggle animationSpeedToggle;

    [Title("[ Photo ]")]
    public Image photoImage;
    public Button saveButton;
    public Button wasteButton;
    public Button snsButton;
    public TMP_Text dateText;
    public TMP_Text pathText;
    public TMP_Text labelText;
}
