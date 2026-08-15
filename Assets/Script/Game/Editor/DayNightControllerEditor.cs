using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DayNightController))]
[CanEditMultipleObjects]
public class DayNightControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var controller = (DayNightController)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Phase", controller.CurrentPhase.ToString());
        EditorGUILayout.LabelField("Wave", controller.CurrentWave.ToString());

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Start Night"))
                controller.StartNight();

            if (GUILayout.Button("Force Day"))
                controller.SetDay();
        }

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Enter Play Mode to use debug controls.", MessageType.Info);
    }
}
