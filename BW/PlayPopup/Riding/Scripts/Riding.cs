using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Riding : MonoBehaviour, IPlayPopup
{
    /// <summary>
    /// Photon Object 컴포넌트 추가 필요
    /// </summary>
    [field : SerializeField] public List<GameObject> ridingPrefabList { get; set; } = new List<GameObject>();

    [field : SerializeField] public string playPopupTitle { get; set; }
    [field : SerializeField] public PlayPopup playPopup { get; set; }
    [field : SerializeField] public ScrollRect scrollRect { get; set; }
    [field : SerializeField] public Button openCloseButton { get; set; }
    [field : SerializeField] public List<Button> buttonList { get; set; } = new List<Button>();
    [field : SerializeField] public bool isRunning { get; set; }

    private int ridingIndex = -1; // 현재 라이딩 인덱스
    private GameObject ridingObject = null; // 현재 라이딩 오브젝트
    private Coroutine ridingUpdateCoroutine;

    private Animator animator;
    private Animator ridingAnimator;

    public void Awake()
    {
        foreach (Transform item in scrollRect.content) {
            Button itemButton = item.GetComponent<Button>();
            itemButton.onClick.AddListener(() => playPopup.OpenOrClose(this));
            buttonList.Add(itemButton);

            int index = (int)item.GetComponent<AnimationButton>().playerAnimationindex - (int)PlayerAnimationTypeIndex.Riding;
            buttonList.Last().onClick.AddListener(() => Ride(index));
        }
        openCloseButton.onClick.AddListener(() => playPopup.OpenOrClose(this));
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => animator = GameManager.instance.playerAnimator);
    }

    public void Ride(int index)
    {
        // 라이딩 시작
        if (ridingObject == null) {
            // Set State => Riding State
            GameManager.instance.playerController.ChangePlayerState(PlayerState.Riding);
            SetRidingAnimation(index); // Set Animation
            SetRidingObject(index); // Set Riding Object
            
            if (ridingUpdateCoroutine != null) StopCoroutine(ridingUpdateCoroutine); 
            ridingUpdateCoroutine = StartCoroutine(RidingUpdate()); // Update Riding Animation
            isRunning = true;
        }
        // 다른 라이딩 (제거 후 새 라이딩)
        else if (ridingObject != null && index != ridingIndex) {
            UnSetRidingAnimation(); // UnSet Animation
            UnSetRidingObject(); // UnSet Riding Object

            SetRidingAnimation(index); // Set Animation
            SetRidingObject(index); // Set Riding Object
        }
        // 같은 라이딩 (제거)
        else if (ridingObject != null && index == ridingIndex) {
            Finish();
        }
    }
    
#region Riding 타기
    // 해당 라이딩 애니메이션 설정
    private void SetRidingAnimation(int index)
    {
        var weight = DOTween.To(() => this.animator.GetLayerWeight((int)PlayerAnimatorLayer.Riding), 
            x => this.animator.SetLayerWeight((int)PlayerAnimatorLayer.Riding, x), 1f, .2f);
        this.animator.SetInteger("index", (int)PlayerAnimationTypeIndex.Riding + index);
    }

    // 해당 라이딩 오브젝트 생성
    private void SetRidingObject(int index)
    {
        if (SceneLoadManager.instance.targetSceneType == SceneType.Online) {
            ridingObject = PhotonNetworkManager.instance.PhotonInstantiate(ridingPrefabList[index].name);
        }
        else if (SceneLoadManager.instance.targetSceneType == SceneType.Offline) {
            ridingObject = Instantiate(ridingPrefabList[index], GameManager.instance.playerCharacter.transform);
        }
        ridingAnimator = ridingObject.GetComponent<Animator>();
        ridingIndex = index;
    }
#endregion

#region Riding 해제
    // 라이딩 애니메이션 해제
    private void UnSetRidingAnimation()
    {
        var weight = DOTween.To(() => this.animator.GetLayerWeight((int)PlayerAnimatorLayer.Riding), 
            x => this.animator.SetLayerWeight((int)PlayerAnimatorLayer.Riding, x), 0f, .2f);
        this.animator.SetInteger("index", (int)PlayerAnimationTypeIndex.Move);
    }

    // 라이딩 오브젝트 해제
    private void UnSetRidingObject()
    {
        if (SceneLoadManager.instance.targetSceneType == SceneType.Online) {
            PhotonNetworkManager.instance.PhotonDestroy(ridingObject);
        }
        else if (SceneLoadManager.instance.targetSceneType == SceneType.Offline) {
            Destroy(ridingObject);
        }
        ridingIndex = -1;
        ridingAnimator = null;
        ridingObject = null;
    }
#endregion

    private IEnumerator RidingUpdate()
    {
        while (true) {
            yield return new WaitForEndOfFrame();
            if (this.isRunning == false || this.ridingAnimator == null) yield break;
            ridingAnimator.SetFloat("angleVal", animator.GetFloat("angleVal"));
        }
    }

    public void Finish()
    {
        GameManager.instance.playerController.ChangePlayerState(PlayerState.Normal);
        UnSetRidingAnimation();
        UnSetRidingObject();
        
        if (ridingUpdateCoroutine != null) StopCoroutine(ridingUpdateCoroutine);
        isRunning = false;
    }
}