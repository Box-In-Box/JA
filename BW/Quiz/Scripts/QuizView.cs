using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Web;
using Sirenix.OdinInspector;

public class QuizView : MonoBehaviour
{
    [SerializeField] private QuizPopup quizPopup;
    [field : SerializeField] public RectTransform quizPopupRect { get; private set; }
    [field : SerializeField] public Image quizPopupImage { get; private set; }

    [field : Title("[ Question Panel ]")]
    [field : SerializeField] public TMP_Text  questionText { get; private set; }
    [field : SerializeField] public RectTransform quistionPagePanel { get; private set; }
    [field : SerializeField] public TMP_Text quistionPageText { get; private set; }
    [field : SerializeField] public Button previousPageButton { get; private set; }
    [field : SerializeField] public Button nextPageButton { get; private set; }
    [field : SerializeField] public RectTransform imgaePanelRect { get; private set; }

    [field : Title("[ Hint ]")]
    [field : SerializeField] public Toggle hintToggle { get; private set; }
    [field : SerializeField] public RectTransform hintPosition { get; private set; }
    [field : SerializeField] public RectTransform hintPosition_Image { get; private set; } 

    [field : Title("[ Answer Panel ]")]
    [field : SerializeField] public QuizAnswer[] answerView { get; private set; }
    [field : SerializeField] public Button answerCheckButton { get; private set; }

    [field : Title("[ Reward Panel ]")]
    [field : SerializeField] public RectTransform rewardPanel { get; private set; }

    [field : Title("[ Setting ]")]
    [field : SerializeField] private float hintheightOffset = 60f;

    private void Awake()
    {
        previousPageButton.onClick.AddListener(() => PreviousQuestionPage());
        nextPageButton.onClick.AddListener(() => NextQuestionPage());
        answerCheckButton.onClick.AddListener(() => quizPopup.AnswerCheck());
    }
    
    [Button]
    public void UseImageSetting()
    {
        imgaePanelRect.gameObject.SetActive(true);
        quizPopupImage.SetNativeSize();
        hintToggle.GetComponent<RectTransform>().anchoredPosition = hintPosition_Image.anchoredPosition;
    }

    [Button]
    public void NotUseImageSetting()
    {
        imgaePanelRect.gameObject.SetActive(false);
        quizPopupImage.SetNativeSize();
        quizPopupRect.sizeDelta = new Vector2(quizPopupRect.rect.width, quizPopupRect.rect.height - imgaePanelRect.rect.height + hintheightOffset) ;
        hintToggle.GetComponent<RectTransform>().anchoredPosition = hintPosition.anchoredPosition;
    }

    public void ViewSetting(QuizData quiz)
    {
        // 질문
        QuestionTextSetting(quiz.question);

        // 보기 (랜덤 적용)
        int count = answerView.Count();
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
            answerView[i].numberText.text = (i + 1).ToString();
            answerView[i].SetAnswer(quiz.answers[randNum].answer_text, quiz.answer_correct);
            answerView[i].button.onClick.AddListener(() => OnSelectedAnswer(answerView[num]));
            answerView[i].SetSelected(false);
        }
    }

#region Question
    private void QuestionTextSetting(string questionString)
    {
        questionText.text = questionString;

        StartCoroutine(GetTextPagesCoroutine());
    }

    // 텍스트 레이아웃 강제 업데이트
    IEnumerator GetTextPagesCoroutine()
    {
        questionText.ForceMeshUpdate();

        yield return null;

        PageView();
    }

    private void PreviousQuestionPage()
    {
        if (questionText.pageToDisplay > 1) {
            questionText.pageToDisplay--;
            PageView();
        }
    }

    private void NextQuestionPage()
    {
        if (questionText.pageToDisplay < questionText.textInfo.pageCount) {
            questionText.pageToDisplay++;
            PageView();
        }
    }

    private void PageView()
    {
        previousPageButton.interactable = questionText.pageToDisplay == 1 ? false : true;
        nextPageButton.interactable = questionText.pageToDisplay == questionText.textInfo.pageCount ? false : true;
        quistionPageText.text = questionText.pageToDisplay.ToString() + '/' + questionText.textInfo.pageCount.ToString();
    }
#endregion

#region Answer
    private void OnSelectedAnswer(QuizAnswer quizAnswer)
    {
        foreach (QuizAnswer answer in answerView) {
            if (quizAnswer == answer) {
                if (quizPopup.selectedAnswer == answer) {
                    quizPopup.selectedAnswer = null;
                    answer.SetSelected(false);
                }
                else {
                    quizPopup.selectedAnswer = answer;
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