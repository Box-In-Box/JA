using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : MonoBehaviour
{
    private Button worldButton;

    private void Awake()
    {
        worldButton = GetComponent<Button>();
        worldButton.onClick.AddListener(() => SceneLoadManager.instance.LoadScene("Stage_Outside"));
    }
}