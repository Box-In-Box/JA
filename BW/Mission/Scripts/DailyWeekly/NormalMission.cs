using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Web;

public class NormalMission : MissionObserver
{
    [Serializable]
    public struct BackgroundSprite
    {
        public Sprite normalSprite;
        public Sprite completedSprite;
    }

    [Serializable]
    public struct StateSprite
    {
        public Sprite normalSprite;
        public Sprite completedSprite;
    }

    [Serializable]
    public struct TextColor
    {
        public Color normalColor;
        public Color completedColor;
    }
    
    [field : Title("[ Daily Mission ]")]
    [field : SerializeField] public MissionData MissionData { get; set; }
    [field : SerializeField] public MissionPopup MissionPopup { get; set; }
    public Button Button { get; set; }

    [Title("[ Background ]")]
    [SerializeField] private BackgroundSprite backgroundSprite;
    [SerializeField] private Image backgroundImage;
    
    [Title("[ Mission State ]")]
    [SerializeField] private StateSprite stateSprite;
    [SerializeField] private TextColor textColor;
    [SerializeField] private Image stateImage;
    [SerializeField] private Image completeCheckImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text stateText;

    [Title("[ Completed ]")]
    [SerializeField] private RectTransform completedPanel;
    
    [Title("[ Reward ]")]
    [SerializeField] private RectTransform rewardPanel;
    [SerializeField] private List<Reward> rewardList;

    private void OnEnable()
    {
        MissionManager.instance.missionSubject.AddObserver(this);
    }

    private void OnDisable()
    {
        MissionManager.instance.missionSubject.RemoveObserver(this);
    }

    private void Start()
    {
        titleText.text = MissionData.mission_title;
        rewardList = new List<Reward>(Reward.GetRewards(MissionData.rewards, rewardPanel));

        var memberMissionData = DatabaseConnector.instance.memberMissionData;
        var data = Array.Find(memberMissionData, x => x.missionIdx == MissionData.mission_id);

        if (data != null) {
            if (data.missionProgress == -1) { // 보상 후 완료 처리
                SetCompleted(MissionData.mission_id);
            }
            else {
                stateText.text = data.missionProgress + " / " + MissionData.mission_sub;

                if (data.missionProgress == int.Parse(MissionData.mission_sub)) { // 보상 ~ 완료 전
                    stateImage.fillAmount = 1f;
                    SetFinished(MissionData.mission_id);
                }
                else { // 진행 전 ~ 보상 전
                    stateImage.fillAmount = data.missionProgress == 0 ? 0f : float.Parse(MissionData.mission_sub) / data.missionProgress;
                    SetNormal(MissionData.mission_id);
                }
            }
        }
        else { // 현재 미션 추가
            DatabaseConnector.instance.SendMissionProgressData(null, null, MissionData.mission_id, 0);
            stateText.text = 0f + " / " + MissionData.mission_sub;
            stateImage.fillAmount = 0f;
            SetNormal(MissionData.mission_id);
        }
    }

    [Button]
    public void SetNormal(int missionId)
    {
        if (MissionData.mission_id == missionId) {
            backgroundImage.sprite = backgroundSprite.normalSprite;
            stateImage.sprite = stateSprite.normalSprite;
            completeCheckImage.gameObject.SetActive(false);
            stateText.color = textColor.normalColor;
            completedPanel.gameObject.SetActive(false);
        } 
    }

    [Button]
    public void SetCompleted(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            backgroundImage.sprite = backgroundSprite.completedSprite;
            stateImage.sprite = stateSprite.completedSprite;
            completeCheckImage.gameObject.SetActive(true);
            stateText.text = "보상받기";
            stateText.color = textColor.completedColor;
            completedPanel.gameObject.SetActive(false);
        }
        
    }

    [Button]
    public void SetFinished(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            SetFinished(missionId);
            completedPanel.gameObject.SetActive(true);
        }
    }


    private void Success()
    {
        Reward.Receive(rewardList);
    }

    public override void Normal(int missionId) => SetNormal(missionId);

    public override void Selected(int missionId) { } // 자동 진행중

    public override void Completed(int missionId) => SetCompleted(missionId);

    public override void Finished(int missionId) => SetFinished(missionId);
}
