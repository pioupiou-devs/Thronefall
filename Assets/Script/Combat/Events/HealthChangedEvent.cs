public readonly struct HealthChangedEvent : IEvent
{
    public readonly Entity Source;
    public readonly float Current;
    public readonly float Max;

    public HealthChangedEvent(Entity source, float current, float max)
    {
        Source = source;
        Current = current;
        Max = max;
    }
}
