using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Targeting))]
[CanEditMultipleObjects]
public class TargetingEditor : Editor
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
            .Where(t => typeof(ITargetingStrategy).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
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
            if (GUILayout.Button("Find Closest Target"))
            {
                var targeting = (Targeting)target;
                targeting.Refresh();
                string result = targeting.CurrentTarget != null
                    ? targeting.CurrentTarget.name
                    : "None";
                Debug.Log($"[Targeting] Closest target on '{targeting.name}': {result}", targeting.CurrentTarget);
            }
        }

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Enter Play Mode to use debug controls.", MessageType.Info);
    }

    private int GetCurrentStrategyIndex()
    {
        string managedTypeName = _strategySelectorProp.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(managedTypeName)) return -1;

        // managedReferenceFullTypename format: "assembly typename"
        string typeName = managedTypeName.Split(' ').Last();
        return Array.FindIndex(_strategyTypes, t => t.FullName == typeName || t.Name == typeName);
    }

    private void SetStrategy(Type type)
    {
        var instance = Activator.CreateInstance(type);
        _strategySelectorProp.managedReferenceValue = instance;
    }
}
