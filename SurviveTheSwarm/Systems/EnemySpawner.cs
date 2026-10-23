using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SurviveTheSwarm;

public class EnemySpawner
{
    private readonly IEnemyFactory _enemyFactory;
    private readonly float _spawnInterval;
    private float _spawnTimer;

    public EnemySpawner(IEnemyFactory enemyFactory, float spawnInterval)
    {
        _enemyFactory = enemyFactory;
        _spawnInterval = spawnInterval;
    }

    public List<IEnemy> Enemies { get; } = new();

    public void Update(float deltaTime, Vector2 playerPosition)
    {
        _spawnTimer -= deltaTime;

        if (_spawnTimer <= 0)
        {
            Enemies.Add(_enemyFactory.Create(playerPosition));
            _spawnTimer = _spawnInterval;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in Enemies)
            enemy.Draw(spriteBatch);
    }
}
