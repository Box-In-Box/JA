using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;

public class MissionPopup : Popup
{
    [field : Title("[ Mission ]")]
    [field : SerializeField] public UIToggleGroup missionGroup { get; set; }

    private void Start()
    {
        if (MissionManager.instance.isMissionData && MissionManager.instance.isMemberMissionData) {
            MissionSetting();
        }
    }

    private void MissionSetting()
    {
        Transform[] content = new Transform[3];  
        for (int i = 0; i < 3; ++i) { // �ϰ�, �ְ�, �ڽ�
            var boxContent = missionGroup.toggleBox[i].content;
            content[i] = boxContent.GetComponentInChildren<ScrollRect>().content;
        }
        // ���� ã�� (����)
        var treasureToggle = missionGroup.toggleBox[3].content.GetComponentInChildren<UIToggleGroup>();
        var treasureContent = treasureToggle.toggleBox.Select(x => x.content.GetComponent<TreasureMission>()).ToArray();

        foreach (var missionData in Gongju.Web.DatabaseConnector.instance.missionData) {
            switch (missionData.mission_type) {
                case "daily" :
                    var dailyMission = Instantiate(Resources.Load<NormalMission>("Mission"), content[0]);
                    dailyMission.missionData = missionData;
                    dailyMission.missionPopup = this;
                    break;
                case "weekly" :
                    var weeklyMission = Instantiate(Resources.Load<NormalMission>("Mission"), content[1]);
                    weeklyMission.missionData = missionData;
                    weeklyMission.missionPopup = this;
                    break;
                case "course" :
                    var courseMission = Instantiate(Resources.Load<CourseMission>("CourseMission"), content[2]);
                    courseMission.missionData = missionData;
                    courseMission.missionPopup = this;
                    break;
                case "treasure":
                    TreasureMission treasureMission = Array.Find(treasureContent, x => x.missionTitle == missionData.mission_title);
                    if (treasureMission)
                    {
                        treasureMission.missionData = missionData;
                        treasureMission.missionPopup = this;
                    }
                    break;
            }
        }
    }
}