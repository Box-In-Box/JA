using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoutubePlayer;

public interface IVideoHandler
{
    public void Play();

    public void Pause();

    public void Stop();

    public void Visible();

    public void InVisible();
}
