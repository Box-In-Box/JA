using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSignSubject : MissionSignObserver
{
    [field : SerializeField, ReadOnly] public List<MissionSignObserver> missionSignObserver { get; set; } = new List<MissionSignObserver>();

    public void AddObserver(MissionSignObserver observer)
    {
        if (observer is MissionSignObserver) missionSignObserver.Add(observer);
    }

    public void RemoveObserver(MissionSignObserver observer)
    {
        if (observer is MissionSignObserver)
        {
            if (missionSignObserver.Contains(observer))
            {
                missionSignObserver.Remove(observer);
            }
        }
    }

    public override void Sign(string typeName)
    {
        foreach (var observer in missionSignObserver) observer.Sign(typeName);
    }
}