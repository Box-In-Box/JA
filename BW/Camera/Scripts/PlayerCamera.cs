using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

[Serializable]
public class InputData
{
    [SerializeField] public int inputId;                // 터치 index
    [SerializeField] public bool isPointerDownOnUI;     // UI위에서 포인터 다운이 발생했는지 여부
    [SerializeField] public bool isDragging;	        // 드래그 중인지 여부
    [SerializeField] public Vector2 dragStartPos;	    // 시작 드래그 좌표
    [SerializeField] public Vector2 currentDragPos;     // 현재 드래그 좌표
    [SerializeField] public Vector2 dragVector;         // 현재 좌표 - 직전 드래그 좌표(이동한 양)
    [SerializeField] public Touch touch;                // For Touch
}

public class PlayerCamera : MonoBehaviour
{
    [Title("[ Camera ]")]
    private bool isCanControl = true;
    public CinemachineFreeLook freeLook { get; private set; }
    [field : SerializeField, Range(0f, 2f)] public float lookAtYOffset { get; private set; } = 1.3f;
    private CinemachineFreeLook.Orbit[] originalOrbits;
    private float originalfov;

    [Title("[ Input ]")]
    [SerializeField] private InputData[] inputData = new InputData[3];
    private List<InputData> currentInputData = new List<InputData>();

    [Title("[ Drag ]")]
    [SerializeField] private bool isCanDrag;
    [SerializeField, Range(0f, 10f)] private float xDragSpeed = 5f;
    [SerializeField, Range(0f, 10f)] private float yDragSpeed = 5f;
    public Action dragAction;

    [Title("[ Zoom ]")]
    [SerializeField] private bool isCanZoom;
    [field : SerializeField, Range(0f, 10f)] public float zoomSpeed { get; private set; } = 5f;
    [field : SerializeField, Range(0f, 10f)] public float zoomMinPercent { get; private set; } = .5f;
    [field : SerializeField, Range(0f, 10f)] public float zoomMaxPercent { get; private set; } = 1f;
    [field : SerializeField, Range(0f, 10f)] public float targetZoomValue { get; private set; } = 1f;
    [field : SerializeField, Range(0f, 10f)] public float currentZoomValue { get; private set; } = 1f;
    public Action zoomAction;
    
    [Title("[ Setting Value ]")]
    private Coroutine zoomCoroutine;
    float playerHeight = 1.6f;
    float playerRound = 0.5f;
    private bool isAxisY = true;
    private bool isAxisX = true;

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();

        CinemachineCore.GetInputAxis = ClickControl;
        // X Axis 예외처리
        Transform camPos = new GameObject("CamPosition").transform;
        camPos.SetParent(GameManager.instance.playerCharacter.transform, false);
        camPos.rotation = Quaternion.Euler(Vector3.zero);

        freeLook.Follow = camPos;
        freeLook.LookAt = camPos;
        freeLook.m_XAxis.Value = GameManager.instance.playerCharacter.transform.eulerAngles.y;
        SetLookAdYOffSet(lookAtYOffset);

        originalOrbits = new CinemachineFreeLook.Orbit[freeLook.m_Orbits.Length];
        for (int i = 0; i < freeLook.m_Orbits.Length; i++) {
            originalOrbits[i].m_Height = freeLook.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = freeLook.m_Orbits[i].m_Radius;
        }
        originalfov = freeLook.m_Lens.FieldOfView;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
#if UNITY_EDITOR
        //TouchSimulation.Enable(); // Editor 터치 테스트용 -> 시뮬 중 휠 줌 불가
#endif
    }

    private  void OnDisable()
    {
        EnhancedTouchSupport.Disable();
#if UNITY_EDITOR
        //TouchSimulation.Disable();  // Editor 터치 테스트용 -> 시뮬 중 휠 줌 불가
#endif
    }

    public void SetControl(bool value) => isCanControl = value;

    private float ClickControl(string axis)
    {
        if (isCanDrag && currentInputData.Count == 1) {
            if  (axis == "Mouse X" && isAxisX){
                dragAction?.Invoke();
                return currentInputData[0].dragVector.x * xDragSpeed * .01f;
            }
            else if (axis == "Mouse Y" && isAxisY) {
                dragAction?.Invoke();
                return currentInputData[0].dragVector.y * yDragSpeed * .01f;
            }
        }
        return 0; 
    }

    private void Update()
    {
        if (!isCanControl) return;
#if UNITY_EDITOR
        if (TouchSimulation.instance != null && TouchSimulation.instance.isActiveAndEnabled) TouchInput();
        else MouseInput();
        ScrollWhellZoom();
#elif UNITY_STANDALONE
        MouseInput();
        ScrollWhellZoom();
#elif UNITY_ANDROID || UNITY_IOS
        TouchInput();
        PinchZoom();
#endif
        StateCheck();
    }

