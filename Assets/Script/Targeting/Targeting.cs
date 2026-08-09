using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Targeting : MonoBehaviour
{
    [SerializeField] private TargetingStrategyData _strategyData;
    // type picker only — its GetType() drives which strategy is instantiated at runtime
    [SerializeReference] private ITargetingStrategy _strategySelector;

    private Entity _selfEntity;
    private ITargetingStrategy _strategy;

    public Entity CurrentTarget { get; private set; }

    private void Awake()
    {
        _selfEntity = GetComponent<Entity>();
        _strategy = (ITargetingStrategy)Activator.CreateInstance(_strategySelector.GetType(), _strategyData);
    }

    public void Refresh()
    {
        CurrentTarget = _strategy.FindTarget(_selfEntity);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_strategySelector == null || _strategyData == null)
        {
            Debug.LogWarning($"[{nameof(Targeting)}] Both a strategy and strategy data must be assigned on '{name}'.", this);
            return;
        }

        var ctor = _strategySelector.GetType().GetConstructor(new[] { _strategyData.GetType() });
        if (ctor == null)
            Debug.LogWarning($"[{nameof(Targeting)}] '{_strategySelector.GetType().Name}' has no constructor accepting '{_strategyData.GetType().Name}' on '{name}'.", this);
    }
#endif
}
