using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Web;

public class CourseMission : MissionObserver
{
    [field : Title("[ Course Mission ]")]
    [field : SerializeField] public MissionData missionData { get; set; }
    [field : SerializeField] public MissionPopup missionPopup { get; set; }
    [SerializeField] private Button button;

    [Title("[ Background ]")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite backgroundNormalSprite; // 선택 전
    [SerializeField] private Sprite backgroundSelectedSprite; // 선택 후
    [SerializeField] private Sprite backgroundCompletedSprite; // 보상
    [SerializeField] private Image maroonImage; // 배경 뒤 밤 이미지

    [Title("[ Title ]")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Color titleTextNormalColor; // 완료 전
    [SerializeField] private Color titleTextSelectedColor; // 완료 후

    [field: Title("[ Course ]")]
    [field: SerializeField] public Image courseImage { get; set; }
    [SerializeField] private TMP_Text courseText;


    [Title("[ Completed ]")]
    [SerializeField] private RectTransform completedPanel; // 완료 보상 전

    [Title("[ Finish ]")]
    [SerializeField] private RectTransform finishPanel; // 완료 보상 후
    [SerializeField] private Button finishButton;

    [field: Title("[ Reward ]")]
    [SerializeField] private RectTransform rewardPanel;
    [field : SerializeField] public List<Reward> rewardList { get; set; }
    
    private void OnEnable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.missionSubject.AddObserver(this);
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.missionSubject.RemoveObserver(this);
        }
    }
    
    private void Start()
    {
        titleText.text = missionData.mission_title;
        courseText.text = missionData.mission_sub;
        rewardList = new List<Reward>(Reward.GetRewards(missionData.rewards, RewardUISize.M, rewardPanel));

        var progressData = MissionManager.instance.GetMissionProgress(missionData.mission_id);
        string routesType = MissionManager.instance.GetMissionRoutesType(missionData.mission_id);

        if (progressData != null) {
            if (routesType == "finished")
            { // 완료 (보상 후)
                Finished(missionData.mission_id);
            }
            else {
                if (routesType == "completed")
                { // 완료 (보상 전)
                    Completed(missionData.mission_id);
                }
                else if (missionData.mission_id == DatabaseConnector.instance.memberData.current_mission_idx)
                { // 미션 중
                    Selected(missionData.mission_id);
                }
                else
                { // 기본 (진행 전)
                    Normal(missionData.mission_id);
                }
            }
        }
        else { // 현재 미션 추가
            DatabaseConnector.instance.SendMissionProgressData(null, null, missionData.mission_id, 0);
            Normal(missionData.mission_id);
        }
    }

    /// <summary>
    /// 진행 전 (0)
    /// </summary>
    [Button]
    public override void Normal(int missionId)
    {
        if (missionData.mission_id == missionId)
        {
            backgroundImage.sprite = backgroundNormalSprite;
            titleText.color = titleTextNormalColor;
            finishPanel.gameObject.SetActive(false);
            completedPanel.gameObject.SetActive(false);
            maroonImage.gameObject.SetActive(false);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.SelectCourseMission(this));
        }
    }

    /// <summary>
    /// 진행 중  (1 ~ n - 2)
    /// </summary>
    [Button]
    public override void Selected(int missionId)
    {
        if (missionData.mission_id == missionId)
        {
            backgroundImage.sprite = backgroundSelectedSprite;
            titleText.color = titleTextSelectedColor;
            finishPanel.gameObject.SetActive(false);
            completedPanel.gameObject.SetActive(false);
            maroonImage.gameObject.SetActive(true);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.SetNormalCourseMission(this));
        }
    }
    /// <summary>
    /// 보상 전  (n - 1)
    /// </summary>
    [Button]
    public override void Completed(int missionId)
    {
        if (missionData.mission_id == missionId)
        {
            Normal(missionId);
            backgroundImage.sprite = backgroundCompletedSprite;
            completedPanel.gameObject.SetActive(true);
            finishPanel.gameObject.SetActive(false);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.CompletedCourseMission(this, null));
        }
    }

    /// <summary>
    /// 보상 후 (-1)
    /// </summary>
    [Button]
    public override void Finished(int missionId)
    {
        if (missionData.mission_id == missionId)
        {
            Normal(missionId);
            completedPanel.gameObject.SetActive(false);
            finishPanel.gameObject.SetActive(true);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.FinishedCourseMission(this));
        }
    }
}