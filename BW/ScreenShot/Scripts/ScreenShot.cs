using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class ScreenShot : View
{
    [SerializeField, ReadOnly] private string folder = "PlayScreenShot";
    [SerializeField, ReadOnly] private string file = "PlayPark";
    [SerializeField] private PlayPopup playPopup;

    [Tooltip("체크 시 결과 이미지(photoImage) 비율대로 찍기")] public bool isRatioScreen = true;
    [SerializeField] private ScreenShotView screenShotView;
    [SerializeField] private Button screenShotButton;
    private Texture2D photoTexture;
    public bool isScreenShot;
    private Tweener swapTweener;

    public override void Awake()
    {
        base.Awake();
        screenShotButton.onClick.AddListener(() => SettingScreenShot());

        screenShotView.returnButton.onClick.AddListener(() => SettingScreenShot());
        screenShotView.clickButton.onClick.AddListener(() => Capture());
        screenShotView.swapToggle.onValueChanged.AddListener((value) => Swap(value));
        screenShotView.animationSpeedToggle.onValueChanged.AddListener((value) => AnimationToggle(value));

        screenShotView.saveButton.onClick.AddListener(() => Save());
        screenShotView.wasteButton.onClick.AddListener(() => Waste());
        screenShotView.snsButton.onClick.AddListener(() => SNS());
    }

    private IEnumerator AddEvent()
    {
        yield return new WaitUntil(() => GameManager.instance.PlayerCameraController != null);

        if (GameManager.instance.PlayerCameraController.playerCamera != null) {
            GameManager.instance.PlayerCameraController.playerCamera.zoomAction += SetZoomText;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(AddEvent());
    }

    private void OnDisable()
    {
        // 씬이 바뀔 시 카메라도 삭제 되므로 필요없음
        //GameManager.instance.PlayerCameraController.playerCamera.zoomAction -= SetZoomText;
    }

    private Vector2Int RatioResolution()
    {
        if (isRatioScreen) {
            RectTransform photoImageRect = screenShotView.photoImage.GetComponent<RectTransform>();
            
            int screenshotWidth = Screen.width;
            int screenshotHeight = Screen.height;

            if (Screen.width > Screen.height) {
                float aspectRatio = photoImageRect.rect.height / photoImageRect.rect.width;
                screenshotWidth = Mathf.RoundToInt(screenshotHeight / aspectRatio);
            }
            else {
                float aspectRatio = photoImageRect.rect.width / photoImageRect.rect.height;
                screenshotHeight = Mathf.RoundToInt(screenshotWidth / aspectRatio);
            }  
            return new Vector2Int (screenshotWidth, screenshotHeight);
        }
        else {
            return new Vector2Int (Screen.width, Screen.height);
        }
    }

    private void SettingScreenShot()
    {
        bool value = screenShotView.screenshotFrame.gameObject.activeSelf ? false : true;

        screenShotView.resolutionText.text = RatioResolution().x.ToString() + "x" + RatioResolution().y.ToString();
        screenShotView.screenshotFrame.gameObject.SetActive(value);
        screenShotView.interactionPanel.gameObject.SetActive(value);
        screenShotView.animationSpeedToggle.isOn = true;
        screenShotView.swapToggle.isOn = false;
        Canvas_Scene.instance.View.SetVisible(!value);
        playPopup.Close();
    }

    public void SetZoomText()
    {
        if (GameManager.instance != null && GameManager.instance.PlayerCameraController.playerCamera != null) {
            var cam = GameManager.instance.PlayerCameraController.playerCamera;
            var zoomValue = Mathf.InverseLerp(cam.zoomMaxPercent, cam.zoomMinPercent, cam.targetZoomValue) + 1;
            screenShotView.zoomText.text = 'x' + string.Format("{0:0.#}", zoomValue);
        }
    }

#region 스크린샷 중
    public void Capture()
    {
        if (!isScreenShot) {
            StartCoroutine(CaptureCoroutine());
        }
    }

    private IEnumerator CaptureCoroutine()
    {
        isScreenShot = true;

        // 사진 찍기
        Shot();
        yield return new WaitForEndOfFrame();

        // 블링크
        yield return Shutter();
        
        // 저장 여부
        Photo();
        yield return new WaitForEndOfFrame();

        isScreenShot = false;
    }

    private void Shot()
    {
        // Set Camera Render
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        RenderTexture.active = renderTexture;
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();
        
        if (isRatioScreen) {
            // SetRatio

            Vector2Int ratioResolution = RatioResolution();
            int startX = (Screen.width - ratioResolution.x) / 2;
            int startY = (Screen.height - ratioResolution.y) / 2;

            // Get Textrue
            photoTexture = new Texture2D(ratioResolution.x, ratioResolution.y, TextureFormat.RGB24, false);
            photoTexture.ReadPixels(new Rect(startX, startY, ratioResolution.x, ratioResolution.y), 0, 0);
            photoTexture.Apply();
        }
        else {
            // Get Textrue
            photoTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            photoTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            photoTexture.Apply();
        }
        // Reset
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
    }

    private IEnumerator Shutter()
    {
        //Sound("shutterSound");

        screenShotView.blink.color = new Color(screenShotView.blink.color.r, screenShotView.blink.color.g, screenShotView.blink.color.b, 0f);
        screenShotView.blink.gameObject.SetActive(true);
        var tween = screenShotView.blink.DOFade(1f, .2f).SetLoops(2, LoopType.Yoyo);

        yield return tween.WaitForCompletion();
        screenShotView.blink.gameObject.SetActive(false);
    }

    private void Photo()
    {
        screenShotView.photoImage.sprite = Sprite.Create(photoTexture, new Rect(0, 0, photoTexture.width, photoTexture.height), new Vector2(0.5f, 0.5f));
#if UNITY_STANDALONE || UNITY_EDITOR
        screenShotView.pathText.text = $"바탕화면/{folder} 폴더에 저장됩니다.";
#elif UNITY_ANDROID
        screenShotView.pathText.text = $"갤러리/{folder} 폴더에 저장됩니다.";
#elif UNITY_IOS
        screenShotView.pathText.text = $"앨범/{folder} 폴더에 저장됩니다.";
#endif
        screenShotView.dateText.text = DateTime.Now.ToString();
        screenShotView.labelText.text = RatioResolution().x.ToString() + "x" + RatioResolution().y.ToString();
        screenShotView.photoFrame.gameObject.SetActive(true);
    }

    private void Swap(bool value)
    {
        screenShotView.animationSpeedToggle.isOn = true;
        
        Transform target = GameManager.instance.playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        Transform player = GameManager.instance.playerCharacter.transform;
        CinemachineFreeLook freeLook = GameManager.instance.PlayerCameraController.playerCamera.freeLook;

        if (value) {
            playPopup.ScreenShotPause(); // 액션, 라이딩 등 해제
            GameManager.instance.playerController.ChangePlayerState(PlayerState.Wait);
            GameManager.instance.playerAnimator.SetInteger("index", (int)PlayerAnimationIndex.selfCamera);
            GameManager.instance.PlayerCameraController.ChangeView(0f, 0f);
            GameManager.instance.PlayerCameraController.ChangeFOV(100f);
            GameManager.instance.PlayerCameraController.SetControl(false);
            freeLook.m_XAxis.Value = player.eulerAngles.y - 180f;
            swapTweener = DOTween.To(() => .5f, x => freeLook.m_YAxis.Value = x, .8f, 1f);
        }
        else {
            playPopup.ScreenShotRelease();
            GameManager.instance.playerController.ChangePlayerState(PlayerState.Normal);
            GameManager.instance.playerAnimator.SetInteger("index", (int)PlayerAnimationIndex.idle);
            GameManager.instance.PlayerCameraController.ChangeViewReturn(10f);
            GameManager.instance.PlayerCameraController.ChangeFOVReturn();
            GameManager.instance.PlayerCameraController.SetControl();
            GameManager.instance.PlayerCameraController.SetCameraView();
            DOTween.Kill(swapTweener);
        }
    }

    private void AnimationToggle(bool value)
    {
        if (value) { // Animation Play
            GameManager.instance.playerAnimator.speed = 1f;
        }
        else { // Animation Pause
            GameManager.instance.playerAnimator.speed = 0f;
        }
    }
#endregion

#region 스크린샷 이후
    private void Save()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), folder) + '/';
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        var data = photoTexture.EncodeToPNG();
        File.WriteAllBytes(path + GetFileName(), data);

#elif UNITY_ANDROID || UNITY_IOS
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);

        if (permission == NativeGallery.Permission.Denied) {
            if (NativeGallery.CanOpenSettings()) {
                NativeGallery.OpenSettings();
            }
        } 

        NativeGallery.SaveImageToGallery(photoTexture, folder, GetFileName(), (success, path) => {
            Debug.Log(success);
            Debug.Log(path);
        });
#endif
        Waste();
    }

    private void Waste()
    {
        Destroy(photoTexture);
        photoTexture = null;
        screenShotView.photoFrame.gameObject.SetActive(false);
    }

    private void SNS()
    {
        Debug.Log("SNS share test");
        NativeShare shared = new NativeShare()
            .AddFile(photoTexture, GetFileName());
        shared.Share();
    }

    private string GetFileName()
    {
        return file + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
    }
#endregion
}
