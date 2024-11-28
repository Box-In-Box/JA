using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace YoutubePlayer
{
    public class YoutubePlayerHandler : MonoBehaviour, IVideoHandler
    {
        [field: Title("[ Youtube URL ]")]
        [field: SerializeField] public string URL { get; set; }

        [field: Title("[ References ]")]
        [field: SerializeField] public Transform Content { get; set; }

        [field: Title("[ Youtube Video ]")]
        [field: SerializeField] public YoutubeVideo YoutubeVideo {  get; set; }

        [field: Title("[ Obejct ]")]
        [field: SerializeField] public Collider ObjectCollider { get; set; }

        [field: Title("[ Setting ]")]
        [field: SerializeField] public bool IsPopupEffect { get; set; } = true;
        [field: SerializeField] public bool IsTriggerPause { get; set; } = false;

        [SerializeField, ReadOnly] private bool isPrepared;
        private Coroutine playCoroutine;

        private bool isRestart;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        private void Awake()
        {
            if (IsPopupEffect)
            {
                Content.localScale = Vector3.zero;
                if (ObjectCollider != null) ObjectCollider.enabled = false;
            } 
        }

        private void Start()
        {
            PrepareYoutubeVideo(URL).Forget();
        }

        public async UniTaskVoid PrepareYoutubeVideo(string url, Action callback = null)
        {
            if (string.IsNullOrEmpty(url)) return;


            if (cancelTokenSource != null && !cancelTokenSource.Token.IsCancellationRequested)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
            }
            cancelTokenSource = new CancellationTokenSource();

            isPrepared = false;

            try
            {
                await YoutubeVideo.PrepareVideoAsync(url, cancellationToken: cancelTokenSource.Token);
                isPrepared = true;
                callback?.Invoke();
            }
            catch (Exception e)
            {
                //Debug.LogWarning(e.Message);
            }
        }

        public void Play()
        {
            if (playCoroutine != null) StopCoroutine(playCoroutine);
            PlayAsync().Forget();
        }

        public void Play(string url)
        {
            YoutubeVideo.Loading.SetActive(true);
            Stop();
            PrepareYoutubeVideo(url, () => Play());
        }

        public void Pause()
        {
            YoutubeVideo.VideoPlayer.Pause();
            YoutubeVideo.VideoPlayer.targetTexture.Release();
            if (playCoroutine != null) StopCoroutine(playCoroutine);
        }

        public void Stop()
        {
            YoutubeVideo.VideoPlayer.Stop();
            YoutubeVideo.VideoPlayer.targetTexture.Release();
            if (playCoroutine != null) StopCoroutine(playCoroutine);
        }

        public void PauseOrStop()
        {
            if (IsTriggerPause)
            {
                Pause();
            }
            else
            {
                Stop();
            }
        }

        public void Visible()
        {
            if (IsPopupEffect)
            {
                if (ObjectCollider != null) ObjectCollider.enabled = true;
                Content.DOKill();
                Content.localScale = Vector3.zero;
                Content.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).OnComplete(() => Play());
            }
            else
            {
                PlayAsync().Forget();
            }
            
        }

        public void InVisible()
        {
            if (IsPopupEffect)
            {
                if (ObjectCollider != null) ObjectCollider.enabled = false;
                Content.DOKill();
                Content.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() => PauseOrStop());
            }
            else
            {
                PauseOrStop();
            }
        }

        private async UniTaskVoid PlayAsync()
        {
            await UniTask.WaitUntil(() => isPrepared);

            YoutubeVideo.Loading.SetActive(false);
            YoutubeVideo.VideoPlayer.Play();
        }
    }
}