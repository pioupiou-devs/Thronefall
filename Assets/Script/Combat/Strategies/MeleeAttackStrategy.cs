using System;
using UnityEngine;

[Serializable]
public class MeleeAttackStrategy : IAttackStrategy
{
    private readonly MeleeAttackStrategyData _data;
    private float _lastAttackTime = float.MinValue;

    public MeleeAttackStrategy() { } // required for [SerializeReference] type picker

    public MeleeAttackStrategy(MeleeAttackStrategyData data)
    {
        _data = data;
    }

    public bool IsInRange(Entity self, Entity target)
    {
        if (_data == null || self == null || target == null) return false;
        return Vector3.Distance(self.transform.position, target.transform.position) <= _data.range;
    }

    public bool TryAttack(Entity self, Entity target)
    {
        if (_data == null || target == null) return false;

        if (Time.time - _lastAttackTime < _data.cooldown) return false;

        if (!target.TryGetComponent<IDamageable>(out var damageable)) return false;

        _lastAttackTime = Time.time;
        damageable.TakeDamage(new Damage(_data.damage));
        return true;
    }
}
