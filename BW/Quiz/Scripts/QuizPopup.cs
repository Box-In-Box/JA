using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Sirenix.OdinInspector;
using Gongju.Web;
using Photon.Voice;
using System.Linq;

public class QuizPopup : Popup
{
    private QuizView quizView;
    [Title("[ Quiz ]")]
    [field: SerializeField] public QuizData quizdata { get; set; }
    [field : SerializeField] public QuizAnswer selectedAnswer { get; set; }
    [field: SerializeField] public List<Reward> rewardList { get; set; }
    [field: SerializeField] public bool isMission { get; set; }

    public override void Awake()
    {
        base.Awake();
        quizView = GetComponent<QuizView>();
    }

    public void Quiz(string typeName)
    {
        quizdata = GetQuizData(typeName);

        if (quizdata == null) {
            PopupManager.instance.Close(this, true);
            PopupManager.instance.Popup("해당 문제가 없습니다.");
            return;
        }

        quizView.NotUseImageSetting(); // 임시

        rewardList = new List<Reward>(Reward.GetRewards(quizdata.rewards, RewardUISize.S, quizView.rewardPanel));

        quizView.ViewSetting(quizdata);
    }

    public void MissionQuiz(string typeName)
    {
        isMission = true;
        Quiz(typeName);
    }

    public void AnswerCheck()
    {
        if (selectedAnswer == null) {
            PopupManager.instance.Popup("정답을 선택 후 확인해주세요.");
        }
        else {
            if (selectedAnswer.isCorrectAnswer) {
                Success();
            }
            else {
                Fail();
            }
        }
    }

    private void Success()
    {
        Reward.Receive(rewardList, () => PopupManager.instance.Close(this));

        if (isMission) {
            MissionManager.instance.courseMissionManager.UpdateMissionProgress(quizdata, null);
        }
    }

    private void Fail()
    {

    }

#region Get
    private QuizData GetQuizData(string typeName)
    {
        return RandomQuiz(GetTypeQuiz(typeName));
    }

    public QuizData[] GetTypeQuiz(string typeName)
    {
        return DatabaseConnector.instance.quizData.Where(x => x.quiz_type == typeName).ToArray();
    }

    public QuizData RandomQuiz(QuizData[] quizDatas)
    {   
        if (quizDatas.Length == 0) return null;

        int randIndex = Random.Range(0, quizDatas.Count());
        return quizDatas[randIndex];
    }
#endregion
}