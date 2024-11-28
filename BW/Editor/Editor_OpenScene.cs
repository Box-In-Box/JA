// This enum is auto created by Editor_OpenSceneCreator.cs
using UnityEditor;
using UnityEditor.SceneManagement;

public class Editor_OpenScene : EditorWindow
{
	[MenuItem("Editor/OpenScene/Login", false, 001)]
	static void Scene_0() => SceneOpen(0);

	[MenuItem("Editor/OpenScene/Lobby", false, 002)]
	static void Scene_1() => SceneOpen(1);

	[MenuItem("Editor/OpenScene/MyRoom", false, 601)]
	static void Scene_2() => SceneOpen(2);

	[MenuItem("Editor/OpenScene/Stage_World", false, 201)]
	static void Scene_3() => SceneOpen(3);

	[MenuItem("Editor/OpenScene/Stage_Outside", false, 202)]
	static void Scene_4() => SceneOpen(4);

	[MenuItem("Editor/OpenScene/Stage_CityHall", false, 203)]
	static void Scene_5() => SceneOpen(5);

	[MenuItem("Editor/OpenScene/Stage_Gongsan", false, 204)]
	static void Scene_6() => SceneOpen(6);

	[MenuItem("Editor/OpenScene/Stage_Magoksa", false, 205)]
	static void Scene_7() => SceneOpen(7);

	[MenuItem("Editor/OpenScene/Stage_MRtomb", false, 206)]
	static void Scene_8() => SceneOpen(8);

	[MenuItem("Editor/OpenScene/Stage_flower", false, 207)]
	static void Scene_9() => SceneOpen(9);

	[MenuItem("Editor/OpenScene/Stage_Cathedral", false, 208)]
	static void Scene_10() => SceneOpen(10);

	[MenuItem("Editor/OpenScene/Stage_History_Cinema", false, 209)]
	static void Scene_11() => SceneOpen(11);

	[MenuItem("Editor/OpenScene/Stage_Museum_Histroy", false, 210)]
	static void Scene_12() => SceneOpen(12);

	[MenuItem("Editor/OpenScene/Stage_academia", false, 211)]
	static void Scene_13() => SceneOpen(13);

	[MenuItem("Editor/OpenScene/Stage_lecture02", false, 212)]
	static void Scene_14() => SceneOpen(14);

	[MenuItem("Editor/OpenScene/Stage_Hwangsaebawi", false, 213)]
	static void Scene_15() => SceneOpen(15);

	[MenuItem("Editor/OpenScene/Stage_0", false, 214)]
	static void Scene_16() => SceneOpen(16);

	[MenuItem("Editor/OpenScene/Stage_1", false, 215)]
	static void Scene_17() => SceneOpen(17);

	[MenuItem("Editor/OpenScene/OffStage_0", false, 401)]
	static void Scene_18() => SceneOpen(18);

	[MenuItem("Editor/OpenScene/OffStage_Archery", false, 402)]
	static void Scene_19() => SceneOpen(19);

	[MenuItem("Editor/OpenScene/OffStage_GatheringChestnuts", false, 403)]
	static void Scene_20() => SceneOpen(20);

	[MenuItem("Editor/OpenScene/OffStage_Test", false, 404)]
	static void Scene_21() => SceneOpen(21);

	[MenuItem("Editor/OpenScene/OffStage_Batting", false, 405)]
	static void Scene_22() => SceneOpen(22);

	[MenuItem("Editor/OpenScene/OffStage_Injeolmi", false, 406)]
	static void Scene_23() => SceneOpen(23);

	[MenuItem("Editor/OpenScene/OffStage_ChestnutFarm", false, 407)]
	static void Scene_24() => SceneOpen(24);

	[MenuItem("Editor/OpenScene/OffStage_RoastingChestnuts", false, 408)]
	static void Scene_25() => SceneOpen(25);

	static public void SceneOpen(int SceneIndex)
	{
		var pathOfFirstScene = EditorBuildSettings.scenes[SceneIndex].path;
		var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
		var sceneName = sceneAsset.ToString().Split(' ');

		if (sceneAsset != null) {
			EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName[0] + ".unity");
		}
	}
}
