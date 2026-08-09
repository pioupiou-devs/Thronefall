using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CityBuilder.EventBus
{
    /// <summary>
    /// Initialises all <see cref="EventBus{T}"/> instances at startup (triggering static
    /// constructors) and clears every bus when exiting play mode in the Editor.
    /// </summary>
    public static class EventBusUtil
    {
    public static IReadOnlyList<Type> EventTypes   { get; private set; }
    public static IReadOnlyList<Type> EventBusTypes { get; private set; }

#if UNITY_EDITOR
    public static PlayModeStateChange PlayModeState { get; private set; }

    [InitializeOnLoadMethod]
    public static void InitializeEditor()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        PlayModeState = state;
        if (state == PlayModeStateChange.ExitingPlayMode)
            ClearAllBuses();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        EventTypes  = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
        EventBusTypes = InitializeAllBuses();
    }

    static List<Type> InitializeAllBuses()
    {
        var busTypes = new List<Type>();
        var typedef  = typeof(EventBus<>);

        foreach (var eventType in EventTypes)
        {
            var busType = typedef.MakeGenericType(eventType);
            busTypes.Add(busType);
            Debug.Log($"[EventBusUtil] Initialized EventBus<{eventType.Name}>");
        }

        return busTypes;
    }

    public static void ClearAllBuses()
    {
        Debug.Log("[EventBusUtil] Clearing all buses…");
        if (EventBusTypes == null) return;

        foreach (var busType in EventBusTypes)
        {
            var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
            clearMethod?.Invoke(null, null);
        }
    }
}
}
