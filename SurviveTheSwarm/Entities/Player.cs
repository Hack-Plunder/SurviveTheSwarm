using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SurviveTheSwarm;

public class Player : Entity
{
    private readonly IInputReader _inputReader;
    private readonly IProjectileFactory _projectileFactory;
    private readonly float _speed;
    private float _fireCooldown;
    private const float FireRate = 0.18f;

    public Player(
        Vector2 startPosition,
        float speed,
        int maxHealth,
        IInputReader inputReader,
        IProjectileFactory projectileFactory)
    {
        Position = startPosition;
        _speed = speed;
        MaxHealth = maxHealth;
        Health = maxHealth;
        _inputReader = inputReader;
        _projectileFactory = projectileFactory;
    }

    public int Health { get; private set; }
    public int MaxHealth { get; }
    public List<Projectile> Projectiles { get; } = new();

    public override void Update(float deltaTime)
    {
        var movement = _inputReader.MoveDirection;
        Position += movement * _speed * deltaTime;

        Position = new Vector2(
            MathHelper.Clamp(Position.X, 15, 1265),
            MathHelper.Clamp(Position.Y, 15, 705));

        _fireCooldown -= deltaTime;

        if (_inputReader.IsFiring && _fireCooldown <= 0)
        {
            Fire();
            _fireCooldown = FireRate;
        }

        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            var projectile = Projectiles[i];
            projectile.Update(deltaTime);

            if (!projectile.IsAlive)
                Projectiles.RemoveAt(i);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        ShapeRenderer.DrawRectangle(spriteBatch, Bounds, Color.Cyan);

        foreach (var projectile in Projectiles)
            projectile.Draw(spriteBatch);
    }

    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    private void Fire()
    {
        var projectile = _projectileFactory.Create(
            Position + new Vector2(0, -22),
            new Vector2(0, -1));

        Projectiles.Add(projectile);
    }
}