#region KeyBoard
    private void MouseInput()
    {
        // Down
        if(Mouse.current.leftButton.wasPressedThisFrame) {
            inputData[0].inputId = 0;
            inputData[0].isPointerDownOnUI = BW.Tool.IsPointerOverUI();
            inputData[0].isDragging = false;
            if (inputData[0].isPointerDownOnUI == false) {
                inputData[0].dragStartPos = Mouse.current.position.ReadValue();
                inputData[0].currentDragPos = inputData[0].dragStartPos;
                currentInputData.Add(inputData[0]);
            }
        }
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            inputData[1].inputId = 1;
            inputData[1].isPointerDownOnUI = BW.Tool.IsPointerOverUI();
            inputData[1].isDragging = false;
            if (inputData[1].isPointerDownOnUI == false) {
                inputData[1].dragStartPos = Mouse.current.position.ReadValue();
                inputData[1].currentDragPos = inputData[1].dragStartPos;
                currentInputData.Add(inputData[1]);
            }
        }
        
        // Pressed
        if(Mouse.current.leftButton.isPressed) {
            if (inputData[0].isPointerDownOnUI == false) {
                inputData[0].dragVector = Mouse.current.delta.ReadValue();
                inputData[0].currentDragPos = Mouse.current.position.ReadValue();
                inputData[0].isDragging = true; 
            }            
        }
        if(Mouse.current.rightButton.isPressed) {
            if (inputData[1].isPointerDownOnUI == false) {
                inputData[1].dragVector = Mouse.current.delta.ReadValue();
                inputData[1].currentDragPos = Mouse.current.position.ReadValue();
                inputData[1].isDragging = true; 
            }            
        }
        // Up
        if(Mouse.current.leftButton.wasReleasedThisFrame) {
            currentInputData.RemoveAll(x => x.inputId == 0);
            ResetTouchData(ref inputData[0]);
        }
        if(Mouse.current.rightButton.wasReleasedThisFrame) {
            currentInputData.RemoveAll(x => x.inputId == 1);
            ResetTouchData(ref inputData[1]);
        }
    }
#endregion

    // Check is Drag Or Zoom
    private void StateCheck()
    {
        int count = 0;
        foreach (var x in inputData) { count += x.isDragging ? 1 : 0; }

        isCanDrag = count == 1;
        isCanZoom = count == 2;
    }

    private void TouchInput()
    {
        for (int i = 0; i < Touch.activeTouches.Count; ++i) {
            if (i > inputData.Length) continue;

            // Get Current Touch, Data
            Touch touch = Touch.activeTouches[i];
            InputData data = inputData[GetTouchData(touch.touchId)];

            // Start
            if (touch.phase == TouchPhase.Began) {
                data.inputId = touch.touchId;
                data.isPointerDownOnUI = BW.Tool.IsPointerOverUI(i);
                data.isDragging = false;
                data.touch = touch;
                if (data.isPointerDownOnUI == false) {
                    data.dragStartPos = touch.screenPosition;
                    data.currentDragPos = touch.screenPosition;
                    currentInputData.Add(data);
                }
            }

            // Running
            if (touch.phase == TouchPhase.Stationary) {
                if (!data.isPointerDownOnUI) {
                    data.dragVector = Vector2.zero;
                }
            }
            else if (touch.phase == TouchPhase.Moved) {
                if (!data.isPointerDownOnUI) {
                    data.dragVector = touch.delta;
                    data.currentDragPos = touch.screenPosition;
                    data.isDragging = true; 
                }
            }

            // End
            if (touch.phase == TouchPhase.Ended) {
                currentInputData.RemoveAll(x => x.inputId == touch.touchId);
                ResetTouchData(ref data);
            }
        }
    }

    /// <summary>
    /// 인풋 데이터에 값이 있으면 반환, 없으면 가까운 빈 공간 반환
    /// </summary>
    private int GetTouchData(int touchId)
    {
        int index = Array.FindIndex(inputData, x => x.inputId == touchId);

        if (index > -1) return index;
        else return Array.FindIndex(inputData, x => x.inputId == 0);
    }

    /// <summary>
    /// 인풋 리셋 용
    /// </summary>
    private void ResetTouchData(ref InputData touchData)
    {
        touchData.inputId = 0;
        touchData.touch = new Touch();
        touchData.isPointerDownOnUI = false;
        touchData.isDragging = false;
        touchData.dragStartPos = Vector2.zero;
        touchData.currentDragPos = Vector2.zero;
        touchData.dragVector = Vector2.zero;
    }

