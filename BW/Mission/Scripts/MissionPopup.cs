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
        for (int i = 0; i < 3; ++i) { // 일간, 주간, 코스
            var boxContent = missionGroup.toggleBox[i].content;
            content[i] = boxContent.GetComponentInChildren<ScrollRect>().content;
        }
        // 보물 찾기 (예외)
        var treasureToggle = missionGroup.toggleBox[3].content.GetComponentInChildren<UIToggleGroup>();
        var treasureContent = treasureToggle.toggleBox.Select(x => x.content.GetComponent<TreasureMission>()).ToArray();

        foreach (var missionData in Gongju.Web.DatabaseConnector.instance.missionData) {
            switch (missionData.mission_type) {
                case "daily" :
                    var dailyMission = Instantiate(Resources.Load<NormalMission>("Mission"), content[0]);
                    dailyMission.MissionData = missionData;
                    dailyMission.MissionPopup = this;
                    break;
                case "weekly" :
                    var weeklyMission = Instantiate(Resources.Load<NormalMission>("Mission"), content[1]);
                    weeklyMission.MissionData = missionData;
                    weeklyMission.MissionPopup = this;
                    break;
                case "course" :
                    var courseMission = Instantiate(Resources.Load<CourseMission>("CourseMission"), content[2]);
                    courseMission.MissionData = missionData;
                    courseMission.MissionPopup = this;
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