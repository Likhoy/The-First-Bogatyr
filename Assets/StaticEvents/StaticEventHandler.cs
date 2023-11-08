using System;

public static class StaticEventHandler 
{
    public static event Action<WaveFinishedEventArgs> OnWaveFinished;

    public static void CallWaveFinishedEvent(int waveNumber)
    {
        OnWaveFinished?.Invoke(new WaveFinishedEventArgs() { waveNumber = waveNumber });
    }
}

public class WaveFinishedEventArgs
{
    public int waveNumber;
}

