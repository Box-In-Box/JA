using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Web;
using Cysharp.Threading.Tasks;
using static NormalMissionManager;

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

    [field: Title("[ Normal Mission ]")]
    [field: SerializeField] public MissionData MissionData { get; set; }
    [field: SerializeField] public MissionPopup MissionPopup { get; set; }
    [field: SerializeField] public Button Button { get; set; }

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
    [field: SerializeField] public bool IsCompleted { get; set; } = false;
    
    [Title("[ Reward ]")]
    [SerializeField] private RectTransform rewardPanel;
    [field: SerializeField] public List<Reward> rewardList { get; set; }

    private void Start()
    {
        titleText.text = MissionData.mission_title;
        rewardList = new List<Reward>(Reward.GetRewards(MissionData.rewards, rewardPanel));

        MissionUpdate();
    }
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

    public void MissionUpdate()
    {
        var memberMissionData = DatabaseConnector.instance.memberMissionData;
        var data = Array.Find(memberMissionData, x => x.missionIdx == MissionData.mission_id);

        if (data != null)
        {
            int progress = 0;
            if (MissionManager.instance.GetMissionRoute(MissionData.mission_id) == -1)
            {
                progress = -1;
            }
            else
            {
                progress = MissionManager.instance.NormalMissionManager.GetMissionProgress(MissionData.mission_id);
            }
            if (progress == -1) // 보상 후
            {
                Finished(MissionData.mission_id);
            }
            else
            {
                stateText.text = progress + " / " + MissionData.mission_sub;

                if (progress >= int.Parse(MissionData.mission_sub)) // 보상 ~ 완료 전
                {
                    stateImage.fillAmount = 1f;
                    Completed(MissionData.mission_id);
                }
                else // 진행 전 ~ 보상 전
                {
                    stateImage.fillAmount = progress == 0 ? 0f : progress / float.Parse(MissionData.mission_sub);
                    Normal(MissionData.mission_id);
                }
            }
        }
        else
        { // 현재 미션 추가
            DatabaseConnector.instance.SendMissionProgressData(null, null, MissionData.mission_id, 0);
            stateText.text = 0f + " / " + MissionData.mission_sub;
            stateImage.fillAmount = 0f;
            Normal(MissionData.mission_id);
        }

        if (MissionManager.instance.NormalMissionManager.GetNormalMissionRouteType(MissionData.mission_id) == "clear_all")
        {
            if (!IsCompleted)
            {
                AllMission().Forget();
            }
        }
    }

    private async UniTaskVoid AllMission()
    {
        await UniTask.NextFrame();

        bool isAllCompleted = true;
        for (int i = 0; i < transform.parent.childCount; ++i)
        {
            transform.parent.GetChild(i).TryGetComponent<NormalMission>(out NormalMission child);
            if (child == this) continue;

            if (!child.IsCompleted)
            {
                isAllCompleted = false;
                break;
            }
        }

        if (isAllCompleted)
        {
            Completed(MissionData.mission_id);
        }
    }

    [Button]
    public override void Normal(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            backgroundImage.sprite = backgroundSprite.normalSprite;
            stateImage.sprite = stateSprite.normalSprite;
            completeCheckImage.gameObject.SetActive(false);
            completedPanel.gameObject.SetActive(false);

            stateText.color = textColor.normalColor;
            stateText.text = MissionManager.instance.NormalMissionManager.GetMissionProgress(missionId) + " / " + MissionData.mission_sub;
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => MissionManager.instance.NormalMissionManager.NormalMission(this, null));
            Button.interactable = true;
            IsCompleted = false;
        }
    }

    [Button]
    public override void Selected(int missionId) => Normal(missionId); // 자동 진행중

    [Button]
    public override void Completed(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            backgroundImage.sprite = backgroundSprite.completedSprite;
            stateImage.sprite = stateSprite.completedSprite;
            stateImage.fillAmount = 1f;
            completeCheckImage.gameObject.SetActive(true);
            completedPanel.gameObject.SetActive(false);

            stateText.color = textColor.completedColor;
            stateText.text = "보상받기";
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() =>
            {
                MissionManager.instance.NormalMissionManager.CompletedMission(this, null);
                Button.onClick.RemoveAllListeners();
                Button.interactable = false;
            });
            IsCompleted = true;
        }
    }

    [Button]
    public override void Finished(int missionId)
    {
        if (MissionData.mission_id == missionId)
        {
            Completed(missionId);
            completedPanel.gameObject.SetActive(true);

            Button.onClick.RemoveAllListeners();
            Button.interactable = false;
        }
    }
}