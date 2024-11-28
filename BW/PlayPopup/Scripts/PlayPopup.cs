using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayPopup : View
{
    private IPlayPopup iPlayPopup = null;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject[] contnet;
    private bool isScreenShotPause = false;

    [Title("[ Effect Setting ]")]
    [SerializeField, Range(0.1f, 1f)] private float fadeTime = .5f;
    [SerializeField, Range(0.1f, 2f)] private float itemFadeTime = 1f;
    [SerializeField] private Vector2 openPosition;
    [SerializeField, ReadOnly] private Vector2 closePosition;

    public override void Awake()
    {
        base.Awake();
        canvasGroup = this.GetComponent<CanvasGroup>();
        rectTransform = this.GetComponent<RectTransform>();

        closePosition = rectTransform.anchoredPosition;
        closeButton.onClick.AddListener(() => PanelFadeOut());

        foreach(GameObject go in contnet) go.SetActive(false);
        this.gameObject.SetActive(false);
    }

    // IPlayPopup를 가진 스크립트로 부터 호출 -> 하나의 창 컨트롤
    public void OpenOrClose(IPlayPopup _iPlayPopup)
    {
        // 셀프 카메라 일 시 감정표현만 사용 가능
        if (isScreenShotPause && _iPlayPopup.GetType() != typeof(Emotion)) {
            PopupManager.instance.Popup("현재 상태에서 사용할 수 없습니다.");
            return;
        }

        if (!this.gameObject.activeSelf) {
            if (this.iPlayPopup != _iPlayPopup) {
                ChangePanelFade(_iPlayPopup);
                PanelFadeIn();
            }
            else {
                PanelFadeIn();
            }
        }
        else {
            if (this.iPlayPopup != _iPlayPopup) {
                ChangePanelFade(_iPlayPopup);
                StartCoroutine(ItemsAnimation());
            }
            else {
                PanelFadeOut();
            }
        }
    }

    public void Close() => PanelFadeOut();


    private void PanelFadeIn()
    {
        canvasGroup.alpha = 0f;
        this.titleText.text = iPlayPopup.playPopupTitle;
        this.gameObject.SetActive(true);
        rectTransform.DOAnchorPos(openPosition, fadeTime, false).SetEase(Ease.OutQuart);
        canvasGroup.DOFade(1f, fadeTime);
        StartCoroutine(ItemsAnimation());
    }

    private void PanelFadeOut()
    {
        canvasGroup.alpha = 1f;
        rectTransform.DOAnchorPos(closePosition, fadeTime, false).SetEase(Ease.InQuart);
        canvasGroup.DOFade(0f, fadeTime).OnComplete(() => this.gameObject.SetActive(false));
        StopCoroutine(ItemsAnimation());
    }

    private void ChangePanelFade(IPlayPopup iPlayPopup)
    {
        this.iPlayPopup?.Finish();
        this.iPlayPopup?.scrollRect.gameObject.SetActive(false);
        iPlayPopup.scrollRect.gameObject.SetActive(true);
        iPlayPopup.scrollRect.verticalScrollbar.value = 1f;
        
        titleText.DOKill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(this.titleText.DOFade(0f, fadeTime / 2)
        .OnComplete(() => this.titleText.text = iPlayPopup.playPopupTitle))
        .Append(this.titleText.DOFade(1f, fadeTime / 2));

        this.iPlayPopup = iPlayPopup;
    }

    private IEnumerator ItemsAnimation()
    {
        foreach (var item in iPlayPopup.buttonList) {
            item.transform.DOKill();
            item.transform.localScale = Vector3.zero;
        }
        foreach (var item in iPlayPopup.buttonList) {
            item.transform.DOScale(1f, Random.Range(itemFadeTime / 2, itemFadeTime)).SetEase(Ease.OutBounce);
            yield return null;
        }
    }

#region 셀프 스크린샷 예외처리
    public void ScreenShotPause()
    {
        this.iPlayPopup?.Finish();
        this.iPlayPopup?.scrollRect.gameObject.SetActive(false);
        PanelFadeOut();
        this.iPlayPopup = null;
        isScreenShotPause = true;
    }

    public void ScreenShotRelease()
    {
        this.iPlayPopup?.Finish();
        isScreenShotPause = false;
    }
#endregion
}