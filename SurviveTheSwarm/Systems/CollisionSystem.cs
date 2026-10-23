using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public class CollisionSystem
{
    public void ResolveCollisions(
        Player player,
        List<IEnemy> enemies,
        ScoreSystem scoreSystem,
        GameStateManager gameStateManager)
    {
        // Projectile hits enemy
        for (int i = player.Projectiles.Count - 1; i >= 0; i--)
        {
            var projectile = player.Projectiles[i];
            bool hitEnemy = false;

            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                var enemy = enemies[j];

                if (projectile.Bounds.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(projectile.Damage);

                    if (!enemy.IsAlive)
                    {
                        enemies.RemoveAt(j);
                        scoreSystem.Add(10);
                    }

                    player.Projectiles.RemoveAt(i);
                    hitEnemy = true;
                    break;
                }
            }

            if (hitEnemy)
                continue;
        }

        // Enemy touches player
        foreach (var enemy in enemies.ToList())
        {
            if (enemy.Bounds.Intersects(player.Bounds))
            {
                player.TakeDamage(25);

                if (player.Health <= 0)
                    gameStateManager.EndGame();

                if (!enemy.IsAlive)
                    enemies.Remove(enemy);
            }
        }
    }
}
