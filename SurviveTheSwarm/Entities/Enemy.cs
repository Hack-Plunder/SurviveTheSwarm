using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public interface IEnemy
{
    Vector2 Position { get; }
    Rectangle Bounds { get; }
    bool IsAlive { get; }
    void Update(float deltaTime, Vector2 playerPosition);
    void Draw(SpriteBatch spriteBatch);
    void TakeDamage(int amount);
}

public class Enemy : IEnemy
{
    private readonly float _speed;
    private int _health;

    public Enemy(Vector2 position, float speed, int health)
    {
        Position = position;
        _speed = speed;
        _health = health;
    }

    public Vector2 Position { get; private set; }
    public Rectangle Bounds => new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);
    public bool IsAlive => _health > 0;

    public void Update(float deltaTime, Vector2 playerPosition)
    {
        var direction = playerPosition - Position;
        if (direction != Vector2.Zero)
            direction.Normalize();

        Position += direction * _speed * deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ShapeRenderer.DrawRectangle(spriteBatch, Bounds, Color.Red);
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
    }
}
