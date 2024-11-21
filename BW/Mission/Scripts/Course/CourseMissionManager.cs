using Gongju.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseMissionManager : MonoBehaviour
{
    [SerializeField] private MissionManager missionManager;
    [field: SerializeField] public MissionSignSubject missionSignSubject { get; set; }
    [field: SerializeField] public MissionCourseLine missionCourseLine { get; set; }
    [field: SerializeField, ReadOnly] public MissionData currentCourceMission { get; set; }

    private void Awake()
    {
        currentCourceMission = new MissionData();
        currentCourceMission.mission_id = -1;
    }

    public void CourseSetting()
    {
        // Line Renderer
        missionCourseLine?.CourseLineRenderer(DatabaseConnector.instance.memberData.current_mission_idx);

        // Quiz Mission Sign
        bool isMission = DatabaseConnector.instance.memberData.current_mission_idx > 0;
        missionSignSubject?.Sign(isMission ? missionManager.GetMissionRoutesType(currentCourceMission.mission_id) : "");
    }

    #region Course UI
    public void SelectCourseMission(CourseMission courseMission)
    {
        string routesType_Current = missionManager.GetMissionRoutesType(currentCourceMission.mission_id);
        string routesType = missionManager.GetMissionRoutesType(courseMission.missionData.mission_id);
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
    /// 코스 선택
    /// </summary>
    public void SetCourseMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.missionData.mission_title + "을(를) 진행 하시겠습니까?"
            , () => MissionManager.instance.UpdateCurrentMission(courseMission.missionData.mission_id, () => {
                missionManager.missionSubject.Selected(courseMission.missionData.mission_id);
                currentCourceMission = courseMission.missionData;
                CourseSetting();
            })
        );
    }

    /// <summary>
    /// 같은 코스 선택 => 코스 취소
    /// </summary>
    public void SetNormalCourseMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.missionData.mission_title + "을(를) 포기 하시겠습니까"
            , () => MissionManager.instance.UpdateCurrentMission(0, () => {
                missionManager.missionSubject.Normal(courseMission.missionData.mission_id);
                currentCourceMission = new MissionData();
                currentCourceMission.mission_id = -1;
                CourseSetting();
            })
        );
    }

    public void ChangeCourseMission(CourseMission courseMission)
    {
        PopupManager.instance.Popup(courseMission.missionData.mission_title + "으로 변경 하시겠습니까?\n(미션 진행상태는 저장 됩니다.)"
            , () => MissionManager.instance.UpdateCurrentMission(courseMission.missionData.mission_id, () => {
                missionManager.missionSubject.Normal(currentCourceMission.mission_id);
                missionManager.missionSubject.Selected(courseMission.missionData.mission_id);
                currentCourceMission = courseMission.missionData;
                CourseSetting();
            })
        );
    }

    /// <summary>
    /// 코스 완료 (보상 전)
    /// </summary>
    public void CompletedCourseMission(CourseMission courseMission, Action callback)
    {
        Reward.Receive(courseMission.rewardList, () => PopupManager.instance.Close<RewardPopup>());
        missionManager.missionSubject.Finished(courseMission.missionData.mission_id);

        callback += CourseSetting;
        int progress = -1;

        MissionManager.instance.UpdateMissionProgress(courseMission.missionData.mission_id, progress, callback);
    }

    /// <summary>
    /// 코스 완료 (보상 후)
    /// </summary>
    public void FinishedCourseMission(CourseMission courseMission)
    {
        // 이미 완료된 코스 미션 입니다
    }
    #endregion

    #region Updata Quiz Mission
    public void UpdateMissionProgress(QuizData quizData, Action callback)
    {
        if (MissionManager.instance.GetMissionRoutesType(currentCourceMission.mission_id) == quizData.quiz_type)
        {
            callback += CourseSetting;
            var progressData = missionManager.GetMissionProgress(currentCourceMission.mission_id);
            int progress = ++progressData.missionProgress;

            MissionManager.instance.UpdateMissionProgress(currentCourceMission.mission_id, progress, callback);
        }
    }
    #endregion
}