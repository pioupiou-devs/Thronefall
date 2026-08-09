public readonly struct EntityDiedEvent : IEvent
{
    public readonly Entity Source;

    public EntityDiedEvent(Entity source)
    {
        Source = source;
    }
}
