using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Gongju.Web;

public class ChatManager : View, IChatReceiver
{
    [Title("[ Chatting ]")]
    public RectTransform chatting;
    public RectTransform chatContentRect;
    public Scrollbar chatScrollbar;
    private ChatArea lastArea = null;
    [Tooltip("체크 시 상대 프로필 표시 (서버에 프로필 가져올 때 체크)")] public bool isUseProfile = false;
    [Tooltip("체크 시 상대 닉네임에 씬 표시")] public bool isUseSceneName = true;

    [Title("[ Chat ]")]
    public RectTransform chat;
    public Toggle chattingToggle;
    public TMP_InputField msgInput;
    public Button sendButton;
    public RectTransform alarm;
    public Action<bool> chattingAction;
    public Action sendAction;
    
    [Title("[ Message Info ]")]
    [Range(1, 200)] public int msgMaxCount = 100; // 최대 메세지 UI 개수
    private Queue<ChatArea> msgQueue = new Queue<ChatArea>(); // 채팅 UI 개수 제한용

    [Title("[ Message Info(Buffer) ]")]
    [Range(1, 100)] public int msgBufferSize = 50; // 메세지 버퍼 개수 (서버에서 넘어오고 처리 안될 때 삭제)
    private Queue<Chat_Message> msgBuffer = new Queue<Chat_Message>(); // 서버 버퍼 처리용
    private Coroutine chatBufferCoroutine;

    public override void Awake()
    {
        base.Awake();
        // Alarm
        chatScrollbar.onValueChanged.AddListener((value) => AlarmScroll());
        alarm.gameObject.SetActive(false);

        // Chatting
        chattingAction += ChattingPopup;
        chattingToggle.onValueChanged.AddListener((value) => chattingAction?.Invoke(value));

        // Chat Send
        sendAction += ChatSend;
        sendButton.onClick.AddListener(() => sendAction?.Invoke());
        msgInput.onEndEdit.AddListener((value) => EnterChatSend());
    }

    private void OnEnable()
    {
        if (PhotonNetworkManager.instance) {
            PhotonNetworkManager.instance.msgAction += ChatReceive;
        }
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.instance) {
            PhotonNetworkManager.instance.msgAction -= ChatReceive;
        }
    }

#region Chatting
    // 채팅창 메세지 생성
    public void Chat(bool isSend, string msg, string user, Texture2D picture)
    {
        if (msg.Trim() == "") return;

        ChatArea area = Instantiate(isSend ? Resources.Load<ChatArea>("ChatArea_Mine") : Resources.Load<ChatArea>("ChatArea_Other"));

        // ChatArea_Mine - VerticalLayoutGroup.padding.right + 15 값
        if (!isUseProfile) area.GetComponent<VerticalLayoutGroup>().padding.left = 50;

        // 글자에 따라 텍스트 창 조절
        area.transform.SetParent(chatContentRect.transform, false);
        area.boxRect.sizeDelta = new Vector2(400, area.boxRect.sizeDelta.y);
        area.msgRect.GetComponent<TMP_Text>().text = msg;
        Fit(area.boxRect);

        float x = area.msgRect.sizeDelta.x + 42;
        float y = area.msgRect.sizeDelta.y;
        if (y > 49) {
            for (int i = 0; i < 200; ++i) {
                area.boxRect.sizeDelta = new Vector2(x - i * 2, area.boxRect.sizeDelta.y);
                Fit(area.boxRect);

                if (y != area.msgRect.sizeDelta.y) {
                    area.boxRect.sizeDelta = new Vector2(x - (i * 2) + 2, y);
                    break;
                }
            }
        }
        else {
            area.boxRect.sizeDelta = new Vector2(x, y);
        }

        // 채팅 시간 
        DateTime t = DateTime.Now;
        area.time = t.ToString("yyyy-MM-dd-HH-mm");
        area.user = user;

        int hour = t.Hour;
        if (t.Hour == 0) hour = 12;
        else if (t.Hour > 12) hour -= 12;
        area.timeText.text = (t.Hour > 12 ? "오후 " : "오전 ") + hour + ":" + t.Minute.ToString("D2");

        // 같은 사람이 채팅 칠 때, 프로필, 이름 제거
        bool isSame = lastArea != null && lastArea.time == area.time && lastArea.user == area.user;
        if (isSame) lastArea.timeText.text = "";

        if (!isSend) {
            area.userImage.gameObject.SetActive(isUseProfile ? !isSame : false);
            area.userText.gameObject.SetActive(!isSame);
            area.userText.text = area.user;
            if (picture != null) {
                area.userImage.sprite = Sprite.Create(picture, new Rect(0, 0, picture.width, picture.height), new Vector2(.5f, .5f));
            }
        }

        // 날짜가 다를 때 날짜 채팅
        if (lastArea != null && lastArea.time.Substring(0, 10) != area.time.Substring(0, 10)) {
            Transform curDateArea = Instantiate(Resources.Load<ChatArea>("DateArea")).transform;
            curDateArea.SetParent(chatContentRect.transform, false);
            curDateArea.SetSiblingIndex(curDateArea.GetSiblingIndex() - 1);

            string week = "";
            switch (t.DayOfWeek) {
                case DayOfWeek.Sunday : week = "일"; break;
                case DayOfWeek.Monday : week = "월"; break;
                case DayOfWeek.Tuesday : week = "화"; break;
                case DayOfWeek.Wednesday : week = "수"; break;
                case DayOfWeek.Thursday : week = "목"; break;
                case DayOfWeek.Friday : week = "금"; break;
                case DayOfWeek.Saturday : week = "토"; break;
            }
            curDateArea.GetComponent<ChatArea>().dateText.text = t.Year + "년 " + t.Month + "월 " + t.Day + "일 " + week + "요일";
        }

        Fit(area.boxRect);
        Fit(area.areaRect);
        Fit(chatContentRect);
        lastArea = area;

        msgQueue.Enqueue(lastArea);
        if(msgQueue.Count > msgMaxCount) {
            Destroy(msgQueue.Dequeue().gameObject);
        }

        if (!isSend && !IsScrollBotton()) return;
        Invoke(nameof(ScrollDown), 0.03f);
    }

    private void Fit(RectTransform rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    private bool IsScrollBotton() { return chatScrollbar.value <= 0.00001f; }
    private void ScrollDown() => chatScrollbar.value = 0f;
#endregion

#region Chat Send
    // 메세지 -> 네트워크 보내기
    public void ChatSend()
    {
        int uuid = DatabaseConnector.instance.memberUUID;
        string nickName = DatabaseConnector.instance.memberData.nickname;
        if (isUseSceneName)
        {
            nickName += '(' + GameManager.instance.currentScene.ToString() + ')';
        }
        string msg = msgInput.text.Trim();

        if (uuid <= 0 || nickName == "" || msg== "") return;

        PhotonNetworkManager.instance.SendChatMessage(uuid, nickName, msg);

        msgInput.text = "";
        msgInput.ActivateInputField();
    }

    public void EnterChatSend()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            ChatSend();
        }
    }
