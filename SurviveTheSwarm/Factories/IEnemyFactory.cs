using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IEnemyFactory
{
    IEnemy Create(Vector2 playerPosition);
}
