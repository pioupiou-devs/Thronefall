using UnityEngine;

[CreateAssetMenu(fileName = "ClosestTargetStrategyData", menuName = "Targeting/Closest Target")]
public class ClosestTargetStrategyData : TargetingStrategyData
{
    [Tooltip("Faction this targeting will hunt. Entities of other factions are ignored.")]
    public Faction targetFaction = Faction.Enemy;
}
