using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public class Projectile
{
    private readonly Vector2 _direction;
    private readonly float _speed;
    private readonly int _damage;

    public Projectile(Vector2 position, Vector2 direction, float speed, int damage)
    {
        Position = position;
        _direction = direction;
        _speed = speed;
        _damage = damage;
    }

    public Vector2 Position { get; private set; }
    public Rectangle Bounds => new Rectangle((int)Position.X - 4, (int)Position.Y - 4, 8, 8);
    public int Damage => _damage;
    public bool IsAlive { get; private set; } = true;

    public void Update(float deltaTime)
    {
        Position += _direction * _speed * deltaTime;

        if (Position.X < -20 || Position.X > 1300 || Position.Y < -20 || Position.Y > 820)
            IsAlive = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ShapeRenderer.DrawRectangle(spriteBatch, Bounds, Color.Yellow);
    }
}
