using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Gongju.Web;

public enum RewardType { exp, coin, }
public enum RewardUISize { S, M, L, }

[Serializable]
public class Reward
{
    [field : SerializeField] public GameObject rewardUIItem { get; set; }
    [field : SerializeField] public RewardUIView rewardUI { get; set; }

    [field : SerializeField] public string rewardType { get; set; }
    [field : SerializeField] public int value { get; set; }

    public Reward(string rewardType, int value, Transform parent, bool isRewardPopup = false)
    {
        if (rewardType == "coin")
        {
            GameObject rewardObj = isRewardPopup ? PopupManager.instance.RewardPrefab.Coin_Ex : PopupManager.instance.RewardPrefab.Coin;
            this.rewardUIItem = GameObject.Instantiate(rewardObj, parent);
        }
        else if (rewardType == "exp")
        {
            GameObject rewardObj = isRewardPopup ? PopupManager.instance.RewardPrefab.Exp_Ex : PopupManager.instance.RewardPrefab.Exp;
            this.rewardUIItem = GameObject.Instantiate(rewardObj, parent);
        }

        this.rewardType = rewardType;
        this.value = value;

        this.rewardUI = rewardUIItem.GetComponent<RewardUIView>();
        rewardUI.ValueText.text = string.Format("{0:#,###}", value);
    }

    /// <summary>
    /// 각 스크립트에서 보상 저장용
    /// </summary>
    public static List<Reward> GetRewards(string rewardString, Transform parent)
    {
        // ex) rewardString = exp100/coin100
        List<Reward> rewardList = new List<Reward>();

        foreach (string reward in rewardString.Split('/')) {
            string rewardType = Regex.Replace(reward, @"\d", "");
            int rewardValue = int.Parse(Regex.Replace(reward, @"\D", ""));
            rewardList.Add(new Reward(rewardType, rewardValue, parent));
        }
        return rewardList;
    }

    /// <summary>
    /// 실제 보상 받을 때 호출
    /// </summary>
    public static void Receive(List<Reward> rewardList, Action okAction = null)
    {
        // 보상 팝업 설정
        var rewardPopup = PopupManager.instance.Open<RewardPopup>(PopupManager.instance.RewardPrefab.RewardPopup);
        rewardPopup.SetReward(rewardList);
        if (okAction != null) rewardPopup.AddOkAction(okAction.Invoke);

        // 보상 (서버 통신)
        ServerReceive(rewardList);
    }

    private static void ServerReceive(List<Reward> rewardList)
    {
        int[] rewards = new int[rewardList.Count];
        int rewardIndex = rewardList.Count - 1;

        foreach (var reward in rewardList) {
            switch (reward.rewardType) {
                case "exp":
                    int currentExp = DatabaseConnector.instance.memberData.experience;
                    rewards[rewardIndex - rewardIndex] = currentExp + reward.value;
                    break;
                case "coin":
                    int currentCoin = DatabaseConnector.instance.memberData.coin;
                    rewards[rewardIndex] = currentCoin + reward.value;
                    break;
            }
        }

        if (rewardList.Count == 1) {
            if (rewardList[0].rewardType == "exp")
            {
                MemberDataPack pack = new MemberDataPack(MemberDataSendType.experience, rewards[0]);
                DatabaseConnector.instance.SendMemberData(null, null, pack);
            }
            else if (rewardList[0].rewardType == "coin")
            {
                MemberDataPack pack = new MemberDataPack(MemberDataSendType.coin, rewards[0]);
                DatabaseConnector.instance.SendMemberData(null, null, pack);
            }
        }
        else if (rewardList.Count == 2) {

            MemberDataPack packEx = new MemberDataPack(MemberDataSendType.experience, rewards[0]);
            MemberDataPack packCoin = new MemberDataPack(MemberDataSendType.experience, rewards[1]);
            DatabaseConnector.instance.SendMemberData(null, null, packEx, packCoin);
        }
    }

    public static string TestRandomReward()
    {
        string rewardString = "";

        int rewardNum = UnityEngine.Random.Range(0, 3);
        bool isExp = rewardNum <= 1;
        bool isCoin = rewardNum >= 1;
        
        if (isExp) {
            rewardString += "exp";
            rewardString += UnityEngine.Random.Range(1, 10000).ToString();
        }
        if (isCoin) {
            if (isExp) rewardString += '/';
            rewardString += "coin";
            rewardString += UnityEngine.Random.Range(1, 10000).ToString();
        }
        return rewardString;
    }
}