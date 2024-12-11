using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ParticleEffect : MonoBehaviour
{
    private ParticleSystem particle;
    [field: SerializeField] UnityEvent playEvent { get; set; }
    [field: SerializeField] UnityEvent stopEvent { get; set; }

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();

        var main = particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    #region OnPlay
    public void Play()
    {
        particle.Play();
        OnParticleSystemPlay();
    }

    public void AddPlayEvent(UnityAction action)
    {
        if (action == null) return;
        this.playEvent.AddListener(action);
    }

    public void RemovePlayEvent(UnityAction action)
    {
        if (action == null) return;
        this.playEvent.RemoveListener(action);
    }

    private void OnParticleSystemPlay()
    {
        playEvent?.Invoke();
    }
    #endregion

    #region OnStop
    public void Stop()
    {
        particle.Stop();
    }

    public void AddStopEvent(UnityAction action)
    {
        if (action == null) return;
        this.stopEvent.AddListener(action);
    }

    public void RemoveStopEvent(UnityAction action)
    {
        if (action == null) return;
        this.stopEvent.RemoveListener(action);
    }

    private void OnParticleSystemStopped()
    {
        stopEvent?.Invoke();
    }
    #endregion
}