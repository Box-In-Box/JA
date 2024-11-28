using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class Editor_SceneNameEnumCreator
{
	private static string sceneNameEnumPath = "Assets/Scripts/SceneControl/";

	[MenuItem("Tools/Create Scene Enum File")]
	public static void CreateEnum()
	{
		EditorBuildSettingsScene[] sceneList = EditorBuildSettings.scenes;

		if (sceneList.Length == 0) return;

		if (Directory.Exists(sceneNameEnumPath) == false) {
			Directory.CreateDirectory(sceneNameEnumPath);
		}
		List<string> sceneNameList = new List<string>();
		foreach (var scene in sceneList) {
			string[] paths = scene.path.Split('/');
			sceneNameList.Add(paths[paths.Length - 1].Split('.')[0]);
		}

		StreamWriter sw = new StreamWriter(sceneNameEnumPath + "SceneLoadManager_SceneName.cs");

		sw.WriteLine("// This enum is auto created by Editor_SceneNameEnumCreator.cs");
		sw.WriteLine();
		sw.WriteLine("public enum SceneName");
		sw.WriteLine('{');
		foreach (var sceneName in sceneNameList) {
			sw.WriteLine($"\t{sceneName},");
		}
		sw.WriteLine("\tNull,");
		sw.WriteLine("}");
		sw.Close();

		AssetDatabase.Refresh();
        SceneView.RepaintAll();
		UnityEngine.Debug.Log("Successfully created scene enum file.");
	}
	/*
	[InitializeOnLoadMethod]
	public static void RegisterSceneListChangedCallback()
	{
		EditorBuildSettings.sceneListChanged -= CreateEnum;
		EditorBuildSettings.sceneListChanged += CreateEnum;
	}
	*/
}