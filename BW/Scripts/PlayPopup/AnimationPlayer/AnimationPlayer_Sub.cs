using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPlayer_Sub : MonoBehaviour
{
    [field : SerializeField] public Button actionButton { get; set; }

    private Button remoteButton;

    private void Awake()
    {
        remoteButton = GetComponent<Button>();

        remoteButton.onClick.AddListener(() => actionButton.onClick.Invoke());
    }
}