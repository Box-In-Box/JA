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
    [Serializable]
    public struct BackgroundSprite
    {
        public Sprite normal; // 선택 전
        public Sprite selected; // 선택 후
        public Sprite completed; // 보상
    }

    [Serializable]
    public struct TextColor
    {
        public Color normal;
        public Color selected;
        public Color completed;
    }

    [field : Title("[ Course Mission ]")]
    [field : SerializeField] public MissionData MissionData { get; set; }
    [field : SerializeField] public MissionPopup MissionPopup { get; set; }

    [field: Title("[ Button ]")]
    [field: SerializeField] public Button Button { get; set; }
    [field: SerializeField] public TMP_Text ButtonText { get; set; }
    [field: SerializeField] public Button Button2 { get; set; }
    [field: SerializeField] public TMP_Text Button2Text { get; set; }

    [Title("[ Background ]")]
    [SerializeField] private BackgroundSprite backgroundSprite;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image maroonImage; // 배경 뒤 밤 이미지

    [Title("[ Title ]")]
    [SerializeField] private TextColor textColor;
    [SerializeField] private TMP_Text titleText;

    [field: Title("[ Course ]")]
    [field: SerializeField] public Image courseImage { get; set; }
    [SerializeField] private TMP_Text courseText;

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
        titleText.text = MissionData.mission_title;
        courseText.text = MissionData.mission_sub;
        rewardList = new List<Reward>(Reward.GetRewards(MissionData.rewards, rewardPanel));

        var progressData = MissionManager.instance.GetMissionProgress(MissionData.mission_id);
        string routesType = MissionManager.instance.GetMissionRoutesType(MissionData.mission_id);

        if (progressData != null) {
            if (routesType == "finished")
            { // 완료 (보상 후)
                Finished(MissionData.mission_id);
            }
            else {
                if (routesType == "completed")
                { // 완료 (보상 전)
                    Completed(MissionData.mission_id);
                }
                else if (MissionData.mission_id == DatabaseConnector.instance.memberData.current_mission_idx)
                { // 미션 중
                    Selected(MissionData.mission_id);
                }
                else
                { // 기본 (진행 전)
                    Normal(MissionData.mission_id);
                }
            }
        }
        else { // 현재 미션 추가
            DatabaseConnector.instance.SendMissionProgressData(null, null, MissionData.mission_id, 0);
            Normal(MissionData.mission_id);
        }
    }

    /// <summary>
    /// 진행 전 (0)
    /// </summary>
    [Button]
    public override void Normal(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            backgroundImage.sprite = backgroundSprite.normal;
            titleText.color = textColor.normal;
            finishPanel.gameObject.SetActive(false);
            maroonImage.gameObject.SetActive(false);

            ButtonText.text = "시작하기";
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.SelectCourseMission(this));
        }
    }

    /// <summary>
    /// 진행 중  (1 ~ n - 2)
    /// </summary>
    [Button]
    public override void Selected(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            backgroundImage.sprite = backgroundSprite.selected;
            titleText.color = textColor.selected;
            finishPanel.gameObject.SetActive(false);
            maroonImage.gameObject.SetActive(true);

            ButtonText.text = "포기하기";
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.SetNormalCourseMission(this));
        }
    }
    /// <summary>
    /// 보상 전  (n - 1)
    /// </summary>
    [Button]
    public override void Completed(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            Normal(missionId);
            backgroundImage.sprite = backgroundSprite.completed;
            titleText.color = textColor.completed;
            finishPanel.gameObject.SetActive(false);

            ButtonText.text = "보상받기";
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.CompletedCourseMission(this, null));
        }
    }

    /// <summary>
    /// 보상 후 (-1)
    /// </summary>
    [Button]
    public override void Finished(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            Normal(missionId);
            finishPanel.gameObject.SetActive(true);

            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => MissionManager.instance.courseMissionManager.FinishedCourseMission(this));
        }
    }
}