using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class YoutubeURLButton : MonoBehaviour
{
    [field: Title("[ Youtube Player ]")]
    [field: SerializeField] public YoutubePlayer.YoutubePlayerHandler YoutubePlayerHandler { get; set; }

    [field: Title("[ Youtube URL ]")]
    [field: SerializeField] public string URL { get; set; }

    public void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => PlayURL());
    }

    public void PlayURL()
    {
        if (URL == "") return;

        YoutubePlayerHandler.Play(URL);
    }
}
