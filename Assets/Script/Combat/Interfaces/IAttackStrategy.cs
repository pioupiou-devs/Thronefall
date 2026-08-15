public interface IAttackStrategy
{
    bool IsInRange(Entity self, Entity target);
    bool TryAttack(Entity self, Entity target);
}
