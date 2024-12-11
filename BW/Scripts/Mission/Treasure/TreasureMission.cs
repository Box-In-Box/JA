using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreasureMission : MissionObserver
{
    [field: Title("[ Treasure Mission ]")]
    [field: SerializeField] public string missionTitle { get; set; }
    [field: SerializeField] public MissionData missionData { get; set; }
    [field: SerializeField] public MissionPopup missionPopup { get; set; }

    [Title("[ Image ]")]
    [SerializeField] private Image[] pieceImage_On_Parts; // 부분 완성 파츠 이미지 
    [SerializeField] private Image completedImage; // 완성 이미지

    [Title("[ Text ]")]
    [SerializeField] private TMP_Text progresstext;
    [SerializeField] private TMP_Text descriptiontext;

    [Title("[ Button ]")]
    [SerializeField] private Button quizButton;

    [field: Title("[ Reward ]")]
    [SerializeField] private RectTransform rewardPanel;
    [field: SerializeField] public List<Reward> rewardList { get; set; }

    private void OnEnable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.MissionSubject.AddObserver(this);
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.MissionSubject.RemoveObserver(this);
        }
    }

    private void Start()
    {
        if (GameManager.instance.isGuest) return;

        descriptiontext.text = missionData.mission_sub;
        rewardList = new List<Reward>(Reward.GetRewards(missionData.rewards, rewardPanel));

        var progressData = MissionManager.instance.GetMissionProgress(missionData.mission_id);
        string routesType = MissionManager.instance.GetMissionRouteType(missionData.mission_id);
        var progress = MissionManager.instance.TreasureMissionManager.GetCurrentProgress(missionData.mission_id);

        if (progressData != null)
        {
            if (routesType == "finished")
            { // 완료 (보상 후)
                Finished(missionData.mission_id);
            }
            else
            {
                if (progress.Item1 == 5)
                { // 완료 (보상 전)
                    Completed(missionData.mission_id);
                }
                else
                { // 기본 (진행 전, 진행 중)
                    Normal(missionData.mission_id);
                }
            }
        }
        else
        {
            Normal(missionData.mission_id);
        }
    }

    [Button]
    public override void Normal(int missionId)
    {
        if (missionData.mission_id == missionId)
        {
            var progress = MissionManager.instance.TreasureMissionManager.GetCurrentProgress(missionId);

            progresstext.text = missionData.mission_title + $" ({progress.Item1}/5)";
            for (int i = 0; i < progress.Item2.Length; ++i)
            {
                pieceImage_On_Parts[i].gameObject.SetActive(progress.Item2[i] == true ? true : false);
            }
            completedImage.gameObject.SetActive(false);
            descriptiontext.gameObject.SetActive(true);
            quizButton.gameObject.SetActive(false);
        }  
    }

    [Button]
    public override void Completed(int missionId)
    {
        if (missionData.mission_id == missionId)
        {
            progresstext.text = missionData.mission_title;
            completedImage.gameObject.SetActive(true);
            descriptiontext.gameObject.SetActive(false);
            quizButton.gameObject.SetActive(true);
            quizButton.onClick.RemoveAllListeners();
            quizButton.onClick.AddListener(() =>
            {
                quizButton.interactable = false;
                PopupManager.instance.Close<MissionPopup>(true);
                var quizPopup = PopupManager.instance.Open<QuizPopup>(PopupManager.instance.CommonPrefab.QuizPopup);
                quizPopup.TreawureQuiz(missionTitle);
            });
        }
    }

    [Button]
    public override void Finished(int missionId)
    {
        if (missionData.mission_id == missionId)
        {
            progresstext.text = missionData.mission_title;
            completedImage.gameObject.SetActive(true);
            descriptiontext.gameObject.SetActive(true);
            quizButton.gameObject.SetActive(false);
        }
    }

    public override void Selected(int missionId) { } // 안 씀
}