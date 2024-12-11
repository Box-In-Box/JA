using Cysharp.Threading.Tasks;
using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public enum MissionType
{
    clear_all, // 예외

    enter_map,
    enter_myroom,
    enter_otherroom,
    enter_myroomshop,
    enter_avatarshop,

    play_minigame, // 종합 적용 (play)
    play_baseball,
    play_injeolmi,
    play_archery,
    play_roasting,
    play_gathering,

    screenshot,
    riding,
    action,
    emotion,

    youtube,
    homepage,
}

public class NormalMissionManager : MonoBehaviour
{
    [Serializable]
    public class Normal
    {
        public List<Mission> missions = new List<Mission>();
    }

    [Serializable]
    public class Mission
    {
        public int missionId;
        public string dateType;
        public int currentCount;
        public int completeCount;

        public Mission(int missionId, string dateType, int currentCount, int completeCount)
        {
            this.missionId = missionId;
            this.dateType = dateType;
            this.currentCount = currentCount;
            this.completeCount = completeCount;
        }
    }

    private void Start()
    {
        missionManager.MidnightAction += () => ResetMission("daily");
        missionManager.WeeklyAction += () => ResetMission("weekly");
    }

    [SerializeField] private MissionManager missionManager;
    [SerializeField] public SerializedDictionary<string, Normal> MissionDatas = new SerializedDictionary<string, Normal>();
    public void NormalSetting(MissionData missiondata)
    {
        int progress = MissionManager.instance.GetMissionRoute(missiondata.mission_id);
        string routeType = GetNormalMissionRouteType(missiondata.mission_id);

        // 공통 미션 묶음
        if (!MissionDatas.ContainsKey(routeType))
        {
            Normal normal = new Normal();
            MissionDatas.Add(routeType, normal);

            Mission mission = new Mission(missiondata.mission_id, missiondata.mission_type, progress, int.Parse(missiondata.mission_sub));
            normal.missions.Add(mission);
        }
        else
        {
            var findMission = MissionDatas[routeType].missions.Find(x => x.missionId == missiondata.mission_id);
            if (findMission == null)
            {
                Mission mission = new Mission(missiondata.mission_id, missiondata.mission_type, progress, int.Parse(missiondata.mission_sub));
                MissionDatas[routeType].missions.Add(mission);
            }
            else
            {
                findMission.currentCount = progress;
            }
        }
    }

    [Button]
    public void CompleteMission(MissionType routeType)
    {
        CompleteMissionTask(routeType.ToString()).Forget();
    }

    public async UniTaskVoid CompleteMissionTask(string routeType)
    {
        if (MissionDatas.ContainsKey(routeType))
        {
            List<bool> requests = new List<bool>();
            bool isCategory = HasCategory(routeType).Item1;
            string categoryName = HasCategory(routeType).Item2;
            int num = 0;

            if (isCategory)
            {
                requests = new List<bool>(new bool[MissionDatas[routeType].missions.Count + MissionDatas[categoryName].missions.Count]);
                num = MissionDatas[routeType].missions.Count - 1;

                foreach (var mission in MissionDatas[categoryName].missions)
                {
                    ++mission.currentCount;
                    ++num;

                    if (mission.currentCount > mission.completeCount)
                    {
                        requests[num] = true;
                    }
                    else
                    {
                        UpdateMissionTask(mission.missionId, mission.currentCount, requests,num).Forget();
                    }
                }
            }
            else
            {
                requests = new List<bool>(new bool[MissionDatas[routeType].missions.Count]);
            }

            num = -1;
            foreach (var mission in MissionDatas[routeType].missions)
            {
                ++mission.currentCount;
                ++num;

                if (mission.currentCount > mission.completeCount)
                {
                    requests[num] = true;
                }
                else
                {
                    UpdateMissionTask(mission.missionId, mission.currentCount, requests, num).Forget();
                }
            }

            MissionSetting(); //UI 우선 처리
            await UniTask.WaitUntil(() => requests.TrueForAll(x => x.Equals(true)));

            Action action = null;
            bool finish = false;
            
            foreach (var mission in MissionDatas[routeType].missions)
            {
                action += () => NormalSetting(missionManager.GetMission(mission.missionId));
            }
            if (isCategory)
            {
                foreach (var mission in MissionDatas[categoryName].missions)
                {
                    action += () => NormalSetting(missionManager.GetMission(mission.missionId));
                }
            }
            action += () => finish = true;

            DatabaseConnector.instance.GetMemberMissionData(action, null);
            await UniTask.WaitUntil(() => finish);
        }
    }

