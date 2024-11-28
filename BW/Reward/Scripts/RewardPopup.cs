using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class RewardPopup : Popup
{
    [Title("[ Reward ]")]
    [SerializeField] private TMP_Text bannertext;
    [SerializeField] private RectTransform rewardPanel;
    [SerializeField] private Button okButton;
    [SerializeField] private List<Reward> rewardList = new List<Reward>();

    private void Start()
    {
        okButton.onClick.AddListener(() => PopupManager.instance.Close(this));
    }

    public void AddOkAction(Action action)
    {
        if (action == null) return;

        okButton.onClick.AddListener(action.Invoke);
    }

    public void SetReward(List<Reward> rewardList)
    {
        foreach (var reward in rewardList) {
            this.rewardList.Add(new Reward(reward.rewardType, reward.value, rewardPanel, true));
        }
        GetReward();
    }

    public void GetReward()
    {
        // 보상 받기
        // 서버 보내기
    }
}