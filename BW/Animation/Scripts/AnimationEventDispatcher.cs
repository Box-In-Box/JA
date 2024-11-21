using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Animator))]
public class AnimationEventDispatcher : MonoBehaviour
{
    [Title("[ Reference ]")]
    public Animator animator;
    public List<AnimationClip> AnimationClipList;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        SetAnimationEvent();
    }

    public void SetAnimationEvent()
    {
        AnimationClipList  = new List<AnimationClip>();
        // Get AnimationClips
        foreach (var clip in animator.runtimeAnimatorController.animationClips) {
            bool isAnimationStartHandler = false;
            bool isAnimationCompleteHandler = false;
            
            // For Only One Event
            foreach (var clipEvent in clip.events) {
                if (clipEvent.functionName == "AnimationStartHandler") isAnimationStartHandler = true;
                else if (clipEvent.functionName == "AnimationCompleteHandler") isAnimationCompleteHandler = true;
            }

            // Set Animation Start Event
            if (!isAnimationStartHandler) {
                AnimationEvent animationStartEvent = new AnimationEvent();
                animationStartEvent.time = 0;
                animationStartEvent.functionName = "AnimationStartHandler";
                SetAnimationParameter<string>(ref animationStartEvent, clip.name);
                clip.AddEvent(animationStartEvent);
            }
            
            // Set Animation Complete Event
            if (!isAnimationCompleteHandler) {
                AnimationEvent animationEndEvent = new AnimationEvent();
                animationEndEvent.time = clip.length;
                animationEndEvent.functionName = "AnimationCompleteHandler";
                SetAnimationParameter<string>(ref animationEndEvent, clip.name);
                clip.AddEvent(animationEndEvent);
            }
            AnimationClipList.Add(clip);
        }
    }

    public void SetAnimationEventParameter(string fuctionName)
    {
        // Get AnimationClips
        foreach (var clip in AnimationClipList) {
            
            AnimationEvent animationEvent = new AnimationEvent();

            foreach (var clipEvent in clip.events) {
                if (clipEvent.functionName == fuctionName) {
                    animationEvent = clipEvent;
                    SetAnimationParameter<string>(ref animationEvent, clip.name);
                    clip.AddEvent(animationEvent);
                }
            }
        }
    }

    protected virtual void SetAnimationParameter<T>(ref AnimationEvent animationEvent, T value)
    {
        switch (typeof(T).Name) {
            case nameof(String) :
                animationEvent.stringParameter = Convert.ToString(value);
                break;
            case nameof(Int32) :
                animationEvent.intParameter = Convert.ToInt32(value);
                break;
            case nameof(Single) :
                animationEvent.floatParameter = Convert.ToSingle(value);
                break;  
        }
    }

    protected virtual void AnimationStartHandler(AnimationEvent animationEvent)
    {
        Debug.Log($"{animationEvent.stringParameter} animation start.");
    }

    protected virtual void AnimationCompleteHandler(AnimationEvent animationEvent)
    {
        Debug.Log($"{animationEvent.stringParameter} animation complete.");
    }
}