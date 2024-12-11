using Cinemachine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class MissionCourseCamera : MonoBehaviour
{
    [Serializable]
    public class CourseTargetGroup
    {
        public int missionId;
        public CinemachineTargetGroup course;
    }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CourseTargetGroup[] courseTargetGroup;
    [SerializeField, Range(.5f, 10f)] private float triggerTime = 4f;

    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    private void OnEnable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.CourseMissionManager.MissionCourseCamera = this;
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.CourseMissionManager.MissionCourseCamera = null;
        }
    }

    public void CourseCamera(int missionId)
    {
        CourseTargetGroup currentCourse = courseTargetGroup.FirstOrDefault(x => x.missionId == missionId);
        if (currentCourse != null)
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            ChangeCamera(currentCourse).Forget();
        }
    }

    public async UniTaskVoid ChangeCamera(CourseTargetGroup target)
    {
        if (GameManager.instance.PlayerCameraController)
        {
            virtualCamera.Follow = target.course.transform;
            virtualCamera.LookAt = target.course.transform;
            virtualCamera.Priority = 12;
            GameManager.instance.playerController.ChangePlayerState(PlayerState.Wait);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(triggerTime), cancellationToken: tokenSource.Token);

        CourseCameraReturn();
    }

    [Button]
    public void CourseCameraReturn()
    {
        if (GameManager.instance.PlayerCameraController)
        {
            virtualCamera.Priority = 10;
            GameManager.instance.playerController.ChangePlayerState(PlayerState.Normal);
        }
    }

    private void OnDestroy()
    {
        tokenSource?.Dispose();
    }
}