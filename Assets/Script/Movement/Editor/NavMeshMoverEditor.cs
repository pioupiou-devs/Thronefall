using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavMeshMover))]
public class NavMeshMoverEditor : Editor
{
    private SerializedProperty debugTargetPositionProperty;

    private void OnEnable()
    {
        debugTargetPositionProperty = serializedObject.FindProperty("debugTargetPosition");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);

        var mover = (NavMeshMover)target;

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (debugTargetPositionProperty != null)
                EditorGUILayout.PropertyField(debugTargetPositionProperty, new GUIContent("Target Position"));

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Set Destination"))
            {
                var pos = debugTargetPositionProperty != null
                    ? debugTargetPositionProperty.vector3Value
                    : Vector3.zero;
                mover.SetDestination(pos);
                Debug.Log($"[NavMeshMoverEditor] SetDestination → {pos} on '{mover.name}'.", mover);
            }

            if (GUILayout.Button("Stop"))
            {
                mover.Stop();
                Debug.Log($"[NavMeshMoverEditor] Stop on '{mover.name}'.", mover);
            }

            EditorGUILayout.EndHorizontal();

            // _agent is null until Awake; only read runtime state in play mode
            if (Application.isPlaying)
                EditorGUILayout.LabelField("Is Moving", mover.IsMoving.ToString());
        }

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Enter Play Mode to use debug controls.", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}
