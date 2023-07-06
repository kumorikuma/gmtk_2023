using UnityEditor;
using UnityEngine;

public class ReactBuildLogWindow : EditorWindow {
    public static string BuildLog = "";

    [MenuItem("React/Build Log")]
    public static void ShowWindow() {
        GetWindow<ReactBuildLogWindow>("React Build Log");
    }

    private void OnGUI() {
        EditorGUILayout.LabelField("React Build Log", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(BuildLog, GUILayout.ExpandHeight(true));
    }

    public static void AddLine(string text) {
        BuildLog += text + "\n";
    }

    public static void ClearLog() {
        BuildLog = "";
    }
}