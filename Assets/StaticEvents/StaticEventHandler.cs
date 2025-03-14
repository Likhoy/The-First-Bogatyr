using System;

public static class StaticEventHandler 
{
    public static event Action<WaveFinishedEventArgs> OnWaveFinished;

    public static void CallWaveFinishedEvent(int waveNumber, WaveDetailsSO waveDetails)
    {
        OnWaveFinished?.Invoke(new WaveFinishedEventArgs() { waveNumber = waveNumber, waveDetails = waveDetails });
    }

    public static event Action<WaveLaunchedEventArgs> OnWaveLaunched;

    public static void CallWaveLaunchedEvent(int waveNumber)
    {
        OnWaveLaunched?.Invoke(new WaveLaunchedEventArgs() { waveNumber = waveNumber });
    }
}

public class WaveLaunchedEventArgs
{
    public int waveNumber;
}

public class WaveFinishedEventArgs
{
    public int waveNumber;
    public WaveDetailsSO waveDetails;
}

