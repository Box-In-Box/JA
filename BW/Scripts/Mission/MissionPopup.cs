using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using Gongju.Web;
using static UIToggleGroup;

public class MissionPopup : Popup
{
    [field : Title("[ Mission ]")]
    [field : SerializeField] public UIToggleGroup missionGroup { get; set; }

    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject NormalMissionPrefab { get; private set; }
    [field: SerializeField] public GameObject CourseMissionPrefab { get; private set; }

    private void Start()
    {
        if (MissionManager.instance.IsMissionData && MissionManager.instance.IsMemberMissionData)
        {
            // 보물 찾기 (예외)
            var treasureToggle = missionGroup.toggleBox[3].content.GetComponentInChildren<UIToggleGroup>();
            var treasureContent = treasureToggle.toggleBox.Select(x => x.content.GetComponent<TreasureMission>()).ToArray();

            foreach (var missionData in Gongju.Web.DatabaseConnector.instance.missionData)
            {
                TreasureMission treasureMission = Array.Find(treasureContent, x => x.missionTitle == missionData.mission_title);
                if (treasureMission)
                {
                    treasureMission.missionData = missionData;
                    treasureMission.missionPopup = this;
                }
            }

            missionGroup.toggleBox[0].toggle.onValueChanged.AddListener((value) => MissionSetting(value, "daily"));
            missionGroup.toggleBox[1].toggle.onValueChanged.AddListener((value) => MissionSetting(value, "weekly"));
            missionGroup.toggleBox[2].toggle.onValueChanged.AddListener((value) => MissionSetting(value, "course"));
            missionGroup.toggleBox[3].toggle.onValueChanged.AddListener((value) => MissionSetting(value, ""));

            missionGroup.toggleBox[0].toggle.onValueChanged.Invoke(true);
        }
    }

    public void MissionSetting(bool toggle, string missionType)
    {
        if (!toggle) return;

        MissionSetting(missionType);
    }

    private void MissionSetting(string missionType)
    {
        Transform[] content = new Transform[3];  
        for (int i = 0; i < 3; ++i) { // 일간, 주간, 코스
            var boxContent = missionGroup.toggleBox[i].content;
            content[i] = boxContent.GetComponentInChildren<ScrollRect>().content;
            foreach (Transform child in content[i])
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var missionData in Gongju.Web.DatabaseConnector.instance.missionData) {
            if (missionType == missionData.mission_type)
            {
                switch (missionData.mission_type)
                {
                    case "daily":
                        Instantiate(NormalMissionPrefab, content[0]).TryGetComponent<NormalMission>(out NormalMission dailyMission);
                        dailyMission.MissionData = missionData;
                        dailyMission.MissionPopup = this;
                        break;
                    case "weekly":
                        Instantiate(NormalMissionPrefab, content[1]).TryGetComponent<NormalMission>(out NormalMission weeklyMission);
                        weeklyMission.MissionData = missionData;
                        weeklyMission.MissionPopup = this;
                        break;
                    case "course":
                        Instantiate(CourseMissionPrefab, content[2]).TryGetComponent<CourseMission>(out CourseMission courseMission);
                        courseMission.MissionData = missionData;
                        courseMission.MissionPopup = this;
                        break;
                }
            }
        }
    }
}