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
    [field: SerializeField] public QuizData Quizdata { get; set; }
    [field: SerializeField] public QuizAnswer SelectedAnswer { get; set; }
    [field: SerializeField] public List<Reward> RewardList { get; set; }
    [field: SerializeField] public bool IsMission { get; set; }
    [field: SerializeField] public bool IsTreasure { get; set; }

    public override void Awake()
    {
        base.Awake();
        quizView = GetComponent<QuizView>();
    }

    public void Quiz(string typeName)
    {
        Quizdata = GetQuizData(typeName);

        if (Quizdata == null) {
            PopupManager.instance.Close(this, true);
            PopupManager.instance.Popup("해당 문제가 없습니다.");
            return;
        }

        quizView.NotUseImageSetting(); // 임시

        RewardList = new List<Reward>(Reward.GetRewards(Quizdata.rewards, quizView.RewardPanel));

        quizView.ViewSetting(Quizdata);
    }

    public void MissionQuiz(string typeName)
    {
        IsMission = true;
        Quiz(typeName);
    }

    public void TreawureQuiz(string typeName)
    {
        IsTreasure = true;
        Quiz(typeName);
    }

    public void AnswerCheck()
    {
        if (SelectedAnswer == null) {
            PopupManager.instance.Popup("정답을 선택 후 확인해주세요.");
        }
        else {
            if (SelectedAnswer.isCorrectAnswer) {
                Success();
            }
            else {
                Fail();
            }
        }
    }

    private void Success()
    {
        Reward.Receive(RewardList, () => PopupManager.instance.Close(this));

        if (IsMission)
        {
            MissionManager.instance.CourseMissionManager.UpdateQuizMissionProgress(Quizdata, null);
        }
        else if (IsTreasure)
        {
            MissionManager.instance.TreasureMissionManager.FinishTreasure(Quizdata, null);
        }
    }

    private void Fail()
    {
        PopupManager.instance.Close(this);
        PopupManager.instance.Popup("퀴즈미션을 실패했습니다...");
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