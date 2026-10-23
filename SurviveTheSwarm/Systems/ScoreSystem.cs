namespace SurviveTheSwarm;

public class ScoreSystem
{
    public int Score { get; private set; }

    public void Add(int amount)
    {
        Score += amount;
    }
}
