using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IProjectileFactory
{
    Projectile Create(Vector2 position, Vector2 direction);
}
