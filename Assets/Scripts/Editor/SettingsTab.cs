using UnityEditor;
using UnityEngine;
public class SettingsTab : EditorTab
{
    private Vector2 scrollPos;
    private int currentLevel;

    public SettingsTab(GameEditor editor) : base(editor)
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel",1);
    }

    public override void Draw() // Draw setting tab.
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        float oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 100;
        GUILayout.Space(15);
        DrawPreferencesTab();
        EditorGUIUtility.labelWidth = oldLabelWidth;
        EditorGUILayout.EndScrollView();
    }

    private void DrawPreferencesTab()
    {
        DrawPreferencesSettings();
    }

    private void DrawPreferencesSettings() // Helper to manage PlayerPrefs.
    {
        EditorGUILayout.LabelField("Set Current Level", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Level", "The current level number."),
            GUILayout.Width(50));
        currentLevel = EditorGUILayout.IntField(currentLevel, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Set progress", GUILayout.Width(120), GUILayout.Height(30)))
            PlayerPrefs.SetInt("currentLevel", currentLevel);

        GUILayout.Space(15);

        EditorGUILayout.LabelField("PlayerPrefs", EditorStyles.boldLabel);
        if (GUILayout.Button("Delete PlayerPrefs", GUILayout.Width(120), GUILayout.Height(30)))
            PlayerPrefs.DeleteAll();
    }
}