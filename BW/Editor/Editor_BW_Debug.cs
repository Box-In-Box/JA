using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class Editor_BW_Debug : OdinEditorWindow
{
    [MenuItem("Editor/Debug Tool(BW)")]
    private static void OpenWindow()
    {
        GetWindow<Editor_BW_Debug>().Show();
    }

    //[Title("Popup")]
    //[Button("QuizPopup"), GUIColor(0, 1, 0)] public void QuizPopup() => PopupManager.instance.Open<QuizPopup>().Quiz("무령왕릉");
    //[Button("MyRoomPopup"), GUIColor(0, 1, 0)] public void MyRoomPopup() => PopupManager.instance.Open<MyRoomPopup>();
}
