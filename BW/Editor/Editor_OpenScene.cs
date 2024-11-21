// This enum is auto created by Editor_OpenSceneCreator.cs
using UnityEditor;
using UnityEditor.SceneManagement;

public class Editor_OpenScene : EditorWindow
{
	[MenuItem("Editor/OpenScene/Login", false, 01)]
	static void Scene_0() => SceneOpen(0);

	[MenuItem("Editor/OpenScene/Loading", false, 02)]
	static void Scene_1() => SceneOpen(1);

	[MenuItem("Editor/OpenScene/Lobby", false, 03)]
	static void Scene_2() => SceneOpen(2);

	[MenuItem("Editor/OpenScene/MyRoom", false, 61)]
	static void Scene_3() => SceneOpen(3);

	[MenuItem("Editor/OpenScene/Stage_World", false, 21)]
	static void Scene_4() => SceneOpen(4);

	[MenuItem("Editor/OpenScene/Stage_Outside", false, 22)]
	static void Scene_5() => SceneOpen(5);

	[MenuItem("Editor/OpenScene/Stage_CityHall", false, 23)]
	static void Scene_6() => SceneOpen(6);

	[MenuItem("Editor/OpenScene/Stage_Gongsan", false, 24)]
	static void Scene_7() => SceneOpen(7);

	[MenuItem("Editor/OpenScene/Stage_Magoksa", false, 25)]
	static void Scene_8() => SceneOpen(8);

	[MenuItem("Editor/OpenScene/Stage_MRtomb", false, 26)]
	static void Scene_9() => SceneOpen(9);

	[MenuItem("Editor/OpenScene/Stage_Flower", false, 27)]
	static void Scene_10() => SceneOpen(10);

	[MenuItem("Editor/OpenScene/Stage_Cathedral", false, 28)]
	static void Scene_11() => SceneOpen(11);

	[MenuItem("Editor/OpenScene/Stage_0", false, 29)]
	static void Scene_12() => SceneOpen(12);

	[MenuItem("Editor/OpenScene/Stage_1", false, 210)]
	static void Scene_13() => SceneOpen(13);

	[MenuItem("Editor/OpenScene/OffStage_0", false, 41)]
	static void Scene_14() => SceneOpen(14);

	[MenuItem("Editor/OpenScene/OffStage_Archery", false, 42)]
	static void Scene_15() => SceneOpen(15);

	[MenuItem("Editor/OpenScene/OffStage_GatheringChestnuts", false, 43)]
	static void Scene_16() => SceneOpen(16);

	[MenuItem("Editor/OpenScene/OffStage_Test", false, 44)]
	static void Scene_17() => SceneOpen(17);

	[MenuItem("Editor/OpenScene/OffStage_Batting", false, 45)]
	static void Scene_18() => SceneOpen(18);

	[MenuItem("Editor/OpenScene/OffStage_Injeolmi", false, 46)]
	static void Scene_19() => SceneOpen(19);

	[MenuItem("Editor/OpenScene/OffStage_ChestnutFarm", false, 47)]
	static void Scene_20() => SceneOpen(20);

	[MenuItem("Editor/OpenScene/OffStage_RoastingChestnuts", false, 48)]
	static void Scene_21() => SceneOpen(21);

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
