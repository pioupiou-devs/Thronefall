public readonly struct GameOverEvent : IEvent
{
    public readonly bool Victory;

    public GameOverEvent(bool victory)
    {
        Victory = victory;
    }
}
