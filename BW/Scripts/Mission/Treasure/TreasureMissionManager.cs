using Cysharp.Threading.Tasks;
using Gongju.Web;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using static NormalMissionManager;

[System.Serializable]
public class MissionTreasure
{
    [field: SerializeField] public int MissionId { get; set; }
    [field: SerializeField] public string TreasureTitle { get; set; }
    [field: SerializeField] public int Count { get; set; }
    [field: SerializeField] public string StringValue { get; set; }
    [field: SerializeField] public bool[] Value { get; set; }
    [field: SerializeField] public bool IsFinish { get; set; }

    public MissionTreasure(int missionId, string treasureTitle, int count, string stringValue, bool[] value)
    {
        this.MissionId = missionId;
        this.TreasureTitle = treasureTitle;
        this.Count = count;
        this.StringValue = stringValue;
        this.Value = value;
    }
}

public class TreasureMissionManager : MonoBehaviour
{
    [SerializeField] private MissionManager missionManager;
    [field: SerializeField] public TreasureConversion treasureConversion { get; set; }

    [field: SerializeField] public List<MissionTreasure> treasureDatas { get; set; } = new List<MissionTreasure>();
    [field: SerializeField] public List<Treasure> treasures { get; set; } = new List<Treasure>();

    public void TreasureSetting(int missionId)
    {
        string title = MissionManager.instance.GetMission(missionId).mission_title;
        var progressData = MissionManager.instance.GetMissionProgress(missionId);

        int count = 0;
        bool[] bools = Enumerable.Repeat(false, 5).ToArray(); ;
        string boolsString = "00000";
        if (progressData != null)
        {
            if (progressData.missionProgress == -1)
            {
                count = 5;
                bools = Enumerable.Repeat(true, 5).ToArray();
                boolsString = "11111";
            }
            else
            {
                var value = treasureConversion.GetProgress(progressData.missionProgress);
                count = value.Item1;
                bools = value.Item2;
                boolsString = value.Item3;
            }
        }

        var treasureData = treasureDatas.Find(x => x.MissionId == missionId);
        if (treasureData == null)
        {
            treasureDatas.Add(new MissionTreasure(missionId, title, count, boolsString, bools));
        }
        else
        {
            treasureData.Count = count;
            treasureData.StringValue = boolsString;
            treasureData.Value = bools;

            if (count == 5)
            {
                MissionManager.instance.MissionSubject.Completed(missionId);
            }
            else if (progressData.missionProgress == -1)
            {
                treasureData.IsFinish = true;
                MissionManager.instance.MissionSubject.Finished(missionId);
            }
            else
            {
                MissionManager.instance.MissionSubject.Normal(missionId);
            }
        }
        foreach (var treasure in treasures)
        {
            treasure.Setting(missionId, treasureData.Value);
        }
    }

    [Button]
    public void FindTreasure(int missionId, int index)
    {
        UpdateFindTreasureTask(missionId, index).Forget();
    }

    private async UniTaskVoid UpdateFindTreasureTask(int missionId, int index)
    {
        var data = treasureDatas.Find(x => x.MissionId == missionId);
        if (data != null)
        {
            data.Value[index] = true; // 00100 (bool)
            data.StringValue = treasureConversion.GetBinary(data.Value); // 00100
            data.Count = treasureConversion.BinaryCheck(data.StringValue).Item1; // 1

            if (data.Count >= 5)
            {
                MissionManager.instance.MissionSubject.Completed(missionId);
            }
            else
            {
                MissionManager.instance.MissionSubject.Normal(missionId);
            }

            foreach (var treasure in treasures)
            {
                treasure.Setting(missionId, data.Value);
            }

            bool finish = false;
            var progress = treasureConversion.StringToBinary(data.StringValue);
            DatabaseConnector.instance.SendMissionProgressData(() => finish = true, null, missionId, progress);

            await UniTask.WaitUntil(() => finish);
            DatabaseConnector.instance.GetMemberMissionData(null, null);
        }
    }

    public void FinishTreasure(QuizData quizData, Action callback)
    {
        FinishTreasureTask(quizData.quiz_type, callback).Forget();
    }

    private async UniTaskVoid FinishTreasureTask(string treasureType, Action callback)
    {
        var data = treasureDatas.Find(x => x.TreasureTitle == treasureType);
        if (data != null)
        {
            data.IsFinish = true;
            MissionManager.instance.MissionSubject.Finished(data.MissionId);

            bool finish = false;
            DatabaseConnector.instance.SendMissionProgressData(() => finish = true, null, data.MissionId, -1);

            await UniTask.WaitUntil(() => finish);
            DatabaseConnector.instance.GetMemberMissionData(null, null);
        }
    }

    public (int, bool[], string) GetCurrentProgress(int missionId)
    {
        var data = treasureDatas.Find(x => x.MissionId == missionId);
        if (data != null)
        {
            return (data.Count, data.Value, data.StringValue);
        }
        else
        {
            return (0, Enumerable.Repeat(false, 5).ToArray(), "00000");
        }
    }
}