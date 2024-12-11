using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gongju.Web;
using Sirenix.OdinInspector;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    private static MissionManager _instance;
    public static MissionManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SystemCall.instance?.Call<MissionManager>();
            }
            return _instance;
        }
    }

    [field: Title("[ Mission Managers ]")]
    [field: SerializeField] public NormalMissionManager NormalMissionManager { get; set; }
    [field: SerializeField] public CourseMissionManager CourseMissionManager { get; set; }
    [field: SerializeField] public TreasureMissionManager TreasureMissionManager { get; set; }

    [field : Title("[ Init Value ]")]
    [field : SerializeField] public bool IsMissionData { get; set; }
    [field : SerializeField] public bool IsMemberMissionData { get; set; }
    [field : SerializeField] public bool IsMissionInit { get; set; }
    [field: SerializeField] public MissionSubject MissionSubject { get; set; }

    [field: Title("[ Date Time Action ]")]
    public Action MidnightAction { get; set; } // 매일 0시
    public Action WeeklyAction { get; set; } // 월요일 0시

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance == this)
        {
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        if (!GameManager.instance.isGuest)
        {
            yield return new WaitUntil(() => IsMissionData && IsMemberMissionData);
            MissionSetting();
            IsMissionInit = true;

            TimeTask().Forget();
        }
    }

    private void MissionSetting()
    {
        foreach (var mission in DatabaseConnector.instance.missionData)
        {
            switch (mission.mission_type)
            {
                case "daily":
                case "weekly":
                    NormalMissionManager.NormalSetting(mission);
                    break;
                case "course":
                    string routesType = GetMissionRouteType(mission.mission_id);
                    if (IsCurrentMission(mission.mission_id) && routesType != "finished")
                    {
                        CourseMissionManager.CurrentCourceMission = mission;
                        CourseMissionManager.MissionSetting();
                    }
                    break;
                case "treasure":
                    TreasureMissionManager.TreasureSetting(mission.mission_id);
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
    /// 미션의 현재 진행 상태
    /// </summary>
    public int GetMissionRoute(int missionId)
    {
        var progressData = GetMissionProgress(missionId);
        if (progressData != null)
        {
            return progressData.missionProgress;
        }
        return 0;
    }

    /// <summary>
    /// 미션의 현재 진행 타입
    /// </summary>
    public string GetMissionRouteType(int missionId)
    {
        var progressData = GetMissionProgress(missionId);
        if (progressData != null)
        {
            int progress = progressData.missionProgress;
            var mission = GetMission(missionId);

            if (progress >= mission?.routes.Length)
            {
                int length = mission.routes.Length;
                return mission.routes[length - 1].key_type;
            }
                
            if (progress < 0)
            {
                return "finished";
            }

            return mission.routes[progress].key_type;
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

    public async UniTaskVoid TimeTask()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.Date.AddDays(1); // 다음 0시 계산
            TimeSpan timeUntilMidnight = nextMidnight - now;

            await UniTask.Delay(timeUntilMidnight, ignoreTimeScale: true, cancellationToken: this.GetCancellationTokenOnDestroy());

            MidnightAction?.Invoke();

            if (nextMidnight.DayOfWeek == DayOfWeek.Monday)
            {
                WeeklyAction?.Invoke();
            }
        }
    }
}