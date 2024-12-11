using Cysharp.Threading.Tasks;
using Gongju.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CourseMissionManager : MonoBehaviour
{
    [SerializeField] private MissionManager missionManager;
    [field: SerializeField] public MissionSignSubject MissionSignSubject { get; set; }
    [field: SerializeField, ReadOnly] public MissionGateway MissionGateway { get; set; }
    [field: SerializeField, ReadOnly] public MissionCourseLine MissionCourseLine { get; set; }
    [field: SerializeField, ReadOnly] public MissionCourseCamera MissionCourseCamera { get; set; }
    [field: SerializeField, ReadOnly] public MissionData CurrentCourceMission { get; set; }

    private void Awake()
    {
        CurrentCourceMission = new MissionData();
        CurrentCourceMission.mission_id = -1;
    }

    public void MissionSetting()
    {
        int courrentMissionId = DatabaseConnector.instance.memberData.current_mission_idx;
        // Line Renderer
        MissionCourseLine?.CourseLineRenderer(courrentMissionId);

        // Line Renderer
        MissionGateway?.CourseGateWay(courrentMissionId);

        // Quiz Mission Sign
        bool isMission = courrentMissionId > 0 && missionManager.GetMissionRoute(courrentMissionId) >= 0;
        if (!isMission) CurrentCourceMission = new MissionData();
        MissionSignSubject?.Sign(isMission ? missionManager.GetMissionRouteType(CurrentCourceMission.mission_id) : "");
    }

    #region Course UI
    public void SelectCourseMission(CourseMission courseMission)
    {
        string routesType_Current = missionManager.GetMissionRouteType(CurrentCourceMission.mission_id);
        string routesType = missionManager.GetMissionRouteType(courseMission.MissionData.mission_id);

        bool isCurrentMission = routesType_Current != "" && routesType_Current != "completed" && routesType_Current != "finished";
        bool isPramMission = routesType != "completed" && routesType != "finished";
        // Change
        if (isCurrentMission && isPramMission)
        {
            ChangeCourseMission(courseMission);
        }
        // Select
        else
        {
            SetCourseMission(courseMission);
        }
    }

    /// <summary>
    /// �ڽ� ����
    /// </summary>
    public void SetCourseMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.MissionData.mission_title + "��(��) ���� �Ͻðڽ��ϱ�?"
            , () => UpdateCurrentMission(courseMission.MissionData.mission_id, () =>
            {
                PopupManager.instance.Close<MissionPopup>();
                missionManager.MissionSubject.Selected(courseMission.MissionData.mission_id);
                CurrentCourceMission = courseMission.MissionData;
                MissionCourseCamera?.CourseCamera(courseMission.MissionData.mission_id);
                MissionSetting();
            })
        );
    }

    /// <summary>
    /// ���� �ڽ� ���� => �ڽ� ���
    /// </summary>
    public void SetNormalMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.MissionData.mission_title + "��(��) ���� �Ͻðڽ��ϱ�"
            , () => UpdateCurrentMission(0, () =>
            {
                missionManager.MissionSubject.Normal(courseMission.MissionData.mission_id);
                CurrentCourceMission = new MissionData();
                CurrentCourceMission.mission_id = -1;
                MissionSetting();
            })
        );
    }

    public void ChangeCourseMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.MissionData.mission_title + "���� ���� �Ͻðڽ��ϱ�?\n(�̼� ������´� ���� �˴ϴ�.)"
            , () => UpdateCurrentMission(courseMission.MissionData.mission_id, () =>
            {
                PopupManager.instance.Close<MissionPopup>();
                missionManager.MissionSubject.Normal(CurrentCourceMission.mission_id);
                missionManager.MissionSubject.Selected(courseMission.MissionData.mission_id);
                CurrentCourceMission = courseMission.MissionData;
                MissionCourseCamera?.CourseCamera(courseMission.MissionData.mission_id);
                MissionSetting();
            })
        );
    }

    public void UpdateCurrentMission(int missionId, Action callback)
    {
        UpdateCurrentMissionTask(missionId, callback).Forget();
    }

    private async UniTaskVoid UpdateCurrentMissionTask(int missionId, Action callback)
    {
        bool first = false;
        DatabaseConnector.instance.SendMemberCurrentMission(() => first = true, null, missionId);
        await UniTask.WaitUntil(() => first);

        bool second = false;
        DatabaseConnector.instance.GetMemberMissionData(() => second = true, null);
        await UniTask.WaitUntil(() => second);

        callback?.Invoke();
    }

    /// <summary>
    /// �ڽ� �Ϸ� (���� ��)
    /// </summary>
    public void CompletedMission(CourseMission courseMission, Action callback)
    {
        Reward.Receive(courseMission.rewardList, () => PopupManager.instance.Close<RewardPopup>());
        missionManager.MissionSubject.Finished(courseMission.MissionData.mission_id);

        callback += MissionSetting;

        UpdateMissionProgress(courseMission.MissionData.mission_id, -1, callback);
    }

    /// <summary>
    /// �ڽ� �Ϸ� (���� ��)
    /// </summary>
    public void FinishedMission(CourseMission courseMission)
    {
        // �̹� �Ϸ�� �ڽ� �̼� �Դϴ�
    }
    #endregion

    public void UpdateMissionProgress(int missionId, int progress, Action callback)
    {
        UpdateCurrentMissionProgressTask(missionId, progress, callback).Forget();
    }
    
    private async UniTaskVoid UpdateCurrentMissionProgressTask(int missionId, int progress, Action callback)
    {
        bool first = false;
        bool second = false;

        DatabaseConnector.instance.SendMemberCurrentMission(() => first = true, null, missionId);
        DatabaseConnector.instance.SendMissionProgressData(() => second = true, null, missionId, progress);

        await UniTask.WaitUntil(() => first && second);
        DatabaseConnector.instance.GetMemberMissionData(() => callback?.Invoke(), null);
    }

    #region Updata Quiz Mission
    public void UpdateQuizMissionProgress(QuizData quizData, Action callback)
    {
        if (MissionManager.instance.GetMissionRouteType(CurrentCourceMission.mission_id) == quizData.quiz_type)
        { 
            var progressData = missionManager.GetMissionProgress(CurrentCourceMission.mission_id);
            int progress = ++progressData.missionProgress;

            callback += MissionSetting;

            UpdateMissionProgress(CurrentCourceMission.mission_id, progress, callback);
        }
    }
    #endregion
}