using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameEditor : EditorWindow
{
	private readonly List<EditorTab> tabs = new List<EditorTab>();

	private int selectedTabIndex = -1;
	private int prevSelectedTabIndex = -1;

	[MenuItem("Level/Editor", false, 0)]
	private static void Init()
	{
		EditorWindow window = GetWindow(typeof(GameEditor));
		window.titleContent = new GUIContent("Game Editor");
	}

	private void OnEnable() // Add our tabs to editor list then set their parent editor by constructor.
	{
		tabs.Add(new LevelEditorTab(this));
		tabs.Add(new SettingsTab(this));
		selectedTabIndex = 0;
	}

	private void OnGUI() // Draw the editor
	{
		selectedTabIndex = GUILayout.Toolbar(selectedTabIndex,
			new[] { "Level Editor", "Settings"});
		if (selectedTabIndex >= 0 && selectedTabIndex < tabs.Count)
		{
			EditorTab selectedEditor = tabs[selectedTabIndex];
			if (selectedTabIndex != prevSelectedTabIndex)
			{
				selectedEditor.OnTabSelected();
				GUI.FocusControl(null);
			}
			selectedEditor.Draw();
			prevSelectedTabIndex = selectedTabIndex;
		}
	}
}
