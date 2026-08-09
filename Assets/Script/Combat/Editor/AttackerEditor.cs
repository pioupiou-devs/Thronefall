using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Attack))]
[CanEditMultipleObjects]
public class AttackerEditor : Editor
{
    private SerializedProperty _strategyDataProp;
    private SerializedProperty _strategySelectorProp;

    private Type[] _strategyTypes;
    private string[] _strategyNames;

    private void OnEnable()
    {
        _strategyDataProp = serializedObject.FindProperty("_strategyData");
        _strategySelectorProp = serializedObject.FindProperty("_strategySelector");

        _strategyTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IAttackStrategy).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .ToArray();

        _strategyNames = _strategyTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_strategyDataProp);

        int currentIndex = GetCurrentStrategyIndex();
        int selectedIndex = EditorGUILayout.Popup("Strategy", currentIndex, _strategyNames);

        if (selectedIndex != currentIndex)
            SetStrategy(_strategyTypes[selectedIndex]);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Try Attack (Current Target)"))
            {
                var attacker = (Attack)target;
                var targeting = attacker.GetComponent<Targeting>();
                if (targeting == null || targeting.CurrentTarget == null)
                {
                    Debug.LogWarning($"[AttackerEditor] No current target on '{attacker.name}'. Add a Targeting component and call Refresh first.", attacker);
                }
                else
                {
                    bool hit = attacker.TryAttack(targeting.CurrentTarget);
                    Debug.Log($"[AttackerEditor] TryAttack on '{targeting.CurrentTarget.name}': {(hit ? "hit" : "missed (range or cooldown)")}.", attacker);
                }
            }
        }

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Enter Play Mode to use debug controls.", MessageType.Info);
    }

    private int GetCurrentStrategyIndex()
    {
        string managedTypeName = _strategySelectorProp.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(managedTypeName)) return -1;

        string typeName = managedTypeName.Split(' ').Last();
        return Array.FindIndex(_strategyTypes, t => t.FullName == typeName || t.Name == typeName);
    }

    private void SetStrategy(Type type)
    {
        _strategySelectorProp.managedReferenceValue = Activator.CreateInstance(type);
    }
}
