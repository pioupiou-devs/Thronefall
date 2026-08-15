using System;
using UnityEngine;

[Serializable]
public class ClosestTargetStrategy : ITargetingStrategy
{
    private readonly ClosestTargetStrategyData _data;

    public ClosestTargetStrategy() { } // required for [SerializeReference] type picker

    public ClosestTargetStrategy(ClosestTargetStrategyData data)
    {
        _data = data;
    }

    public Entity FindTarget(Entity self)
    {
        if (_data == null) return null;

        var candidates = UnityEngine.Object.FindObjectsByType<Entity>(FindObjectsInactive.Exclude);

        Entity closest = null;
        float minSqrDist = float.MaxValue;

        foreach (var candidate in candidates)
        {
            if (candidate == self) continue;

            // Only hunt the configured target faction
            if (candidate.Faction != _data.targetFaction) continue;

            // Skip destroyed/dead targets (e.g. a razed building or fallen unit)
            if (candidate.TryGetComponent<Health>(out var health) && health.isDead) continue;

            float sqrDist = (candidate.transform.position - self.transform.position).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = candidate;
            }
        }

        return closest;
    }
}
