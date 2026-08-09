using CityBuilder.EventBus;

public readonly struct EntityDiedEvent : IEvent
{
    public readonly IDamageable Entity;

    public EntityDiedEvent(IDamageable entity)
    {
        Entity = entity;
    }
}
