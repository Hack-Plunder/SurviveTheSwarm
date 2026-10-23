namespace SurviveTheSwarm;

public class GameStateManager
{
    public bool IsGameOver { get; private set; }

    public void EndGame()
    {
        IsGameOver = true;
    }
}
