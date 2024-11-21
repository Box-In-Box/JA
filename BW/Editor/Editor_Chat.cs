using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChatManager))]
public class Editor_Chat : Editor
{
    ChatManager chatManager;
    string text;

    void OnEnable()
    {
        chatManager = target as ChatManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle style = new GUIStyle (GUI.skin.textArea);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("===== Editor =====", style);

        EditorGUILayout.BeginHorizontal();
        text = EditorGUILayout.TextArea(text);

        if (GUILayout.Button("보내기", GUILayout.Width(60)) && text.Trim() != "") {
            chatManager.Chat(true, text, "나", null);
            text = "";
            GUI.FocusControl(null);
        }

        if (GUILayout.Button("받기", GUILayout.Width(60)) && text.Trim() != "") {
            Chat_Message chat_Message = new Chat_Message();
            chat_Message.uuid = int.MaxValue;
            chat_Message.nickName = "상대";
            chat_Message.msg = text;
            chatManager.ChatReceive(chat_Message);
            text = "";
            GUI.FocusControl(null);
        }

        EditorGUILayout.EndHorizontal();
    }
}
