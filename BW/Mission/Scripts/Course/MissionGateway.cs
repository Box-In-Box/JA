using Gongju.Web;
using Photon.Pun.Demo.Procedural;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static MissionGateway;

public class MissionGateway : MonoBehaviour
{
    [System.Serializable]
    public class Course
    {
        public int missionId;
        public List<Gateway> missionCourseList = new List<Gateway>();
    }

    [System.Serializable]
    public class Gateway
    {
        public string routesType;
        public MissionNPC missionNPC;
    }

    [SerializeField] private Color gatewayChatBubbleColor;
    [SerializeField] private List<Course> missionGateways = new List<Course>();
    [SerializeField, ReadOnly] private string gatewayType = "";
    [SerializeField, ReadOnly] private bool isMission = false;

    private void OnEnable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.courseMissionManager.MissionGateway = this;
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.courseMissionManager.MissionGateway = null;
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        if (!GameManager.instance.isGuest) {
            yield return new WaitUntil(() => MissionManager.instance.isMissionInit);
            int currentId = DatabaseConnector.instance.memberData.current_mission_idx;
            CourseGateWay(currentId, MissionManager.instance.GetMissionRoutesType(currentId));
        }
        else
        {
            foreach (var course in missionGateways)
            {
                foreach (var gateway in course.missionCourseList)
                {
                    gateway.missionNPC.gameObject.SetActive(false);
                }
            }
        }
    }

    public void CourseGateWay(int missionId, string routesType)
    {
        foreach (var course in missionGateways)
        {
            if (course.missionId == missionId)
            {
                foreach (var gateway in course.missionCourseList)
                {
                    gateway.missionNPC.gameObject.SetActive(true);
                    var chatBubbleController = (gateway.missionNPC.TalkNPC.HudTarget.HudUI as HudUI_NPC).ChatBubbleController;
                    chatBubbleController.RemoveAllListener();
                    chatBubbleController.ChatBubbleColor();
                    chatBubbleController.IsClickable = false;
                }
            }
            else
            {
                foreach (var gateway in course.missionCourseList)
                {
                    gateway.missionNPC.gameObject.SetActive(false);
                }
            }
        }
        isMission = false; 
        gatewayType = "";

        var missionCourse = missionGateways.Find(x => x.missionId == missionId);
        var missionGateway = missionCourse?.missionCourseList.Find(x => x.routesType == routesType);

        if (missionGateway != null)
        {
            var chatBubbleController = (missionGateway.missionNPC.TalkNPC.HudTarget.HudUI as HudUI_NPC).ChatBubbleController;
            chatBubbleController.AddListener(() => Quiz());
            chatBubbleController.ChatBubbleColor(gatewayChatBubbleColor);
            chatBubbleController.IsClickable = true;
            isMission = true;
            gatewayType = routesType;
        }
    }

    public void Quiz()
    {
        if (isMission)
        {
            PopupManager.instance.Close<SignPopup>(true);
            var quizPopup = PopupManager.instance.Open<QuizPopup_Gongju>();
            quizPopup.MissionQuiz(gatewayType);
        }
    }
}