#region Zoom
    /// <summary>
    /// Keyboard Zoom
    /// </summary>
    private void ScrollWhellZoom()
    {
        float scrollWheel = Mouse.current.scroll.ReadValue().y;
        if (scrollWheel != 0 && isAxisY) {
            targetZoomValue += -scrollWheel * zoomSpeed * .0001f;
            targetZoomValue = Mathf.Clamp(targetZoomValue, zoomMinPercent, zoomMaxPercent);
            if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
            zoomCoroutine = StartCoroutine(ZoomCoroutine());
        }
    }

    /// <summary>
    /// Touch Zoom
    /// </summary>
    private void PinchZoom()
    {
        if(isCanZoom && currentInputData.Count == 2 && isAxisY) {
            Vector2 prevPos0 = currentInputData[0].touch.screenPosition - currentInputData[0].touch.delta;
            Vector2 prevPos1 = currentInputData[1].touch.screenPosition - currentInputData[1].touch.delta;

            float prevDistance = (prevPos0 - prevPos1).magnitude;
            float currDistance = (currentInputData[0].touch.screenPosition - currentInputData[1].touch.screenPosition).magnitude;

            float diff = currDistance - prevDistance;

            targetZoomValue -= diff * zoomSpeed * .0001f;
            targetZoomValue = Mathf.Clamp(targetZoomValue, zoomMinPercent, zoomMaxPercent);
            if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
            zoomCoroutine = StartCoroutine(ZoomCoroutine());
        }
    }

    /// <summary>
    /// Zoom In And Out
    /// </summary>
    private IEnumerator ZoomCoroutine()
    {
        while (Mathf.Abs(targetZoomValue - currentZoomValue) >= 0.001f) {
            currentZoomValue = Mathf.Lerp(currentZoomValue, targetZoomValue, Time.deltaTime);
            SetCameraRound();
            zoomAction?.Invoke();
            yield return new WaitForEndOfFrame();
        }
        zoomAction?.Invoke();
        currentZoomValue = targetZoomValue;
        SetCameraRound();
    }

    private void SetCameraRound()
    {
        for (int i = 0; i < freeLook.m_Orbits.Length; i++) {
            int index = !isAxisY ? 0 : i;
            freeLook.m_Orbits[i].m_Height = Mathf.Max(originalOrbits[index].m_Height * currentZoomValue, playerHeight - playerHeight / 2 * i);
            freeLook.m_Orbits[i].m_Radius = Mathf.Max(originalOrbits[index].m_Radius * currentZoomValue, playerRound);
        }
    }
#endregion

#region View
    public void ChangeView(float maxViewPercent, float changeSpeed)
    {
        zoomMaxPercent = maxViewPercent;

        if (changeSpeed == 0f) {
            targetZoomValue = zoomMaxPercent;
            currentZoomValue = targetZoomValue;
            SetCameraRound();
        }
        else {
            if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
            zoomCoroutine = StartCoroutine(ViewCoroutine(changeSpeed));
        }
    }

    private IEnumerator ViewCoroutine(float changeSpeed)
    {
        while (Mathf.Abs(zoomMaxPercent - targetZoomValue) >= 0.001f) {
            targetZoomValue = Mathf.Lerp(targetZoomValue, zoomMaxPercent, Time.deltaTime * changeSpeed);
            currentZoomValue = Mathf.Lerp(currentZoomValue, targetZoomValue, Time.deltaTime * changeSpeed);
            SetCameraRound();
            zoomAction?.Invoke();
            yield return new WaitForEndOfFrame();
        }
        zoomAction?.Invoke();
        targetZoomValue = zoomMaxPercent;
        currentZoomValue = targetZoomValue;
        SetCameraRound();
    }

    public void ChangeFOV(float value)
    {
        if (value == -1) value = originalfov;
        freeLook.m_Lens.FieldOfView = value;
    }

    public void SetOrbitWorld()
    {
        for (int i = 0; i < freeLook.m_Orbits.Length; i++) {
            freeLook.m_Orbits[i].m_Height = freeLook.m_Orbits[0].m_Height;
            freeLook.m_Orbits[i].m_Radius = freeLook.m_Orbits[0].m_Radius;
        }
        freeLook.m_YAxis.Value = 1f;
        freeLook.m_XAxis.Value = 200f;
        isAxisY = false;
        isAxisX = false;
    }

    public void SetOrbitStage()
    {
        for (int i = 0; i < freeLook.m_Orbits.Length; i++) {
            freeLook.m_Orbits[i].m_Height = Mathf.Max(originalOrbits[i].m_Height * currentZoomValue, playerHeight - playerHeight / 2 * i);
            freeLook.m_Orbits[i].m_Radius = Mathf.Max(originalOrbits[i].m_Radius * currentZoomValue, playerRound);
        }
        isAxisY = true;
    }

    public void SetLookAdYOffSet(float value)
    {
        for (int i = 0; i < freeLook.m_Orbits.Length; ++i) {
            freeLook.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = Vector3.up * value;
        }
    }
#endregion
}