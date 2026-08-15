public readonly struct WaveStartEvent : IEvent
{
    public readonly int WaveIndex;

    public WaveStartEvent(int waveIndex)
    {
        WaveIndex = waveIndex;
    }
}
