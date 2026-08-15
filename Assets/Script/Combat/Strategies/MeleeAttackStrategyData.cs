using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttackStrategyData", menuName = "Combat/Attack/Melee")]
public class MeleeAttackStrategyData : AttackStrategyData
{
    public float damage = 10f;
    public float cooldown = 1f;
    public float range = 2f;
}
