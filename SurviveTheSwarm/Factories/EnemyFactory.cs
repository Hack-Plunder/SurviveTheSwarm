using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public class EnemyFactory : IEnemyFactory
{
    public IEnemy Create(Vector2 playerPosition)
    {
        var spawnX = Random.Shared.Next(0, 1280);
        var spawnY = Random.Shared.Next(0, 720);

        var speed = Random.Shared.Next(40, 100);
        var health = Random.Shared.Next(1, 3);

        return new Enemy(new Vector2(spawnX, spawnY), speed, health);
    }
}
