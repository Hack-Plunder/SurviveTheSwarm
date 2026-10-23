using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public class ProjectileFactory : IProjectileFactory
{
    public Projectile Create(Vector2 position, Vector2 direction)
    {
        return new Projectile(position, direction, 600f, 1);
    }
}
