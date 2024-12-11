using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIToggleOnOff_Sprite : MonoBehaviour
{
    private Toggle toggle;
    
    [Title("[ On Sprite ]")]
    [SerializeField] private Image on_Image;
    [SerializeField] private Sprite on_On_Sprite;
    [SerializeField] private Sprite on_Off_Sprite;

    [Title("[ Off Sprite ]")]
    [SerializeField] private Image off_Image;
    [SerializeField] private Sprite off_On_Sprite;
    [SerializeField] private Sprite off_Off_Sprite;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((value) => ON_OFF_GameObject(value));
    }

    private void ON_OFF_GameObject(bool value)
    {
        on_Image.sprite = value ? on_On_Sprite : on_Off_Sprite;

        off_Image.sprite = value ? off_Off_Sprite : off_On_Sprite;
    }
}