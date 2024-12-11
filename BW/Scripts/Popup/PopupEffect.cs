using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum PopupEffectType
{
    // None
    None,

    // Scale
    Scale, // 기본 확장 팝업

    // Move
    UpToDown, // 위에서 아래로 내려오는 팝업
    DownToUp,
    LeftToRight,
    RightToLeft,
}

public class PopupEffect : View
{
    [Tooltip("이펙트 타입")] public PopupEffectType popupEffectType;
    [Tooltip("체크 시 Close 후 오브젝트 비활성화 (체크 해제 시 Close 후 오브젝트 삭제)")] public bool isRemained = false; // PopupManager 호출 말고 캔버스 할당 시
    private Vector2 originalAnchor;

    public override void Awake()
    {
        base.Awake();
        originalAnchor = rect.anchoredPosition;
    }
    
    /// <summary>
    /// UI Open Popup Animation
    /// </summary>
    public virtual void OpenPopup(Action openCompleteAction = null)
    {
        this.gameObject.SetActive(true);
        Effect(popupEffectType, true,  openCompleteAction);
    }
    
    /// <summary>
    /// UI Close Popup Animation
    /// </summary>
    public virtual void ClosePopup(Action closeCompleteAction = null, bool immediately = false)
    {
        Action closeCompleteActionAfter = closeCompleteAction;

        if (isRemained) { // 오브젝트 남겨놓음
            closeCompleteActionAfter += () => this.gameObject.SetActive(false);
        }
        else { // 오브젝트 제거
            Dimmed dimmed = this.GetComponentInParent<Dimmed>();
            closeCompleteActionAfter += () => Destroy(dimmed? dimmed.gameObject : this.gameObject); 
        }

        if (immediately) { // 즉각 제거
            Effect(PopupEffectType.None, false, closeCompleteActionAfter);
        }
        else { // 이펙트 후 제거
            Effect(popupEffectType, false, closeCompleteActionAfter);
        }
        
    }

    private void Effect(PopupEffectType popupEffectType, bool isOpen = true, Action openCompleteAction = null)
    {
        switch (popupEffectType) {
            // Move
            case >= PopupEffectType.UpToDown :
                if (isOpen) { OpenMoveAnimation(popupEffectType, Ease.OutBounce, openCompleteAction); }
                else { CloseMoveAnimation(popupEffectType, Ease.OutQuad, openCompleteAction); }
                break;
            // Scale
            case >= PopupEffectType.Scale :
                if (isOpen) { OpenScaleAnimation(Ease.OutBack, openCompleteAction); }
                else { CloseScaleAnimation(Ease.InBack, openCompleteAction); }
                break;
            // None
            default :
                if (isOpen) { Open(openCompleteAction); }
                else { Close(openCompleteAction); }
                break;
        }
    }

#region  None
    // None Open
    private void Open(Action openCompleteAction = null)
    {
        openCompleteAction?.Invoke();
    }

    // None Close
    private void Close(Action closeCompleteAction = null)
    {
        closeCompleteAction?.Invoke();
    }
#endregion

#region  Scale
    // Scale Open
    private void OpenScaleAnimation(Ease ease, Action openCompleteAction = null)
    {
        DOTween.Kill(rect);
        rect.localScale = new Vector3(.8f, .8f, .8f);
        rect.DOScale(1f, .5f).SetEase(ease)
            .OnComplete(() => { openCompleteAction?.Invoke(); });
    }

    // Scale Close
    private void CloseScaleAnimation(Ease ease, Action closeCompleteAction = null)
    {
        DOTween.Kill(rect);
        rect.DOScale(0f, .5f).SetEase(ease).OnComplete(() => { 
            rect.localScale = new Vector3(.8f, .8f, .8f);
            closeCompleteAction?.Invoke();
            });
    }
#endregion

#region Move
    // Move Open
    private void OpenMoveAnimation(PopupEffectType direction, Ease ease, Action openCompleteAction = null)
    {
        DOTween.Kill(rect);
        DOTween.Kill(canvasGroup);
        Sequence sequence = DOTween.Sequence();
        canvasGroup.alpha = 0f;
        
        switch (direction) {
            case PopupEffectType.UpToDown :
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 200f);
                sequence.Append(rect.DOAnchorPosY(originalAnchor.y, .5f)).SetEase(ease);
                break;
            case PopupEffectType.DownToUp :
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 200f);
                sequence.Append(rect.DOAnchorPosY(originalAnchor.y, .5f)).SetEase(ease);
                break;
            case PopupEffectType.LeftToRight :
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x - 200f, rect.anchoredPosition.y);
                sequence.Append(rect.DOAnchorPosX(originalAnchor.x, .5f)).SetEase(ease);
                break;
            case PopupEffectType.RightToLeft :
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + 200f, rect.anchoredPosition.y);
                sequence.Append(rect.DOAnchorPosX(originalAnchor.x, .5f)).SetEase(ease);
                break;
        }
        sequence.Join(canvasGroup.DOFade(1f, .5f));
        sequence.OnComplete(() => { openCompleteAction?.Invoke(); });
    }

    // Move Close
    private void CloseMoveAnimation(PopupEffectType direction, Ease ease, Action closeCompleteAction = null)
    {
        DOTween.Kill(rect);
        DOTween.Kill(canvasGroup);
        Sequence sequence = DOTween.Sequence();
    
        switch (direction) {
            case PopupEffectType.UpToDown :
                sequence.Append(rect.DOAnchorPosY(originalAnchor.y + 200f, .5f)).SetEase(ease);
                break;
            case PopupEffectType.DownToUp :
                sequence.Append(rect.DOAnchorPosY(originalAnchor.y - 200f, .5f)).SetEase(ease);
                break;
            case PopupEffectType.LeftToRight :
                sequence.Append(rect.DOAnchorPosX(originalAnchor.x -200f, .5f)).SetEase(ease);
                break;
            case PopupEffectType.RightToLeft :
                sequence.Append(rect.DOAnchorPosX(originalAnchor.x + 200f, .5f)).SetEase(ease);
                break;
        }
        sequence.Join(canvasGroup.DOFade(0f, .5f));
        sequence.OnComplete(() => { 
            closeCompleteAction?.Invoke();
        });
    }
#endregion
}