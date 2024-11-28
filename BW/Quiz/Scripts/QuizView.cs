using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Web;
using Sirenix.OdinInspector;
using System.Security.Policy;

public class QuizView : MonoBehaviour
{
    [SerializeField] private QuizPopup quizPopup;
    [field : SerializeField] public RectTransform QuizPopupRect { get; private set; }
    [field : SerializeField] public Image QuizPopupImage { get; private set; }

    [field : Title("[ Question Panel ]")]
    [field : SerializeField] public TMP_Text QuestionText { get; private set; }
    [field : SerializeField] public RectTransform QuistionPagePanel { get; private set; }
    [field : SerializeField] public TMP_Text QuistionPageText { get; private set; }
    [field : SerializeField] public Button PreviousPageButton { get; private set; }
    [field : SerializeField] public Button NextPageButton { get; private set; }
    [field : SerializeField] public RectTransform ImgaePanelRect { get; private set; }

    [field : Title("[ Hint ]")]
    [field : SerializeField] public Button HintButton { get; private set; }
    [field : SerializeField] public RectTransform HintPosition { get; private set; }
    [field : SerializeField] public RectTransform HintPosition_Image { get; private set; } 

    [field : Title("[ Answer Panel ]")]
    [field : SerializeField] public QuizAnswer[] AnswerView { get; private set; }
    [field : SerializeField] public Button AnswerCheckButton { get; private set; }

    [field : Title("[ Reward Panel ]")]
    [field : SerializeField] public RectTransform RewardPanel { get; private set; }

    [field : Title("[ Setting ]")]
    [SerializeField] private float hintheightOffset = 60f;

    private void Awake()
    {
        PreviousPageButton.onClick.AddListener(() => PreviousQuestionPage());
        NextPageButton.onClick.AddListener(() => NextQuestionPage());
        AnswerCheckButton.onClick.AddListener(() => quizPopup.AnswerCheck());
    }
    
    [Button]
    public void UseImageSetting()
    {
        ImgaePanelRect.gameObject.SetActive(true);
        QuizPopupImage.SetNativeSize();
        HintButton.GetComponent<RectTransform>().anchoredPosition = HintPosition_Image.anchoredPosition;
    }

    [Button]
    public void NotUseImageSetting()
    {
        ImgaePanelRect.gameObject.SetActive(false);
        QuizPopupImage.SetNativeSize();
        QuizPopupRect.sizeDelta = new Vector2(QuizPopupRect.rect.width, QuizPopupRect.rect.height - ImgaePanelRect.rect.height + hintheightOffset) ;
        HintButton.GetComponent<RectTransform>().anchoredPosition = HintPosition.anchoredPosition;
    }

    public void ViewSetting(QuizData quiz)
    {
        // 질문
        QuestionTextSetting(quiz.question);

        // 보기 (랜덤 적용)
        int count = AnswerView.Count();
        List<int> list = Enumerable.Range(0, count).ToList();
        List<int> randList = new List<int>();
    
        for(int i = 0 ; i < count ; ++i) {
            int num = Random.Range(0, count - i);
            randList.Add(list[num]);
            list.RemoveAt(num);
        }

        for (int i = 0; i < count; ++i) {
            int num = i;
            int randNum = randList[i];
            AnswerView[i].NumberText.text = (i + 1).ToString();
            AnswerView[i].SetAnswer(quiz.answers[randNum].answer_text, quiz.answer_correct);
            AnswerView[i].Button.onClick.AddListener(() => OnSelectedAnswer(AnswerView[num]));
            AnswerView[i].SetSelected(false);
        }

        HintButton.onClick.AddListener(() => Application.OpenURL(quiz.hint));
    }

#region Question
    private void QuestionTextSetting(string questionString)
    {
        QuestionText.text = questionString;

        StartCoroutine(GetTextPagesCoroutine());
    }

    // 텍스트 레이아웃 강제 업데이트
    IEnumerator GetTextPagesCoroutine()
    {
        QuestionText.ForceMeshUpdate();

        yield return null;

        PageView();
    }

    private void PreviousQuestionPage()
    {
        if (QuestionText.pageToDisplay > 1) {
            QuestionText.pageToDisplay--;
            PageView();
        }
    }

    private void NextQuestionPage()
    {
        if (QuestionText.pageToDisplay < QuestionText.textInfo.pageCount) {
            QuestionText.pageToDisplay++;
            PageView();
        }
    }

    private void PageView()
    {
        PreviousPageButton.interactable = QuestionText.pageToDisplay == 1 ? false : true;
        NextPageButton.interactable = QuestionText.pageToDisplay == QuestionText.textInfo.pageCount ? false : true;
        QuistionPageText.text = QuestionText.pageToDisplay.ToString() + '/' + QuestionText.textInfo.pageCount.ToString();

        if (QuestionText.textInfo.pageCount <= 1)
        {
            QuistionPagePanel.gameObject.SetActive(false);
        }
    }
#endregion

#region Answer
    private void OnSelectedAnswer(QuizAnswer quizAnswer)
    {
        foreach (QuizAnswer answer in AnswerView) {
            if (quizAnswer == answer) {
                if (quizPopup.SelectedAnswer == answer) {
                    quizPopup.SelectedAnswer = null;
                    answer.SetSelected(false);
                }
                else {
                    quizPopup.SelectedAnswer = answer;
                    answer.SetSelected(true);
                }
            }
            else {
                answer.SetSelected(false);
            }
        }
    }
#endregion
}