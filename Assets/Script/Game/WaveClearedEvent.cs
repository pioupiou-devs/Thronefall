public readonly struct WaveClearedEvent : IEvent
{
    public readonly int WaveIndex;

    public WaveClearedEvent(int waveIndex)
    {
        WaveIndex = waveIndex;
    }
}
