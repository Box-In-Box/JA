using System.IO;
using UnityEditor;
using System.Collections.Generic;
public class Editor_OpenSceneCreator : EditorWindow
{
    private static string openScenePath = "Assets/Association/BW/Editor/";

    [MenuItem("Tools/Create Open Scene File")]
    public static void CreateOpenScene()
    {
        EditorBuildSettingsScene[] sceneList = EditorBuildSettings.scenes;

        if (sceneList.Length == 0) return;

        if (Directory.Exists(openScenePath) == false) {
            Directory.CreateDirectory(openScenePath);
        }
        List<string> sceneNameList = new List<string>();
        foreach (var scene in sceneList) {
            string[] paths = scene.path.Split('/');
            sceneNameList.Add(paths[paths.Length - 1].Split('.')[0]);
        }

        StreamWriter sw = new StreamWriter(openScenePath + "Editor_OpenScene.cs");

        sw.WriteLine("// This enum is auto created by Editor_OpenSceneCreator.cs");
        sw.WriteLine("using UnityEditor;");
        sw.WriteLine("using UnityEditor.SceneManagement;");
        sw.WriteLine();
        sw.WriteLine("public class Editor_OpenScene : EditorWindow");
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
            sw.WriteLine($"\t[MenuItem(\"Editor/OpenScene/{sceneNameList[i]}\", false, {sceneGroup + count_current.ToString("D2")})]");
            sw.WriteLine($"\tstatic void Scene_{i}() => SceneOpen({i});");
            sw.WriteLine();
        }

        sw.WriteLine("\tstatic public void SceneOpen(int SceneIndex)");
        sw.WriteLine("\t{");
        sw.WriteLine("\t\tvar pathOfFirstScene = EditorBuildSettings.scenes[SceneIndex].path;");
        sw.WriteLine("\t\tvar sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);");
        sw.WriteLine("\t\tvar sceneName = sceneAsset.ToString().Split(' ');");
        sw.WriteLine();
        sw.WriteLine("\t\tif (sceneAsset != null) {");
        sw.WriteLine("\t\t\tEditorSceneManager.OpenScene(\"Assets/Scenes/\" + sceneName[0] + \".unity\");");
        sw.WriteLine("\t\t}");
        sw.WriteLine("\t}");
        sw.WriteLine("}");
        sw.Close();

        AssetDatabase.Refresh();
        SceneView.RepaintAll();
        UnityEngine.Debug.Log("Successfully created Editor_OpenScene.cs file.");
    }

    [InitializeOnLoadMethod]
    public static void RegisterSceneListChangedCallback()
    {
        EditorBuildSettings.sceneListChanged -= CreateOpenScene;
        EditorBuildSettings.sceneListChanged += CreateOpenScene;
    }
}