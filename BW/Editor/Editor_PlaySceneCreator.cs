using System.IO;
using UnityEditor;
using System.Collections.Generic;
public class Editor_PlaySceneCreator : EditorWindow
{
    private static string playScenePath = "Assets/Association/BW/Editor/";

    [MenuItem("Tools/Create Play Scene File")]
    public static void CreatePlayScene()
    {
        EditorBuildSettingsScene[] sceneList = EditorBuildSettings.scenes;

        if (sceneList.Length == 0) return;

        if (Directory.Exists(playScenePath) == false) {
            Directory.CreateDirectory(playScenePath);
        }
        List<string> sceneNameList = new List<string>();
        foreach (var scene in sceneList) {
            string[] paths = scene.path.Split('/');
            sceneNameList.Add(paths[paths.Length - 1].Split('.')[0]);
        }

        StreamWriter sw = new StreamWriter(playScenePath + "Editor_PlayScene.cs");

        sw.WriteLine("// This enum is auto created by Editor_PlaySceneCreator.cs");
        sw.WriteLine("using UnityEditor;");
        sw.WriteLine("using UnityEditor.SceneManagement;");
        sw.WriteLine();
        sw.WriteLine("public class Editor_PlayScene : EditorWindow");
        sw.WriteLine('{');

        short count_Common = 0;
        short count_Stage = 0;
        short count_OffStage = 0;
        short count_ETC = 0;
        short count_current = 0;
        string sceneGroup = "0"; // 우선순위 11이상 차이 시 구분선 (+2씩)

        for (int i = 0; i < sceneNameList.Count; ++i) {
            string sceneSplit = sceneNameList[i].ToString().Split('_')[0];
            if (sceneSplit == "Login" || sceneSplit == "Lobby" || sceneSplit == "Loading") {
                count_current = ++count_Common;
                sceneGroup = "0";
            }
            else if (sceneSplit == "Stage") {
                count_current = ++count_Stage;
                sceneGroup = "2";
            }
            else if (sceneSplit == "OffStage") {
                count_current = ++count_OffStage;
                sceneGroup = "4";
            }
            else {
                count_current = ++count_ETC;
                sceneGroup = "6";
            }
            sw.WriteLine($"\t[MenuItem(\"Editor/PlayScene/{sceneNameList[i]}\", false, {sceneGroup + count_current})]");
            sw.WriteLine($"\tstatic void Scene_Play_{i}() => ScenePlay({i});");
            sw.WriteLine();
        }

        sw.WriteLine("\tpublic static void ScenePlay(int SceneIndex)");
        sw.WriteLine("\t{");
        sw.WriteLine("\t\tvar pathOfFirstScene = EditorBuildSettings.scenes[SceneIndex].path;");
        sw.WriteLine("\t\tvar sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);");
        sw.WriteLine();
        sw.WriteLine("\t\tif (sceneAsset != null) {");
        sw.WriteLine("\t\t\tEditorSceneManager.playModeStartScene = sceneAsset;");
        sw.WriteLine("\t\t\tEditorApplication.EnterPlaymode();");
        sw.WriteLine("\t\t\tEditorApplication.quitting += Reset;");
        sw.WriteLine("\t\t}");
        sw.WriteLine("\t}");
        sw.WriteLine();
        sw.WriteLine("\t[InitializeOnEnterPlayMode]");
        sw.WriteLine("\tprivate static void OnEnterPlayMode(EnterPlayModeOptions option)");
        sw.WriteLine("\t{");
        sw.WriteLine("\t\tEditorApplication.update += Reset;");
        sw.WriteLine("\t}");
        sw.WriteLine();
        sw.WriteLine("\t private static void Reset()");
        sw.WriteLine("\t{");
        sw.WriteLine("\t\tEditorSceneManager.playModeStartScene = null;");
        sw.WriteLine("\t\tEditorApplication.update -= Reset;");
        sw.WriteLine("\t}");
        sw.WriteLine('}');
        sw.Close();

        AssetDatabase.Refresh();
        SceneView.RepaintAll();
        UnityEngine.Debug.Log("Successfully created Editor_PlayScene.cs file.");
    }

    [InitializeOnLoadMethod]
    public static void RegisterSceneListChangedCallback()
    {
        EditorBuildSettings.sceneListChanged -= CreatePlayScene;
        EditorBuildSettings.sceneListChanged += CreatePlayScene;
    }
}