#endregion

#region Chat Receive
    // 네트워크 메세지 -> 버퍼 추가 -> 채팅창에 추가
    public void ChatReceive(Chat_Message chat_Message)
    {
        msgBuffer.Enqueue(chat_Message);

        if(msgBuffer.Count > msgBufferSize) {
            msgBuffer.Dequeue();
        }

        if (chatBufferCoroutine == null) {
            chatBufferCoroutine = StartCoroutine(ChatBufferCoroutine());
        }

        if (chat_Message.uuid != DatabaseConnector.instance.memberUUID && (!chattingToggle.isOn || !IsScrollBotton())) {
            Alarm(true);
        }
    }

    // 네트워크 메세지 -> 버퍼
    private IEnumerator ChatBufferCoroutine()
    {
        while (true) {
            if (msgBuffer.Count == 0) {
                chatBufferCoroutine = null;
                yield break;
            }

            Chat_Message currentChat = msgBuffer.Dequeue();
            if (currentChat.uuid == DatabaseConnector.instance.memberUUID) {
                Chat(true, currentChat.msg, "", null);
            }
            else {
                Chat(false, currentChat.msg, currentChat.nickName, null);
            }
            yield return null;
        }
    }
#endregion

#region Chat Popup
    // 채팅창 팝업
    private void ChattingPopup(bool value)
    {
        if (value) {
            DOTween.Kill(chatting);
            chatting.DOAnchorPosX(chatting.rect.width, 1f).SetEase(Ease.OutQuart);
            if (IsScrollBotton()) {
                Alarm(false);
            }
        }
        else {
            DOTween.Kill(chatting);
            chatting.DOAnchorPosX(0f, 1f).SetEase(Ease.OutQuart).OnComplete(() => ScrollDown());
        }
    }

    // 알람 적용 여부 함수
    private void Alarm(bool value)
    {
        if (value) {
            if (!alarm.gameObject.activeSelf) {
                DOTween.Kill(alarm);
                alarm.localScale = Vector3.zero;
                alarm.gameObject.SetActive(true);
                alarm.DOScale(1f, 1f).SetEase(Ease.OutQuart);
            }
        }
        else {
            if (alarm.gameObject.activeSelf) {
                DOTween.Kill(alarm);
                alarm.DOScale(0f, 1f).SetEase(Ease.InQuart).OnComplete(() => alarm.gameObject.SetActive(false));
            }
        }
    }

    // 스크롤 아래 시 알람 제거
    private void AlarmScroll()
    {
        if (chattingToggle.isOn && IsScrollBotton()) {
            Alarm(false);
        }
    }
#endregion
}