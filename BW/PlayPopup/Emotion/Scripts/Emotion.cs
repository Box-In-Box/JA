using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emotion : MonoBehaviour, IPlayPopup
{
    [field : SerializeField] public string playPopupTitle { get; set; }
    [field : SerializeField] public PlayPopup playPopup { get; set; }
    [field : SerializeField] public ScrollRect scrollRect { get; set; }
    [field : SerializeField] public Button openCloseButton { get; set; }
    [field : SerializeField] public List<Button> buttonList { get; set; } = new List<Button>();
    [field : SerializeField] public bool isRunning { get; set; }
    
    public void Awake()
    {
        foreach (Transform item in scrollRect.content) {
            Button itemButton = item.GetComponent<Button>();
            itemButton.onClick.AddListener(() => playPopup.OpenOrClose(this));
            buttonList.Add(itemButton);
        }
        openCloseButton.onClick.AddListener(() => playPopup.OpenOrClose(this));
    }

    public void Finish()
    {
        
    }
}