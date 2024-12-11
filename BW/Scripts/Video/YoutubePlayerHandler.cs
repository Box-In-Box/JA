using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
        [field: SerializeField, Tooltip("체크 시 팝업 이펙트")] public bool IsPopupEffect { get; set; } = true;
        [field: SerializeField, Tooltip("True: 트리거 Pasue, False: 트리거 Stop")] public bool IsTriggerPause { get; set; } = false;

        [SerializeField, ReadOnly] private bool isPrepared;

        private bool isRestart;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

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
            if (!isPrepared && !tokenSource.IsCancellationRequested)
            {
                tokenSource?.Cancel();
                tokenSource?.Dispose();
            }
            tokenSource = new CancellationTokenSource();
            PrepareYoutubeVideo(URL, tokenSource.Token).Forget();
        }

        public async UniTaskVoid PrepareYoutubeVideo(string url, CancellationToken ct, Action callback = null)
        {
            if (string.IsNullOrEmpty(url)) return;

            isPrepared = false;

            await YoutubeVideo.PrepareVideoAsync(url, cancellationToken: ct);

            isPrepared = true;

            callback?.Invoke();
        }

        private async UniTaskVoid PlayAsync()
        {
            await UniTask.WaitUntil(() => isPrepared);

            YoutubeVideo.Loading.SetActive(false);
            YoutubeVideo.VideoPlayer.Play();
        }

        public void Play()
        {
            PlayAsync().Forget();
        }

        public void Play(string url)
        {
            YoutubeVideo.Loading.SetActive(true);

            Stop();

            if (!isPrepared && !tokenSource.IsCancellationRequested)
            {
                tokenSource?.Cancel();
                tokenSource?.Dispose();
            }
            tokenSource = new CancellationTokenSource();

            YoutubeVideo.StopAllCoroutines();
            YoutubeVideo.CancelInvoke();
            PrepareYoutubeVideo(url, tokenSource.Token, () => Play()).Forget();
        }

        public void PauseOrStop()
        {
            if (IsTriggerPause) Pause();
            else Stop();
        }

        public void Pause()
        {
            YoutubeVideo.VideoPlayer.Pause();
            YoutubeVideo.VideoPlayer.targetTexture.Release();
        }

        public void Stop()
        {
            YoutubeVideo.VideoPlayer.Stop();
            YoutubeVideo.VideoPlayer.targetTexture.Release();
        }
        
        /// <summary>
        /// 트리거 Enter
        /// </summary>
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
                Play();
            }
            
        }

        /// <summary>
        /// 트리거 Exit
        /// </summary>
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
    }
}