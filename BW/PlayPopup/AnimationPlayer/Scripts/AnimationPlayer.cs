using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class AnimationPlayer : MonoBehaviour, IPlayPopup
{
    [field : SerializeField] public string playPopupTitle { get; set; }
    [field : SerializeField] public PlayPopup playPopup { get; set; }
    [field : SerializeField] public ScrollRect scrollRect { get; set; }
    [field : SerializeField] public Button openCloseButton { get; set; }
    [field : SerializeField] public List<Button> buttonList { get; set; } = new List<Button>();
    [field : SerializeField] public bool isRunning { get; set; }

    private int playerAnimationIndex = -1; // 현재 애니메이션 인덱스
    private PlayerAnimationTypeIndex playerAnimationType; // 현재 애니메이션 타입
    private Coroutine waitCoroutine;
    
    private Animator animator;
    private PlayerInputSystem input;

    private bool isFinishAnimation = false;
    private bool isChangeAnimation = false;


    public void Awake()
    {
        foreach (Transform item in scrollRect.content) {
            Button itemButton = item.GetComponent<Button>();
            itemButton.onClick.AddListener(() => playPopup.OpenOrClose(this));
            buttonList.Add(itemButton);

            int index = (int)item.GetComponent<AnimationButton>().playerAnimationindex;
            itemButton.onClick.AddListener(() => AnimationPlay(index));
        }
        openCloseButton.onClick.AddListener(() => playPopup.OpenOrClose(this));
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => animator = GameManager.instance.playerAnimator);
        yield return new WaitUntil(() => input = GameManager.instance.playerController.PlayerInputSystem);
    }

    public void AnimationPlay(int index)
    {
        // 중복 리턴
        if (index == playerAnimationIndex) return;
        playerAnimationIndex = index;
        
        // 애니메이션 변경 확인
        if (isRunning) isChangeAnimation = true;
        
        // 애니메이션 타입 체크
        SetAnimationType(index);

        // 실행
        if (waitCoroutine != null) StopCoroutine(waitCoroutine);
        waitCoroutine = StartCoroutine(AnimationPlayCoroutine(index));
    }

    private void SetAnimationType(int index)
    {
        if (index >= (int)PlayerAnimationTypeIndex.Motion && index < (int)PlayerAnimationTypeIndex.Motion_Once) {
            playerAnimationType = PlayerAnimationTypeIndex.Motion;
        }
        else if (index >= (int)PlayerAnimationTypeIndex.Motion_Once && index < (int)PlayerAnimationTypeIndex.Motion_Fix) {
            playerAnimationType = PlayerAnimationTypeIndex.Motion_Once;
        }
        else if (index >= (int)PlayerAnimationTypeIndex.Motion_Fix && index < (int)PlayerAnimationTypeIndex.Riding) {
            playerAnimationType = PlayerAnimationTypeIndex.Motion_Fix;
        }
    }

    private IEnumerator AnimationPlayCoroutine(int index)
    {
        // 애니메이션 바뀜 예외처리 Wait
        yield return new WaitForEndOfFrame();
        isRunning = true;

        SetAnimation(index);

        // 한번 실행 일 시 애니 기다리는 코루틴 실행
        yield return StartCoroutine(WaitAnimationCoroutine());

        yield return new WaitUntil(() => input.IsMoveing || input.IsJumping || isFinishAnimation || isChangeAnimation);

        if (input.IsMoveing || input.IsJumping || isFinishAnimation) {
            UnSetAnimation();
            Reset();
        }
        else if (isChangeAnimation) {
            isChangeAnimation = false;
        }
    }

    private void SetAnimation(int index)
    {
        this.animator.SetInteger("index", index);

        var weight = DOTween.To(() => this.animator.GetLayerWeight((int)PlayerAnimatorLayer.Motion), 
            x => this.animator.SetLayerWeight((int)PlayerAnimatorLayer.Motion, x), 1f, .5f);
    }

    private void UnSetAnimation()
    {
        var weight = DOTween.To(() => this.animator.GetLayerWeight((int)PlayerAnimatorLayer.Motion), 
            x => this.animator.SetLayerWeight((int)PlayerAnimatorLayer.Motion, x), 0f, .5f);
    }

    // 클립 종료 시 Finish
    private IEnumerator WaitAnimationCoroutine()
    {
        if (playerAnimationType != PlayerAnimationTypeIndex.Motion_Once) yield break;
        yield return new WaitUntil(() => animator.IsInTransition((int)PlayerAnimatorLayer.Motion));
        yield return new WaitUntil(() => !animator.IsInTransition((int)PlayerAnimatorLayer.Motion));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo((int)PlayerAnimatorLayer.Motion).normalizedTime >= 1f);
        isFinishAnimation = true;
    }

    private void Reset()
    {
        playerAnimationIndex = -1;
        playerAnimationType = PlayerAnimationTypeIndex.Move;
        animator.SetInteger("index", (int)PlayerAnimationIndex.idle);
        isRunning = false;
        isFinishAnimation = false;
        isChangeAnimation = false;

        if (waitCoroutine != null) StopCoroutine(waitCoroutine);
    }

    public void Finish()
    {
       Reset();
       UnSetAnimation();
    }
}