using System.Collections;
using System.Collections.Generic;
using Gongju.Web;
using Photon.Pun.Demo.Procedural;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionSign : MissionSignObserver
{
    [field: Title("[ MissionSign ]")]
    [field: SerializeField] public WorldSign WorldSign { get; private set; }

    [field: Title("[ Prefabs ]")]
    [SerializeField] private GameObject signPopupPrefab;

    private bool isMission = false;

    private void OnEnable()
    {
        if (MissionManager.instance && DatabaseConnector.instance.memberIDRetrieved)
        {
            MissionManager.instance.CourseMissionManager.MissionSignSubject.AddObserver(this);
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance && DatabaseConnector.instance.memberIDRetrieved)
        {
            MissionManager.instance.CourseMissionManager.MissionSignSubject.RemoveObserver(this);
        }
    }

    private IEnumerator Start()
    {
        if (DatabaseConnector.instance.memberIDRetrieved)
        {
            yield return new WaitUntil(() => WorldSign.View != null);
            Sign(MissionManager.instance.GetMissionRouteType(MissionManager.instance.CourseMissionManager.CurrentCourceMission.mission_id));
        }
        WorldSign.View.Button.onClick.AddListener(() => SignPopup());
    }

    public override void Sign(string typeName)
    {
        bool routeCheck = false;
        if (MissionManager.instance.CourseMissionManager.CurrentCourceMission.mission_id <= 0)
        {
            NormalSign();
            return;
        }

        for (int i = 0; i < MissionManager.instance.CourseMissionManager.CurrentCourceMission.routes.Length; i++)
        {
            var routeType = MissionManager.instance.CourseMissionManager.CurrentCourceMission.routes[i].key_type;
            if (typeName == routeType)
            {
                if (WorldSign.SignName == routeType)
                {
                    QuizSign();
                    return;
                }
                routeCheck = true;
            }
                
            if (!routeCheck && WorldSign.SignName == routeType)
            {
                StampSign();
                return;
            }
        }
        NormalSign();
    }

    public void NormalSign()
    {
        WorldSign.View.NormalSign();
        isMission = false;
    }

    public void QuizSign()
    {
        WorldSign.View.TitleSign();
        WorldSign.View.Title.text = "Quiz";
        isMission = true;
    }

    public void StampSign()
    {
        WorldSign.View.StampSign();
        isMission = false;
    }

    private void SignPopup()
    {
        var popup = PopupManager.instance.Open<SignPopup>(signPopupPrefab);
        var popupView = popup.GetComponent<SignPopupView>();

        popupView.SetQuizButton(isMission, () => Quiz());
    }

    public void Quiz()
    {
        if (isMission)
        {
            PopupManager.instance.Close<SignPopup>(true);
            var quizPopup = PopupManager.instance.Open<QuizPopup>(PopupManager.instance.CommonPrefab.QuizPopup);
            quizPopup.MissionQuiz(WorldSign.SignName);
        }
        else
        {
            PopupManager.instance.Close<SignPopup>(false);
        }
    }
}