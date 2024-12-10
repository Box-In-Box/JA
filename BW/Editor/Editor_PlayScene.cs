// This enum is auto created by Editor_PlaySceneCreator.cs
using UnityEditor;
using UnityEditor.SceneManagement;

public class Editor_PlayScene : EditorWindow
{
	[MenuItem("Editor/PlayScene/Login", false, 001)]
	static void Scene_Play_0() => ScenePlay(0);

	[MenuItem("Editor/PlayScene/Lobby", false, 002)]
	static void Scene_Play_1() => ScenePlay(1);

	[MenuItem("Editor/PlayScene/MyRoom", false, 601)]
	static void Scene_Play_2() => ScenePlay(2);

	[MenuItem("Editor/PlayScene/Stage_World", false, 201)]
	static void Scene_Play_3() => ScenePlay(3);

	[MenuItem("Editor/PlayScene/Stage_Outside", false, 202)]
	static void Scene_Play_4() => ScenePlay(4);

	[MenuItem("Editor/PlayScene/Stage_CityHall", false, 203)]
	static void Scene_Play_5() => ScenePlay(5);

	[MenuItem("Editor/PlayScene/Stage_Gongsan", false, 204)]
	static void Scene_Play_6() => ScenePlay(6);

	[MenuItem("Editor/PlayScene/Stage_Magoksa", false, 205)]
	static void Scene_Play_7() => ScenePlay(7);

	[MenuItem("Editor/PlayScene/Stage_MRtomb", false, 206)]
	static void Scene_Play_8() => ScenePlay(8);

	[MenuItem("Editor/PlayScene/Stage_flower", false, 207)]
	static void Scene_Play_9() => ScenePlay(9);

	[MenuItem("Editor/PlayScene/Stage_Cathedral", false, 208)]
	static void Scene_Play_10() => ScenePlay(10);

	[MenuItem("Editor/PlayScene/Stage_History_Cinema", false, 209)]
	static void Scene_Play_11() => ScenePlay(11);

	[MenuItem("Editor/PlayScene/Stage_Museum_Histroy", false, 210)]
	static void Scene_Play_12() => ScenePlay(12);

	[MenuItem("Editor/PlayScene/Stage_academia", false, 211)]
	static void Scene_Play_13() => ScenePlay(13);

	[MenuItem("Editor/PlayScene/Stage_lecture02", false, 212)]
	static void Scene_Play_14() => ScenePlay(14);

	[MenuItem("Editor/PlayScene/Stage_Hwangsaebawi", false, 213)]
	static void Scene_Play_15() => ScenePlay(15);

	[MenuItem("Editor/PlayScene/Stage_0", false, 214)]
	static void Scene_Play_16() => ScenePlay(16);

	[MenuItem("Editor/PlayScene/Stage_1", false, 215)]
	static void Scene_Play_17() => ScenePlay(17);

	[MenuItem("Editor/PlayScene/OffStage_0", false, 401)]
	static void Scene_Play_18() => ScenePlay(18);

	[MenuItem("Editor/PlayScene/OffStage_Archery_", false, 402)]
	static void Scene_Play_19() => ScenePlay(19);

	[MenuItem("Editor/PlayScene/OffStage_GatheringChestnuts", false, 403)]
	static void Scene_Play_20() => ScenePlay(20);

	[MenuItem("Editor/PlayScene/OffStage_Test", false, 404)]
	static void Scene_Play_21() => ScenePlay(21);

	[MenuItem("Editor/PlayScene/OffStage_Archery", false, 405)]
	static void Scene_Play_22() => ScenePlay(22);

	[MenuItem("Editor/PlayScene/OffStage_Batting", false, 406)]
	static void Scene_Play_23() => ScenePlay(23);

	[MenuItem("Editor/PlayScene/OffStage_ChestnutFarm", false, 407)]
	static void Scene_Play_24() => ScenePlay(24);

	[MenuItem("Editor/PlayScene/OffStage_Injeolmi", false, 408)]
	static void Scene_Play_25() => ScenePlay(25);

	[MenuItem("Editor/PlayScene/OffStage_RoastingChestnuts", false, 409)]
	static void Scene_Play_26() => ScenePlay(26);

	[MenuItem("Editor/PlayScene/OffStage_PickUpChestnut", false, 410)]
	static void Scene_Play_27() => ScenePlay(27);

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
