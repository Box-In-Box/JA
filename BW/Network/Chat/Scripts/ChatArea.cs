using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatArea : MonoBehaviour
{
    public RectTransform areaRect, boxRect, msgRect;
    public TMP_Text timeText, userText, dateText;
    public Image userImage;
    public string time, user;
}
