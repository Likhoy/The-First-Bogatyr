
public static class GameProgressData
{
    #region Hints
    public static bool healthHintShown = false;
    #endregion

    #region Saves
    public static bool[] checkpointReached = new bool[Settings.numberOfCheckpoints];
    #endregion
}