    private async UniTaskVoid UpdateMissionTask(int missionId, int progress, List<bool> requests, int number)
    {
        bool finish = false;
        DatabaseConnector.instance.SendMissionProgressData(() => finish = true, null, missionId, progress);
        await UniTask.WaitUntil(() => finish);

        if (requests.Count > number)
        {
            requests[number] = true;
        }
    }

    public void MissionSetting()
    {
        foreach (var observer in MissionManager.instance.MissionSubject.normalMissionObserver)
        {
            (observer as NormalMission).MissionUpdate();
        }
    }

    public (bool, string) HasCategory(string routeType)
    {
        if (routeType == "play_baseball") return (true, "play_minigame");
        if (routeType == "play_injeolmi") return (true, "play_minigame");
        if (routeType == "play_archery") return (true, "play_minigame");
        if (routeType == "play_roasting") return (true, "play_minigame");
        if (routeType == "play_gathering") return (true, "play_minigame");

        return (false, null);
    }

    #region On Click UI
    public void NormalMission(NormalMission normalMission, Action callback)
    {
        // 처리 없음
    }

    public void CompletedMission(NormalMission normalMission, Action callback)
    {
        Reward.Receive(normalMission.rewardList, () => PopupManager.instance.Close<RewardPopup>());
        normalMission.Finished(normalMission.MissionData.mission_id);
        CompletedMissionTask(normalMission.MissionData.mission_id, -1, callback).Forget();
    }

    private async UniTaskVoid CompletedMissionTask(int missionId, int progress, Action callback)
    {
        bool finish = false;
        DatabaseConnector.instance.SendMissionProgressData(() => finish = true, null, missionId, progress);
        await UniTask.WaitUntil(() => finish);
        callback?.Invoke();
    }

    public void FinishedMission(NormalMission normalMission, Action callback)
    {
        // 처리 없음
    }
    #endregion

    public int GetMissionProgress(int missionId)
    {
        string routeType = MissionManager.instance.GetMissionRouteType(missionId);
        MissionDatas.TryGetValue(routeType, out var normal);
        var mission = normal.missions.Find(x => x.missionId == missionId);
        return mission.currentCount;
    }

    public string GetNormalMissionRouteType(int missionId)
    {
        var progressData = missionManager.GetMissionProgress(missionId);
        if (progressData != null)
        {
            var mission = missionManager.GetMission(missionId);
            return mission.routes[0].key_type;
        }
        return "";
    }

    [Button]
    public void ResetMission(string dateType)
    {
        ResetMissionTask(dateType).Forget();
    }

    public async UniTaskVoid ResetMissionTask(string dateType)
    {
        List<bool> resetRequests = new List<bool>();
        int count = 0;

        foreach (var missionData in MissionDatas)
        {
            foreach(Mission mission in missionData.Value.missions)
            {
                if (mission.dateType == dateType)
                {
                    mission.currentCount = 0;

                    resetRequests.Add(false);
                    int num = count++;

                    DatabaseConnector.instance.SendMissionProgressData(() =>
                    {
                        resetRequests[num] = true;
                        MissionSetting(mission.missionId);
                    }
                    , null, mission.missionId, 0);
                    continue;
                }
            }

            if (dateType == "weekly")
            {
                bool isCategory = HasCategory(missionData.Key).Item1;
                string categoryName = HasCategory(missionData.Key).Item2;
                if (isCategory)
                {
                    resetRequests.Add(false);
                    int num = count++;

                    Mission categoryMission = MissionDatas[categoryName].missions[0];
                    DatabaseConnector.instance.SendMissionProgressData(() =>
                    {
                        resetRequests[num] = true;
                        MissionSetting(categoryMission.missionId);
                    }
                    , null, categoryMission.missionId, 0);
                }
            }
        }
        await UniTask.WaitUntil(() => resetRequests.TrueForAll(x => x.Equals(true)));
        DatabaseConnector.instance.GetMemberMissionData(null, null);
    }

    public void MissionSetting(int missionId)
    {
        foreach (var observer in MissionManager.instance.MissionSubject.normalMissionObserver)
        {
            if ((observer as NormalMission).MissionData.mission_id == missionId)
            {
                (observer as NormalMission).MissionUpdate();
                break;
            }
        }
    }
}
