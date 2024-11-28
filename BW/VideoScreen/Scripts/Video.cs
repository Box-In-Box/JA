using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YoutubePlayer;

public class Video : MonoBehaviour
{
    [field: Title("[ References ]")]
    [field: SerializeField, ReadOnly] private IVideoHandler handler { get; set; }
    [field: SerializeField] public RenderTexture RenderTexture { get; set; }
    [field: SerializeField] public RawImage RawImage { get; set; }

    private void Start()
    {
        handler = this.GetComponentInParent<IVideoHandler>();
    }
}
