using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{

#region On GUI Log
# if !UNITY_EDITOR
    string myLog;
    Queue myLogQueue = new Queue();
    GUIStyle logStyle;

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
        logStyle = new GUIStyle(); // GUIStyle 초기화
        logStyle.fontSize = 12;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        string newString = "\n [" + type + "] : " + logString;

        // 로그 타입에 따라 색상 선택
        switch (type) {
            case LogType.Error:
                newString = $"<color=#ff0000>{newString}</color>";
                break;
            case LogType.Warning:
                newString = $"<color=#ffa500>{newString}</color>";
                break;
            case LogType.Log:
                newString = $"<color=#00ff00>{newString}</color>";
                break;
            case LogType.Exception:
                newString = $"<color=#ff00ff>{newString}\n{stackTrace}</color>";
                break;
        }

        myLogQueue.Enqueue(newString);

        if (myLogQueue.Count > 15) {
            myLogQueue.Dequeue(); // 큐가 너무 길어지지 않도록 제한
        }

        myLog = string.Empty;
        foreach (string log in myLogQueue) {
            myLog += log;
        }
    }

    void OnGUI() {
        logStyle.richText = true; // Rich Text 활성화
        GUILayout.Label(myLog, logStyle); // 스타일 적용
    }
#endif
#endregion

    public TMP_Text valueText;
    public TMP_InputField inputText;

    private void Update()
    {
        if (long.TryParse(inputText.text, out long value)) {
            valueText.text = BW.Tool.GetCurrencyText(value);
        }
    }
}
