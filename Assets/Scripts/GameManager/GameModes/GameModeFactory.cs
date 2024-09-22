using System;

public class GameModeFactory
{
    public static IGameModeStrategy CreateStrategy(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.MainStory:
                return new MainStoryModeStrategy();
            case GameMode.Endless:
                return new EndlessModeStrategy();
            default:
                throw new ArgumentException("Invalid game mode");
        }
    }
}