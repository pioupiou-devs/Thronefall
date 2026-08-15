using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConsoleDebugger))]
[CanEditMultipleObjects]
public class ConsoleDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Event Triggers", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Raise WaveStartEvent"))
            {
                var dbg = (ConsoleDebugger)target;
                dbg.TriggerWaveStart();
            }

            if (GUILayout.Button("Raise EntityDiedEvent"))
            {
                var dbg = (ConsoleDebugger)target;
                dbg.TriggerEntityDied();
            }

            if (GUILayout.Button("Raise WaveClearedEvent"))
            {
                var dbg = (ConsoleDebugger)target;
                dbg.TriggerWaveCleared();
            }

            if (GUILayout.Button("Raise GameOverEvent"))
            {
                var dbg = (ConsoleDebugger)target;
                dbg.TriggerGameOver();
            }
        }

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Enter Play Mode to use event triggers.", MessageType.Info);
    }
}
