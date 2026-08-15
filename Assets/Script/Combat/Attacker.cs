using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Attack : MonoBehaviour
{
    [SerializeField] private AttackStrategyData _strategyData;
    [SerializeReference] private IAttackStrategy _strategySelector;

    private Entity _selfEntity;
    private IAttackStrategy _strategy;

    private void Awake()
    {
        if(_strategyData == null) throw new ArgumentNullException("Attack is missing data");

        _selfEntity = GetComponent<Entity>();
        _strategy = (IAttackStrategy)Activator.CreateInstance(_strategySelector.GetType(), args: _strategyData);
    }

    public bool TryAttack(Entity target) => _strategy != null && _strategy.TryAttack(_selfEntity, target);

    public bool IsInRange(Entity target) => _strategy != null && _strategy.IsInRange(_selfEntity, target);

    /// <summary>Attack range of the configured strategy data, or 0 if not a ranged-data type.</summary>
    public float Range =>
        _strategyData is MeleeAttackStrategyData melee ? melee.range : 0f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_strategySelector == null || _strategyData == null)
        {
            Debug.LogWarning($"[{nameof(Attack)}] Both a strategy and strategy data must be assigned on '{name}'.", this);
            return;
        }

        var ctor = _strategySelector.GetType().GetConstructor(new[] { _strategyData.GetType() });
        if (ctor == null)
            Debug.LogWarning($"[{nameof(Attack)}] '{_strategySelector.GetType().Name}' has no constructor accepting '{_strategyData.GetType().Name}' on '{name}'.", this);
    }

    private void OnDrawGizmosSelected()
    {
        float range = Range;
        if (range <= 0f) return;

        Gizmos.color = new Color(1f, 0.4f, 0.2f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
}
