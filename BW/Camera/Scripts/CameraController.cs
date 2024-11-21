using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [field : Title("[ Camera ]")]
    public PlayerCamera playerCamera { get; set; }
    public CinemachineBrain cinemachineBrain { get; set; }
    public ICinemachineCamera playerCinemachineCamera { get; set; }
    public ICinemachineCamera currentCinemachineCamera { get; set; }

    [field : Title("[ Camera Trigger ]")]
    public List<CameraTrigger> cameraTriggerList { get; set; } = new List<CameraTrigger>();

    [field : Title("[ Camera Setting ]")]
    [field : SerializeField, Range(0f, 10f)] public float baseCameraView { get; set; } = 1f;
    [field : SerializeField, Range(0f, 10f)] private float cameraView_ = 0f;
    public float cameraView
    {
        get => cameraView_;
        set
        {
            cameraView_ = value;
            ChangeView(baseCameraView + value);
        }
    }
    private bool isWorldView = false;

    private void Awake()
    {
        bool isSceneCamera = false;

        for (int i = 0; i < CinemachineCore.Instance.VirtualCameraCount; ++i) {
            var camera = CinemachineCore.Instance.GetVirtualCamera(i);
            // Scene Player Camera
            if (camera.tag == "PlayerCamera") {
                playerCinemachineCamera = CinemachineCore.Instance.GetVirtualCamera(i);
                currentCinemachineCamera = playerCinemachineCamera;
                isSceneCamera = true;
            }
            // Scene Trigger Camera
            else if (camera.tag == "TriggerCamera") {

            }
        }

        // Player Camera
        if (!isSceneCamera) {
            playerCamera = Instantiate(Resources.Load<PlayerCamera>("PlayerCamera"));
            playerCamera.transform.SetParent(this.transform);
            playerCinemachineCamera = playerCamera.freeLook;
            currentCinemachineCamera = playerCinemachineCamera;
        }

        // Brain Setting (첫 카메라 즉시 전환)
        cinemachineBrain = CinemachineCore.Instance.GetActiveBrain(0);
        cinemachineBrain.m_DefaultBlend.m_Time = 0f;
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

        currentCinemachineCamera.Priority = 11;

        // View Setting
        SetCameraView();
    }

    public IEnumerator Start()
    {
        // Brain Setting
        yield return null;
        cinemachineBrain.m_DefaultBlend.m_Time = .5f;
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
    }

#region 카메라 컨트롤 가능 여부 설정
    public void SetControl(bool value = true)
    {
        playerCamera?.SetControl(value);
    }
#endregion

#region 카메라 뷰 최대범위 지정
    public void ChangeView(float maxViewPercent, float changeSpeed = 1f)
    {
        playerCamera?.ChangeView(maxViewPercent, changeSpeed);
    }

    public void ChangeViewReturn(float changeSpeed)
    {
        ChangeView(baseCameraView, changeSpeed);
    }
#endregion

#region 카메라 FOV 설정
    public void ChangeFOV(float fov)
    {
        playerCamera?.ChangeFOV(fov);
    }

    public void ChangeFOVReturn()
    {
        ChangeFOV(-1f);
    }
#endregion

#region 카메라 변경
    public void SwitchCamera(ICinemachineCamera cinemachineCamera = null)
    {
        currentCinemachineCamera.Priority = 10;
        cinemachineCamera.Priority = 11;

        currentCinemachineCamera = cinemachineCamera;
    }

    public void SwitchCameraReturn()
    {
        currentCinemachineCamera.Priority = 10;
        playerCinemachineCamera.Priority = 11;

        currentCinemachineCamera = playerCinemachineCamera;
    }

    public void SetFreeLookAngle(float angle)
    {
        playerCamera.freeLook.m_XAxis.Value = angle;
    }
#endregion

#region Scene View
    public void SetCameraView()
    {
        if (GameManager.instance.currentScene == "Stage_Outside" || GameManager.instance.currentScene == "MyRoom") {
            WorldView();
        }
        else {
            StageView();
        }
    }

    [ButtonGroup, GUIColor(0f, 1f, 0f)]
    public void WorldView()
    {
        playerCamera?.SetOrbitWorld();
        if (!isWorldView)
        {
            baseCameraView = 3f;
            ChangeView(baseCameraView + cameraView, 10f);
        }
        isWorldView = true;
    }

    [ButtonGroup, GUIColor(0f, 1f, 0f)]
    public void StageView()
    {
        playerCamera?.SetOrbitStage();
        if (isWorldView)
        {
            baseCameraView = 1f;
            ChangeView(baseCameraView + cameraView, 10f);
        }
        isWorldView = false;
    }
    #endregion
}