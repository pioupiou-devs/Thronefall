using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    private SerializedProperty debugDamageAmountProperty;

    private void OnEnable()
    {
        debugDamageAmountProperty = serializedObject.FindProperty("debugDamageAmount");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Take Damage"))
            {
                var health = (Health)target;
                var damageAmount = debugDamageAmountProperty != null ? debugDamageAmountProperty.floatValue : 10f;
                health.TakeDamage(new Damage(damageAmount));
                Debug.Log($"[HealthEditor] TakeDamage button clicked on '{health.name}' with amount {damageAmount:0.##}.", health);
            }
        }

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to use the Take Damage debug button.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}