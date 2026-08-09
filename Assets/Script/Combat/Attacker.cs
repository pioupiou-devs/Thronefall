using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Attacker : MonoBehaviour
{
    [SerializeField] private AttackStrategyData _strategyData;
    [SerializeReference] private IAttackStrategy _strategySelector;

    private Entity _selfEntity;
    private IAttackStrategy _strategy;

    private void Awake()
    {
        _selfEntity = GetComponent<Entity>();
        _strategy = (IAttackStrategy)Activator.CreateInstance(_strategySelector.GetType(), _strategyData);
    }

    public bool TryAttack(Entity target) => _strategy.TryAttack(_selfEntity, target);

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_strategySelector == null || _strategyData == null)
        {
            Debug.LogWarning($"[{nameof(Attacker)}] Both a strategy and strategy data must be assigned on '{name}'.", this);
            return;
        }

        var ctor = _strategySelector.GetType().GetConstructor(new[] { _strategyData.GetType() });
        if (ctor == null)
            Debug.LogWarning($"[{nameof(Attacker)}] '{_strategySelector.GetType().Name}' has no constructor accepting '{_strategyData.GetType().Name}' on '{name}'.", this);
    }
#endif
}
