using UnityEditor;
using UnityEngine;

public class Editor_Rename : EditorWindow
{
    private string mBaseName = "NewName"; // Default name
    private int mStartIdx = 0; // Start index
    private int mAssetsCount = 0; // Number of selected assets

    [MenuItem("Editor/Hierarchy Rename")]
    public static void ShowWindow()
    {
        GetWindow<Editor_Rename>("Hierarchy Rename");
    }

    private void OnGUI()
    {
        mBaseName = EditorGUILayout.TextField("Base Name", mBaseName);
        mStartIdx = EditorGUILayout.IntField("Start Index", mStartIdx);

        GUILayout.Space(10);

        // Check conditions to enable/disable the button
        bool canRename = !string.IsNullOrEmpty(mBaseName) && Selection.objects.Length > 0;

        GUI.enabled = canRename; // Set button's enabled state

        if (GUILayout.Button("Rename Selected Assets"))
            RenameSelectedAssets();

        GUI.enabled = true; // Reset enabled state for other GUI elements

        // Add more space
        GUILayout.Space(20);

        // Begin a horizontal layout group
        GUILayout.BeginHorizontal();

        // Display a label with the text "Powered by: Bonnate" at the bottom of the window
        EditorGUILayout.LabelField("Powered by: Bonnate");

        // Create a button with the label "Github" that opens a URL when clicked
        if (GUILayout.Button("Github", GetHyperlinkLabelStyle()))
        {
            // Call the method to open the GitHub URL
            OpenURL("https://github.com/bonnate");
        }

        // Create a button with the label "Blog" that opens a URL when clicked
        if (GUILayout.Button("Blog", GetHyperlinkLabelStyle()))
        {
            // Call the method to open the Blog URL
            OpenURL("https://bonnate.tistory.com/");
        }

        // End the horizontal layout group
        GUILayout.EndHorizontal();
    }

    private void RenameSelectedAssets()
    {
        Object[] selectedAssets = Selection.objects;

        if (selectedAssets.Length == 0)
        {
            Debug.LogWarning("No assets selected.");
            return;
        }

        mAssetsCount = selectedAssets.Length;

        for (int i = 0; i < mAssetsCount; i++)
        {
            Object asset = selectedAssets[i];
            string newName = mBaseName + (mStartIdx + i);

            // Rename Assets
            string assetPath = AssetDatabase.GetAssetPath(asset);

            // Record Undo
            Undo.RecordObject(asset, "Rename Asset");

            AssetDatabase.RenameAsset(assetPath, newName);

            // Rename
            asset.name = newName;

            // SetDirty
            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // Returns a GUI style for a hyperlink-like label
    private GUIStyle GetHyperlinkLabelStyle()
    {
        // Create a new GUIStyle based on the default label style
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = new Color(0f, 0.5f, 1f); // Blue color
        style.stretchWidth = false;
        style.wordWrap = false;
        return style;
    }

    // Opens the provided URL with the default application
    private void OpenURL(string url)
    {
        EditorUtility.OpenWithDefaultApp(url);
    }
}