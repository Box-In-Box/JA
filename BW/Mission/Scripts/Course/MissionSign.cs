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
    [SerializeField] private WorldSign worldSign;
    private bool isMission = false;

    private void OnEnable()
    {
        if (MissionManager.instance && DatabaseConnector.instance.memberIDRetrieved)
        {
            MissionManager.instance.courseMissionManager.missionSignSubject.AddObserver(this);
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance && DatabaseConnector.instance.memberIDRetrieved)
        {
            MissionManager.instance.courseMissionManager.missionSignSubject.RemoveObserver(this);
        }
    }

    private IEnumerator Start()
    {
        if (DatabaseConnector.instance.memberIDRetrieved)
        {
            yield return new WaitUntil(() => worldSign.view != null);
            Sign(MissionManager.instance.GetMissionRoutesType(MissionManager.instance.courseMissionManager.currentCourceMission.mission_id));
        }
        worldSign.view.button.onClick.AddListener(() => SignPopup());
    }

    public override void Sign(string typeName)
    {
        if (worldSign.signName == typeName) // Quiz 표시
        {
            QuizSign();
        }
        else // Normal 표시
        {
            NormalSign();
        }
    }

    public void NormalSign()
    {
        worldSign.view.NormalSign();
        isMission = false;
    }

    public void QuizSign()
    {
        worldSign.view.TitleSign();
        worldSign.view.title.text = "Quiz";
        isMission = true;
    }

    private void SignPopup()
    {
        var popup = PopupManager.instance.Open<SignPopup>();
        var popupView = popup.GetComponent<SignPopupView>();

        if (isMission == true)
        {
            popupView.SetQuizButton(true, () => Quiz());
        }
        else
        {
            popupView.SetQuizButton(false);
        }
    }

    public void Quiz()
    {
        if (isMission)
        {
            var quizPopup = PopupManager.instance.Open<QuizPopup>();
            quizPopup.MissionQuiz(worldSign.signName);
        }
    }
}