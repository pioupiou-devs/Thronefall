public readonly struct GamePhaseChangedEvent : IEvent
{
    public readonly GamePhase Phase;

    public GamePhaseChangedEvent(GamePhase phase)
    {
        Phase = phase;
    }
}
