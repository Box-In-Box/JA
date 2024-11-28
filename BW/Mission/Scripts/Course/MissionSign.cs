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
            MissionManager.instance.courseMissionManager.MissionSignSubject.AddObserver(this);
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance && DatabaseConnector.instance.memberIDRetrieved)
        {
            MissionManager.instance.courseMissionManager.MissionSignSubject.RemoveObserver(this);
        }
    }

    private IEnumerator Start()
    {
        if (DatabaseConnector.instance.memberIDRetrieved)
        {
            yield return new WaitUntil(() => worldSign.View != null);
            Sign(MissionManager.instance.GetMissionRoutesType(MissionManager.instance.courseMissionManager.CurrentCourceMission.mission_id));
        }
        worldSign.View.Button.onClick.AddListener(() => SignPopup());
    }

    public override void Sign(string typeName)
    {
        if (worldSign.SignName == typeName) // Quiz 표시
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
        worldSign.View.NormalSign();
        isMission = false;
    }

    public void QuizSign()
    {
        worldSign.View.TitleSign();
        worldSign.View.Title.text = "Quiz";
        isMission = true;
    }

    private void SignPopup()
    {
        var popup = PopupManager.instance.Open<SignPopup>();
        var popupView = popup.GetComponent<SignPopupView>();

        popupView.SetQuizButton(isMission, () => Quiz());
    }

    public void Quiz()
    {
        if (isMission)
        {
            PopupManager.instance.Close<SignPopup>(true);
            var quizPopup = PopupManager.instance.Open<QuizPopup>();
            quizPopup.MissionQuiz(worldSign.SignName);
        }
        else
        {
            PopupManager.instance.Close<SignPopup>(false);
        }
    }
}