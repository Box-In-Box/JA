using Gongju.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseMissionManager : MonoBehaviour
{
    [SerializeField] private MissionManager missionManager;
    [field: SerializeField] public MissionSignSubject MissionSignSubject { get; set; }
    [field: SerializeField, ReadOnly] public MissionGateway MissionGateway { get; set; }
    [field: SerializeField, ReadOnly] public MissionCourseLine MissionCourseLine { get; set; }
    [field: SerializeField, ReadOnly] public MissionData CurrentCourceMission { get; set; }

    private void Awake()
    {
        CurrentCourceMission = new MissionData();
        CurrentCourceMission.mission_id = -1;
    }

    public void CourseSetting()
    {
        int courrentMissionId = DatabaseConnector.instance.memberData.current_mission_idx;

        // Line Renderer
        MissionCourseLine?.CourseLineRenderer(courrentMissionId);

        // Line Renderer
        MissionGateway?.CourseGateWay(courrentMissionId, MissionManager.instance.GetMissionRoutesType(courrentMissionId));

        // Quiz Mission Sign
        bool isMission = courrentMissionId > 0;
        MissionSignSubject?.Sign(isMission ? missionManager.GetMissionRoutesType(CurrentCourceMission.mission_id) : "");
    }

    #region Course UI
    public void SelectCourseMission(CourseMission courseMission)
    {
        string routesType_Current = missionManager.GetMissionRoutesType(CurrentCourceMission.mission_id);
        string routesType = missionManager.GetMissionRoutesType(courseMission.MissionData.mission_id);
        // Change
        if (routesType_Current != "completed" && routesType_Current != "finished" && routesType != "completed" && routesType != "finished")
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
            , () => MissionManager.instance.UpdateCurrentMission(courseMission.MissionData.mission_id, () => {
                missionManager.missionSubject.Selected(courseMission.MissionData.mission_id);
                CurrentCourceMission = courseMission.MissionData;
                CourseSetting();
            })
        );
    }

    /// <summary>
    /// ���� �ڽ� ���� => �ڽ� ���
    /// </summary>
    public void SetNormalCourseMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.MissionData.mission_title + "��(��) ���� �Ͻðڽ��ϱ�"
            , () => MissionManager.instance.UpdateCurrentMission(0, () => {
                missionManager.missionSubject.Normal(courseMission.MissionData.mission_id);
                CurrentCourceMission = new MissionData();
                CurrentCourceMission.mission_id = -1;
                CourseSetting();
            })
        );
    }

    public void ChangeCourseMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.MissionData.mission_title + "���� ���� �Ͻðڽ��ϱ�?\n(�̼� ������´� ���� �˴ϴ�.)"
            , () => MissionManager.instance.UpdateCurrentMission(courseMission.MissionData.mission_id, () => {
                missionManager.missionSubject.Normal(CurrentCourceMission.mission_id);
                missionManager.missionSubject.Selected(courseMission.MissionData.mission_id);
                CurrentCourceMission = courseMission.MissionData;
                CourseSetting();
            })
        );
    }

    /// <summary>
    /// �ڽ� �Ϸ� (���� ��)
    /// </summary>
    public void CompletedCourseMission(CourseMission courseMission, Action callback)
    {
        Reward.Receive(courseMission.rewardList, () => PopupManager.instance.Close<RewardPopup>());
        missionManager.missionSubject.Finished(courseMission.MissionData.mission_id);

        callback += CourseSetting;
        int progress = -1;

        MissionManager.instance.UpdateMissionProgress(courseMission.MissionData.mission_id, progress, callback);
    }

    /// <summary>
    /// �ڽ� �Ϸ� (���� ��)
    /// </summary>
    public void FinishedCourseMission(CourseMission courseMission)
    {
        // �̹� �Ϸ�� �ڽ� �̼� �Դϴ�
    }
    #endregion

    #region Updata Quiz Mission
    public void UpdateMissionProgress(QuizData quizData, Action callback)
    {
        if (MissionManager.instance.GetMissionRoutesType(CurrentCourceMission.mission_id) == quizData.quiz_type)
        {
            callback += CourseSetting;
            var progressData = missionManager.GetMissionProgress(CurrentCourceMission.mission_id);
            int progress = ++progressData.missionProgress;

            MissionManager.instance.UpdateMissionProgress(CurrentCourceMission.mission_id, progress, callback);
        }
    }
    #endregion
}