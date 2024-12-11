using Gongju.Web;
using Photon.Pun.Demo.Procedural;
using Sirenix.OdinInspector;
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
        public int routeId;
        public MissionNPC missionNPC;
    }

    [field: Title("[ Prefabs ]")]
    [field: SerializeField] public GameObject QuizPopup_GongjuPrefab { get; private set; }

    [SerializeField] private Color gatewayChatBubbleColor;
    [SerializeField] private List<Course> missionGateways = new List<Course>();
    [SerializeField, ReadOnly] private bool isMission = false;

    private void OnEnable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.CourseMissionManager.MissionGateway = this;
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.CourseMissionManager.MissionGateway = null;
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        if (!GameManager.instance.isGuest) {
            yield return new WaitUntil(() => MissionManager.instance.IsMissionInit);
            int currentId = DatabaseConnector.instance.memberData.current_mission_idx;
            CourseGateWay(currentId);
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

    public void CourseGateWay(int missionId)
    {
        var missionProgress = MissionManager.instance.GetMissionProgress(missionId)?.missionProgress;
        var routeId = MissionManager.instance.GetMissionRoute(missionId);
        var routesType = MissionManager.instance.GetMissionRouteType(missionId);

        foreach (var course in missionGateways)
        {
            if (missionProgress >= 0 && course.missionId == missionId)
            {
                foreach (var gateway in course.missionCourseList)
                {
                    gateway.missionNPC.gameObject.SetActive(true);
                    var chatBubbleController = (gateway.missionNPC.TalkNPC.HudTarget.HudUI as HudUI_NPC).ChatBubbleController;
                    chatBubbleController.RemoveAllListener();
                    chatBubbleController.ChatBubbleColor();
                    chatBubbleController.IsClickable = false;
                    if (gateway.routeId == routeId)
                    {
                        gateway.missionNPC.TalkNPC.AlwaysTalkData = gateway.missionNPC.missionTalk;
                    }
                    else if (gateway.routeId > routeId)
                    {
                        gateway.missionNPC.TalkNPC.AlwaysTalkData = gateway.missionNPC.missionBeforeTalk;
                    }
                    else if (gateway.routeId < routeId)
                    {
                        gateway.missionNPC.TalkNPC.AlwaysTalkData = gateway.missionNPC.missionAfterTalk;
                    }
                    gateway.missionNPC.TalkNPC.TalkReceiver.ChatText(gateway.missionNPC.TalkNPC.AlwaysTalkData);
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

        var missionCourse = missionGateways.Find(x => x.missionId == missionId);
        var missionGateway = missionCourse?.missionCourseList.Find(x => x.routeId == routeId);

        if (missionGateway != null)
        {
            var chatBubbleController = (missionGateway.missionNPC.TalkNPC.HudTarget.HudUI as HudUI_NPC).ChatBubbleController;
            chatBubbleController.AddListener(() => Quiz(routesType));
            chatBubbleController.ChatBubbleColor(gatewayChatBubbleColor);
            chatBubbleController.IsClickable = true;
            isMission = true;
        }
    }

    public void Quiz(string gatewayType)
    {
        if (isMission)
        {
            PopupManager.instance.Close<SignPopup>(true);
            var quizPopup = PopupManager.instance.Open<QuizPopup_Gongju>(QuizPopup_GongjuPrefab);
            quizPopup.MissionQuiz(gatewayType);
        }
    }
}