using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace YoutubePlayer
{
    public class YoutubeVideo : YoutubePlayer
    {
        [field: Title("[ References ]")]
        [field: SerializeField, ReadOnly] private IVideoHandler handler { get; set; }

        [field: Title("[ Video ]")]
        [field: SerializeField] public RawImage RawImage { get; set; }
        [field: SerializeField, ReadOnly] public RenderTexture RenderTexture { get; set; }

        [field: Title("[ Loading ]")]
        [field: SerializeField] public GameObject Loading { get; set; }
        

        private void Start()
        {
            handler = this.GetComponentInParent<IVideoHandler>();
            RenderTexture = InstantiateTextrue();
            RawImage.texture = RenderTexture;

            VideoPlayer.targetTexture = RenderTexture;
        }

        public RenderTexture InstantiateTextrue()
        {
            var renderTexture = new RenderTexture(1920, 1080, 24);
            renderTexture.name = "Video_Render_Texture";
            renderTexture.Create();
            return renderTexture;
        }
    }
}