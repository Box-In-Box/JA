using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gongju.Web;

public class MissionSubject : MissionObserver
{
    [field : SerializeField, ReadOnly] public List<MissionObserver> normalMissionObserver { get; set; } = new List<MissionObserver>();
    [field : SerializeField, ReadOnly] public List<MissionObserver> courseMissionObserver { get; set; } = new List<MissionObserver>();
    [field: SerializeField, ReadOnly] public List<MissionObserver> TreasureMissionObserver { get; set; } = new List<MissionObserver>();

    public void AddObserver(MissionObserver missionObserver)
    {
        if (missionObserver is NormalMission) normalMissionObserver.Add(missionObserver);

        if (missionObserver is CourseMission) courseMissionObserver.Add(missionObserver);

        if (missionObserver is TreasureMission) TreasureMissionObserver.Add(missionObserver);
    }

    public void RemoveObserver(MissionObserver missionObserver)
    {
        if (missionObserver is NormalMission)
        {
            if (normalMissionObserver.Contains(missionObserver))
            {
                normalMissionObserver.Remove(missionObserver);
            }
        }

        if (missionObserver is CourseMission)
        {
            if (courseMissionObserver.Contains(missionObserver))
            {
                courseMissionObserver.Remove(missionObserver);
            }
        }

        if (missionObserver is TreasureMission)
        {
            if (TreasureMissionObserver.Contains(missionObserver))
            {
                TreasureMissionObserver.Remove(missionObserver);
            }
        }
    }

    public override void Normal(int missionId)
    {
        foreach (var observer in normalMissionObserver) observer.Normal(missionId);

        foreach (var observer in courseMissionObserver) observer.Normal(missionId);

        foreach (var observer in TreasureMissionObserver) observer.Normal(missionId);
    }

    public override void Selected(int missionId)
    {
        foreach (var observer in normalMissionObserver) observer.Selected(missionId);

        foreach (var observer in courseMissionObserver) observer.Selected(missionId);

        foreach (var observer in TreasureMissionObserver) observer.Selected(missionId);
    }

    public override void Completed(int missionId)
    {
        foreach (var observer in normalMissionObserver) observer.Completed(missionId);

        foreach (var observer in courseMissionObserver) observer.Completed(missionId);

        foreach (var observer in TreasureMissionObserver) observer.Completed(missionId);
    }

    public override void Finished(int missionId)
    {
        foreach (var observer in normalMissionObserver) observer.Finished(missionId);

        foreach (var observer in courseMissionObserver) observer.Finished(missionId);

        foreach (var observer in TreasureMissionObserver) observer.Finished(missionId);
    }
}