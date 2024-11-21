// This enum is auto created by Editor_PlaySceneCreator.cs
using UnityEditor;
using UnityEditor.SceneManagement;

public class Editor_PlayScene : EditorWindow
{
	[MenuItem("Editor/PlayScene/Login", false, 01)]
	static void Scene_Play_0() => ScenePlay(0);

	[MenuItem("Editor/PlayScene/Loading", false, 02)]
	static void Scene_Play_1() => ScenePlay(1);

	[MenuItem("Editor/PlayScene/Lobby", false, 03)]
	static void Scene_Play_2() => ScenePlay(2);

	[MenuItem("Editor/PlayScene/MyRoom", false, 61)]
	static void Scene_Play_3() => ScenePlay(3);

	[MenuItem("Editor/PlayScene/Stage_World", false, 21)]
	static void Scene_Play_4() => ScenePlay(4);

	[MenuItem("Editor/PlayScene/Stage_Outside", false, 22)]
	static void Scene_Play_5() => ScenePlay(5);

	[MenuItem("Editor/PlayScene/Stage_CityHall", false, 23)]
	static void Scene_Play_6() => ScenePlay(6);

	[MenuItem("Editor/PlayScene/Stage_Gongsan", false, 24)]
	static void Scene_Play_7() => ScenePlay(7);

	[MenuItem("Editor/PlayScene/Stage_Magoksa", false, 25)]
	static void Scene_Play_8() => ScenePlay(8);

	[MenuItem("Editor/PlayScene/Stage_MRtomb", false, 26)]
	static void Scene_Play_9() => ScenePlay(9);

	[MenuItem("Editor/PlayScene/Stage_Flower", false, 27)]
	static void Scene_Play_10() => ScenePlay(10);

	[MenuItem("Editor/PlayScene/Stage_Cathedral", false, 28)]
	static void Scene_Play_11() => ScenePlay(11);

	[MenuItem("Editor/PlayScene/Stage_0", false, 29)]
	static void Scene_Play_12() => ScenePlay(12);

	[MenuItem("Editor/PlayScene/Stage_1", false, 210)]
	static void Scene_Play_13() => ScenePlay(13);

	[MenuItem("Editor/PlayScene/OffStage_0", false, 41)]
	static void Scene_Play_14() => ScenePlay(14);

	[MenuItem("Editor/PlayScene/OffStage_Archery", false, 42)]
	static void Scene_Play_15() => ScenePlay(15);

	[MenuItem("Editor/PlayScene/OffStage_GatheringChestnuts", false, 43)]
	static void Scene_Play_16() => ScenePlay(16);

	[MenuItem("Editor/PlayScene/OffStage_Test", false, 44)]
	static void Scene_Play_17() => ScenePlay(17);

	[MenuItem("Editor/PlayScene/OffStage_Batting", false, 45)]
	static void Scene_Play_18() => ScenePlay(18);

	[MenuItem("Editor/PlayScene/OffStage_Injeolmi", false, 46)]
	static void Scene_Play_19() => ScenePlay(19);

	[MenuItem("Editor/PlayScene/OffStage_ChestnutFarm", false, 47)]
	static void Scene_Play_20() => ScenePlay(20);

	[MenuItem("Editor/PlayScene/OffStage_RoastingChestnuts", false, 48)]
	static void Scene_Play_21() => ScenePlay(21);

	public static void ScenePlay(int SceneIndex)
	{
		var pathOfFirstScene = EditorBuildSettings.scenes[SceneIndex].path;
		var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);

		if (sceneAsset != null) {
			EditorSceneManager.playModeStartScene = sceneAsset;
			EditorApplication.EnterPlaymode();
			EditorApplication.quitting += Reset;
		}
	}

	[InitializeOnEnterPlayMode]
	private static void OnEnterPlayMode(EnterPlayModeOptions option)
	{
		EditorApplication.update += Reset;
	}

	 private static void Reset()
	{
		EditorSceneManager.playModeStartScene = null;
		EditorApplication.update -= Reset;
	}
}
