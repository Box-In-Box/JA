using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

[System.Serializable]
public class MissionTreasure
{
    [field: SerializeField] public int missionId { get; set; }
    [field: SerializeField] public string treasureTitle { get; set; }
    [field: SerializeField] public int count { get; set; }
    [field: SerializeField] public bool[] value { get; set; }

    public MissionTreasure(int missionId, string treasureTitle, int count, bool[] value)
    {
        this.missionId = missionId;
        this.treasureTitle = treasureTitle;
        this.count = count;
        this.value = value;
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
        var progress = GetProgress(missionId);
        var treasureData = treasureDatas.Find(x => x.missionId == missionId);
        if (treasureData == null)
        {
            treasureDatas.Add(new MissionTreasure(missionId, title, progress.Item1, progress.Item2));
        }
        else
        {
            treasureData.count = progress.Item1;
            treasureData.value = progress.Item2;

            if (progress.Item1 == 5)
            {
                MissionManager.instance.missionSubject.Completed(missionId);
            }
            else if (progressData.missionProgress == -1)
            {
                MissionManager.instance.missionSubject.Finished(missionId);
            }
            else
            {
                MissionManager.instance.missionSubject.Normal(missionId);
            }
        }
        foreach (var treasure in treasures)
        {
            treasure.Setting(missionId, treasureData.value);
        }
    }

    [Button]
    public void FindTreasure(int missionId, int index)
    {
        var progressData = MissionManager.instance.GetMissionProgress(missionId);
        var binary = treasureConversion.IntToBinaryString(progressData.missionProgress);
        var binaryString = treasureConversion.UpdateBinary(binary, index);
        var binaryInt = treasureConversion.StringToBinary(binaryString);

        MissionManager.instance.UpdateMissionProgress(missionId, binaryInt, () =>
        {
            TreasureSetting(missionId);
        });
    }

    public (int, bool[]) GetProgress(int missionId)
    {
        var progressData = MissionManager.instance.GetMissionProgress(missionId);
        int currentCount = 0;
        bool[] bools = Enumerable.Repeat(false, 5).ToArray();
        if (progressData != null)
        {
            if (progressData.missionProgress == -1)
            {
                currentCount = 5;
                bools = Enumerable.Repeat(true, 5).ToArray();
            }
            else
            {
                currentCount = treasureConversion.GetProgress(progressData.missionProgress).Item1;
                bools = treasureConversion.GetProgress(progressData.missionProgress).Item2;
            }
        }
        return (currentCount, bools);
    }
}