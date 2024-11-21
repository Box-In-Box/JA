using System;
using System.Collections;
using System.Collections.Generic;
using Gongju.Web;
using Sirenix.OdinInspector;
using UnityEngine;

public class MissionManager : MonoSingleton<MissionManager>
{
    [field : Title("[ Init Value ]")]
    [field : SerializeField] public bool isMissionData { get; set; }
    [field : SerializeField] public bool isMemberMissionData { get; set; }
    [field : SerializeField] public bool isSentMissionData { get; set; }
    [field : SerializeField] public bool isSentMissionProgressData { get; set; }
    [field : SerializeField] public bool isMissionInit { get; set; }
    [field: SerializeField] public MissionSubject missionSubject { get; set; }

    [field: Title("[ Mission Managers ]")]
    [field: SerializeField] public CourseMissionManager courseMissionManager { get; set; }
    [field: SerializeField] public TreasureMissionManager treasureMissionManager { get; set; }

    [field: Title("[ ETC ]")]
    private Coroutine updateCoroutine;

    private IEnumerator Start()
    {
        yield return null;
        if (!GameManager.instance.isGuest)
        {
            yield return new WaitUntil(() => isMissionData && isMemberMissionData);
            MissionSetting();
            isMissionInit = true;
        }
    }

    private void MissionSetting()
    {
        foreach (var mission in DatabaseConnector.instance.missionData)
        {
            switch (mission.mission_type)
            {
                case "daily":
                    break;
                case "weekly":
                    break;
                case "course":
                    string routesType = GetMissionRoutesType(mission.mission_id);
                    if (IsCurrentMission(mission.mission_id) && routesType != "completed" && routesType != "finished")
                    { // 현재 미션 중
                        courseMissionManager.currentCourceMission = mission;
                        courseMissionManager.CourseSetting();
                    }
                    break;
                case "treasure":
                    treasureMissionManager.TreasureSetting(mission.mission_id);
                    break;
            }
        }
    }

    /// <summary>
    /// 미션 Data
    /// </summary>
    public MissionData GetMission(int missionId)
    {
        return Array.Find(DatabaseConnector.instance.missionData, x => x.mission_id == missionId);
    }

    /// <summary>
    /// 미션의 진행도 Data
    /// </summary>
    public MemberMissionData GetMissionProgress(int missionId)
    {
        return Array.Find(DatabaseConnector.instance.memberMissionData, x => x.missionIdx == missionId);
    }

    /// <summary>
    /// 미션의 현재 진행 타입
    /// </summary>
    public string GetMissionRoutesType(int missionId)
    {
        var progressData = GetMissionProgress(missionId);
        if (progressData != null)
        {
            int progress = progressData.missionProgress;

            if (progress >= GetMission(missionId)?.routes.Length) return "";
            if (progress < 0) return "finished";

            return GetMission(missionId).routes[progress].key_type;
        }
        return "";
    }

    /// <summary>
    /// 해당 미션이 진행중인지
    /// </summary>
    public bool IsCurrentMission(int missionId)
    {
        return DatabaseConnector.instance.memberData.current_mission_idx == missionId ? true : false;
    }

    #region Update Mission

    #region Updata Mission (미션 업데이트)
    public void UpdateMissionProgress(int missionId, int progress, Action callback)
    {
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateCurrentMissionProgressCoroutine(missionId, progress, callback));
    }
    #endregion

    private IEnumerator UpdateCurrentMissionProgressCoroutine(int missionId, int progress, Action callback)
    {
        isSentMissionData = false;
        isSentMissionProgressData = false;
        isMemberMissionData = false;
        isMissionInit = false;

        DatabaseConnector.instance.SendMemberCurrentMission(() => isSentMissionData = true, null, missionId);
        DatabaseConnector.instance.SendMissionProgressData(() => isSentMissionProgressData = true, null, missionId, progress);
        yield return new WaitUntil(() => isSentMissionData && isSentMissionProgressData);

        DatabaseConnector.instance.GetMemberMissionData(() => isMemberMissionData = true, null);
        yield return new WaitUntil(() => isMemberMissionData);

        isMissionInit = true;
        callback?.Invoke();
    }
    #endregion

    #region Change Current Mission (현재 진행중인 미션 변경)
    public void UpdateCurrentMission(int missionId, Action callback)
    {
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateCurrentMissionCoroutine(missionId, callback));
    }

    private IEnumerator UpdateCurrentMissionCoroutine(int missionId, Action callback)
    {
        isSentMissionData = false;
        isMemberMissionData = false;
        isMissionInit = false;

        DatabaseConnector.instance.SendMemberCurrentMission(() => isSentMissionData = true, null, missionId);
        yield return new WaitUntil(() => isSentMissionData);

        DatabaseConnector.instance.GetMemberMissionData(() => isMemberMissionData = true, null);
        yield return new WaitUntil(() => isMemberMissionData);

        isMissionInit = true;
        callback?.Invoke();
    }
    #endregion
}