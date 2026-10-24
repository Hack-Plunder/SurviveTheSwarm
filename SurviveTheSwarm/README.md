# Survive the Swarm

Below is a student-friendly, step-by-step MonoGame setup for “Survive the Swarm,” written so you can copy/paste commands and code directly. It assumes nothing is installed yet.

The demo is designed to teach recreate this repo from scratch:
- MonoGame basics
- C# OOP basics
- SOLID principles as they appear in code

We will show the exact point where each SOLID principle is implemented.

## 1) What we are building

A simple top-down shooter:
- Player moves with WASD or arrow keys
- Player fires with Space
- Enemies spawn in waves
- Enemies move toward the player
- Bullets hit enemies
- Enemies hit the player
- Score increases
- Game ends when health reaches zero

## 2) Install prerequisites

### Windows
Open PowerShell and run:
```powershell
winget install --id Microsoft.DotNet.SDK.10 --source winget
winget install --id Microsoft.VisualStudioCode --source winget
```

Then install the VS Code C# extension:
- Open VS Code
- Go to Extensions
- Install: C# and C# Dev Kit

Then install MonoGame template:
```powershell
dotnet new install MonoGame.Templates.CSharp
```

### macOS
Open Terminal and run:
```bash
brew install --cask visual-studio-code
brew install --cask dotnet-sdk
```

Then install the C# extension in VS Code:
- Open VS Code
- Install: C# and C# Dev Kit

Then install MonoGame template:
```bash
dotnet new install MonoGame.Templates.CSharp
```

### Linux (Ubuntu/Debian)
Open Terminal and run:
```bash
sudo apt update
sudo apt install -y dotnet-sdk-10.0 code
```

Then install the VS Code C# extension:
- Open VS Code
- Install: C# and C# Dev Kit

Then install MonoGame template:
```bash
dotnet new install MonoGame.Templates.CSharp
```

### VS Code Configuration
```bash

code --profile "MonoGame"
code --profile "MonoGame" --install-extension ms-dotnettools.csdevkit
code --profile "MonoGame" --install-extension ms-vscode.mono-debug
code --profile "MonoGame" --install-extension r88.monogame

```


What these commands do:
- dotnet SDK gives you the C# compiler and tools
- VS Code is your code editor
- C# extension helps with IntelliSense, debugging, and code completion
- MonoGame template gives you the project starter for a game

3) Create the game project

Windows PowerShell
```powershell
mkdir SurviveTheSwarm
cd SurviveTheSwarm
dotnet new mgdesktopgl -o SurviveTheSwarm
cd SurviveTheSwarm
dotnet restore
```

macOS/Linux
```bash
mkdir SurviveTheSwarm
cd SurviveTheSwarm
dotnet new mgdesktopgl -o SurviveTheSwarm
cd SurviveTheSwarm
dotnet restore
```

What this does:
- Creates a working MonoGame DesktopGL project
- Creates a starter `Game1.cs`
- Restores NuGet packages required for MonoGame

Now open the project:
```powershell
code --profile MonoGame .
```
or
```bash
code --profile MonoGame .
```

4) Understand the starter project

The template creates these files:
- Program.cs
- Game1.cs
- Content folder
- SurviveTheSwarm.csproj

We will replace the starter files with our own class structure.

5) Create the folder structure

Windows PowerShell
```powershell
New-Item -ItemType Directory -Path "Input" -Force
New-Item -ItemType Directory -Path "Entities" -Force
New-Item -ItemType Directory -Path "Factories" -Force
New-Item -ItemType Directory -Path "Systems" -Force
New-Item -ItemType Directory -Path "UI" -Force
```

macOS/Linux
```bash
mkdir -p Input Entities Factories Systems UI
```

What this does:
- Keeps code organized
- Makes it easier to explain OOP responsibilities
- Lets us demonstrate SRP (Single Responsibility Principle)

6) Replace Program.cs

This file is tiny and should stay tiny.

Windows PowerShell:
```powershell
@'
using var game = new Game1();
game.Run();
'@ | Set-Content -Path "Program.cs"
```

macOS/Linux:
```bash
cat > Program.cs <<'EOF'
using var game = new Game1();
game.Run();
EOF
```

Code explanation:
- `using var game = new Game1();` creates the game object
- `game.Run();` starts MonoGame’s game loop
- This is the standard MonoGame entry point

7) Create the shape helper

Create file: ShapeRenderer.cs

Windows PowerShell:
```powershell
@'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public static class ShapeRenderer
{
    private static Texture2D _pixel = null!;

    public static void Initialize(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        spriteBatch.Draw(_pixel, rect, color);
    }
}
'@ | Set-Content -Path "ShapeRenderer.cs"
```

macOS/Linux:
```bash
cat > ShapeRenderer.cs <<'EOF'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public static class ShapeRenderer
{
    private static Texture2D _pixel = null!;

    public static void Initialize(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        spriteBatch.Draw(_pixel, rect, color);
    }
}
EOF
```

Code explanation:
- This gives us a simple way to draw boxes using raw colored rectangles
- No SpriteSheet or image asset needed
- This keeps the game quick and easy to build

This is a good moment to explain:
- SRP: this class is only responsible for drawing shapes
- It does one thing, and it does it well

8) Create input abstraction

File: Input/IInputReader.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IInputReader
{
    Vector2 MoveDirection { get; }
    bool IsFiring { get; }
    void Update();
}
'@ | Set-Content -Path "Input\IInputReader.cs"
```

macOS/Linux:
```bash
cat > Input/IInputReader.cs <<'EOF'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IInputReader
{
    Vector2 MoveDirection { get; }
    bool IsFiring { get; }
    void Update();
}
EOF
```

Code explanation:
- This abstraction lets the game read input without caring how input is implemented
- Later we can swap keyboard input for AI input or a controller system

This is where we begin demonstrating:
- Dependency Inversion Principle (DIP): high-level systems depend on abstractions, not concrete input code
- Interface Segregation Principle (ISP): this interface is small and focused

9) Create concrete input reader

File: Input/KeyboardInputReader.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SurviveTheSwarm;

public class KeyboardInputReader : IInputReader
{
    public Vector2 MoveDirection { get; private set; }
    public bool IsFiring { get; private set; }

    public void Update()
    {
        var keyboard = Keyboard.GetState();

        var direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            direction.Y = -1;

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            direction.Y = 1;

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            direction.X = -1;

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            direction.X = 1;

        MoveDirection = direction;
        IsFiring = keyboard.IsKeyDown(Keys.Space);
    }
}
'@ | Set-Content -Path "Input\KeyboardInputReader.cs"
```

macOS/Linux:
```bash
cat > Input/KeyboardInputReader.cs <<'EOF'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SurviveTheSwarm;

public class KeyboardInputReader : IInputReader
{
    public Vector2 MoveDirection { get; private set; }
    public bool IsFiring { get; private set; }

    public void Update()
    {
        var keyboard = Keyboard.GetState();

        var direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            direction.Y = -1;

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            direction.Y = 1;

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            direction.X = -1;

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            direction.X = 1;

        MoveDirection = direction;
        IsFiring = keyboard.IsKeyDown(Keys.Space);
    }
}
EOF
```

Code explanation:
- `Keyboard.GetState()` reads the actual keys
- `MoveDirection` is a normalized direction vector, not a bunch of booleans
- `IsFiring` is separate and easy to understand

This is a good place to emphasize:
- SRP: this class only reads keyboard input
- DIP: Player depends on `IInputReader`, not on keyboard directly

10) Create base entity

File: Entities/Entity.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public abstract class Entity
{
    public Vector2 Position { get; protected set; }

    public virtual Rectangle Bounds =>
        new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);

    public virtual void Update(float deltaTime) { }

    public virtual void Draw(SpriteBatch spriteBatch) { }
}
'@ | Set-Content -Path "Entities\Entity.cs"
```

macOS/Linux:
```bash
cat > Entities/Entity.cs <<'EOF'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public abstract class Entity
{
    public Vector2 Position { get; protected set; }

    public virtual Rectangle Bounds =>
        new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);

    public virtual void Update(float deltaTime) { }

    public virtual void Draw(SpriteBatch spriteBatch) { }
}
EOF
```

Code explanation:
- `Entity` is a shared base class for objects in the world
- We avoid repeating common properties like position and bounds
- This is a basic OOP concept: inheritance

This is a good moment to explain:
- OOP: common behavior goes into a base class
- SRP: each entity does not decide all game rules, only its own properties

11) Create projectile

File: Entities/Projectile.cs

Windows:
```powershell
@'
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
'@ | Set-Content -Path "Entities\Projectile.cs"
```

macOS/Linux:
```bash
cat > Entities/Projectile.cs <<'EOF'
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
EOF
```

Code explanation:
- A projectile should know how to move, draw, and determine if it is alive
- This is one responsibility: projectile behavior
- It is not responsible for spawning itself or checking collisions

This introduces:
- SRP: projectile logic is separate

12) Create player

File: Entities/Player.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

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
'@ | Set-Content -Path "Entities\Player.cs"
```

macOS/Linux:
```bash
cat > Entities/Player.cs <<'EOF'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

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
EOF
```

Code explanation:
- Player is responsible for:
  - movement
  - health
  - shooting
  - projectile list handling
- It depends on `IInputReader` and `IProjectileFactory`
- This is a strong example of dependent abstraction and SRP

We also begin showing:
- DIP: Player does not need a concrete keyboard class
- SRP: movement logic is in Player, not Game1

13) Create enemy interface and enemy class

File: Entities/Enemy.cs

Windows:
```powershell
@'
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
'@ | Set-Content -Path "Entities\Enemy.cs"
```

macOS/Linux:
```bash
cat > Entities/Enemy.cs <<'EOF'
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
EOF
```

Code explanation:
- `IEnemy` defines a contract for any enemy
- `Enemy` implements that contract
- The game can treat all enemies the same because they share the same interface

This is a strong place to explain:
- Liskov Substitution Principle (LSP): any class implementing `IEnemy` can be used anywhere an `IEnemy` is expected
- OCP: later we can add `FastEnemy`, `TankEnemy`, or `ShooterEnemy` without rewriting the game loop

14) Create factories

File: Factories/IProjectileFactory.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IProjectileFactory
{
    Projectile Create(Vector2 position, Vector2 direction);
}
'@ | Set-Content -Path "Factories\IProjectileFactory.cs"
```

macOS/Linux:
```bash
cat > Factories/IProjectileFactory.cs <<'EOF'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IProjectileFactory
{
    Projectile Create(Vector2 position, Vector2 direction);
}
EOF
```

File: Factories/ProjectileFactory.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public class ProjectileFactory : IProjectileFactory
{
    public Projectile Create(Vector2 position, Vector2 direction)
    {
        return new Projectile(position, direction, 600f, 1);
    }
}
'@ | Set-Content -Path "Factories\ProjectileFactory.cs"
```

macOS/Linux:
```bash
cat > Factories/ProjectileFactory.cs <<'EOF'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public class ProjectileFactory : IProjectileFactory
{
    public Projectile Create(Vector2 position, Vector2 direction)
    {
        return new Projectile(position, direction, 600f, 1);
    }
}
EOF
```

File: Factories/IEnemyFactory.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IEnemyFactory
{
    IEnemy Create(Vector2 playerPosition);
}
'@ | Set-Content -Path "Factories\IEnemyFactory.cs"
```

macOS/Linux:
```bash
cat > Factories/IEnemyFactory.cs <<'EOF'
using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IEnemyFactory
{
    IEnemy Create(Vector2 playerPosition);
}
EOF
```

File: Factories/EnemyFactory.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;
using System;

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
'@ | Set-Content -Path "Factories\EnemyFactory.cs"
```

macOS/Linux:
```bash
cat > Factories/EnemyFactory.cs <<'EOF'
using Microsoft.Xna.Framework;
using System;

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
EOF
```

Code explanation:
- Factory classes are responsible for making objects
- This keeps monster creation or projectile creation out of the Player or Game1 class
- This is a clean way to teach:
  - SRP
  - OCP
  - DIP

15) Create enemy spawner

File: Systems/EnemySpawner.cs

Windows:
```powershell
@'
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
'@ | Set-Content -Path "Systems\EnemySpawner.cs"
```

macOS/Linux:
```bash
cat > Systems/EnemySpawner.cs <<'EOF'
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
EOF
```

Code explanation:
- This class handles an entire gameplay responsibility: enemy generation
- The game loop no longer needs to manually spawn enemies
- This is clean SRP at work

16) Create score and game-state systems

File: Systems/ScoreSystem.cs

Windows:
```powershell
@'
namespace SurviveTheSwarm;

public class ScoreSystem
{
    public int Score { get; private set; }

    public void Add(int amount)
    {
        Score += amount;
    }
}
'@ | Set-Content -Path "Systems\ScoreSystem.cs"
```

macOS/Linux:
```bash
cat > Systems/ScoreSystem.cs <<'EOF'
namespace SurviveTheSwarm;

public class ScoreSystem
{
    public int Score { get; private set; }

    public void Add(int amount)
    {
        Score += amount;
    }
}
EOF
```

File: Systems/GameStateManager.cs

Windows:
```powershell
@'
namespace SurviveTheSwarm;

public class GameStateManager
{
    public bool IsGameOver { get; private set; }

    public void EndGame()
    {
        IsGameOver = true;
    }
}
'@ | Set-Content -Path "Systems\GameStateManager.cs"
```

macOS/Linux:
```bash
cat > Systems/GameStateManager.cs <<'EOF'
namespace SurviveTheSwarm;

public class GameStateManager
{
    public bool IsGameOver { get; private set; }

    public void EndGame()
    {
        IsGameOver = true;
    }
}
EOF
```

Code explanation:
- Score logic and game-over logic are independent responsibilities
- This is a better design than having all of it in Game1
- This is SRP as a real-world example

17) Create collision system

File: Systems/CollisionSystem.cs

Windows:
```powershell
@'
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
'@ | Set-Content -Path "Systems\CollisionSystem.cs"
```

macOS/Linux:
```bash
cat > Systems/CollisionSystem.cs <<'EOF'
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
EOF
```

Code explanation:
- A collision system is a separate service
- It knows how objects interact
- It is not mixed into Player or Enemy
- This is a clean SRP example and also a good DIP example because Game1 can depend on `CollisionSystem`, not on collision logic embedded everywhere

18) Create the HUD

UI/Hud.cs

Windows:
```powershell
@'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public class Hud
{
    private readonly Player _player;
    private readonly ScoreSystem _scoreSystem;
    private readonly GameStateManager _gameStateManager;
    private readonly SpriteFont _font;

    public Hud(Player player, ScoreSystem scoreSystem, GameStateManager gameStateManager, SpriteFont font)
    {
        _player = player;
        _scoreSystem = scoreSystem;
        _gameStateManager = gameStateManager;
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var healthBarWidth = 200;
        var healthPercent = (float)_player.Health / _player.MaxHealth;
        var currentHealthWidth = (int)(healthBarWidth * healthPercent);

        ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(20, 20, healthBarWidth, 20), Color.DarkGray);
        ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(20, 20, currentHealthWidth, 20), Color.LimeGreen);

        spriteBatch.DrawString(_font, $"Health: {_player.Health}/{_player.MaxHealth}", new Vector2(230, 16), Color.White);
        spriteBatch.DrawString(_font, $"Score: {_scoreSystem.Score}", new Vector2(20, 50), Color.White);

        if (_gameStateManager.IsGameOver)
        {
            ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(250, 250, 780, 220), Color.Black);

            ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(320, 320, 620, 60), Color.Red);

            DrawCenteredText(spriteBatch, "GAME OVER", 275);
            DrawCenteredText(spriteBatch, $"Final score: {_scoreSystem.Score}", 335);
            DrawCenteredText(spriteBatch, "Press Enter to restart | Esc to quit", 410);
        }
    }

    private void DrawCenteredText(SpriteBatch spriteBatch, string text, float y)
    {
        float x = 640 - _font.MeasureString(text).X / 2;
        spriteBatch.DrawString(_font, text, new Vector2(x, y), Color.White);
    }
}
'@ | Set-Content -Path "UI\Hud.cs"
```

macOS/Linux:
```bash
cat > UI/Hud.cs <<'EOF'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public class Hud
{
    private readonly Player _player;
    private readonly ScoreSystem _scoreSystem;
    private readonly GameStateManager _gameStateManager;
    private readonly SpriteFont _font;

    public Hud(Player player, ScoreSystem scoreSystem, GameStateManager gameStateManager)
    {
        _player = player;
        _scoreSystem = scoreSystem;
        _gameStateManager = gameStateManager;
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var healthBarWidth = 200;
        var healthPercent = (float)_player.Health / _player.MaxHealth;
        var currentHealthWidth = (int)(healthBarWidth * healthPercent);

        ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(20, 20, healthBarWidth, 20), Color.DarkGray);
        ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(20, 20, currentHealthWidth, 20), Color.LimeGreen);

        spriteBatch.DrawString(_font, $"Health: {_player.Health}/{_player.MaxHealth}", new Vector2(230, 16), Color.White);
        spriteBatch.DrawString(_font, $"Score: {_scoreSystem.Score}", new Vector2(20, 50), Color.White);

        if (_gameStateManager.IsGameOver)
        {
            ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(250, 250, 780, 220), Color.Black);

            ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(320, 320, 620, 60), Color.Red);

            DrawCenteredText(spriteBatch, "GAME OVER", 275);
            DrawCenteredText(spriteBatch, $"Final score: {_scoreSystem.Score}", 335);
            DrawCenteredText(spriteBatch, "Press Enter to restart | Esc to quit", 410);
        }
    }

    private void DrawCenteredText(SpriteBatch spriteBatch, string text, float y)
    {
        float x = 640 - _font.MeasureString(text).X / 2;
        spriteBatch.DrawString(_font, text, new Vector2(x, y), Color.White);
    }
}
EOF
```

Code explanation:
- HUD is responsible only for drawing UI
- Not for scoring rules or collision logic
- This is a great example of SRP

19) Replace Game1.cs

Now replace the generated `Game1.cs` with the main game loop.

Windows:
```powershell
@'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SurviveTheSwarm;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private SpriteFont _hudFont = null!;
    private readonly IInputReader _inputReader;
    private readonly IProjectileFactory _projectileFactory;
    private readonly IEnemyFactory _enemyFactory;

    private Player _player = null!;
    private EnemySpawner _enemySpawner = null!;
    private CollisionSystem _collisionSystem = null!;
    private ScoreSystem _scoreSystem = null!;
    private GameStateManager _gameStateManager = null!;
    private Hud _hud = null!;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        Window.Title = "Survive the Swarm";

        _inputReader = new KeyboardInputReader();
        _projectileFactory = new ProjectileFactory();
        _enemyFactory = new EnemyFactory();
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        ShapeRenderer.Initialize(GraphicsDevice);
        _hudFont = Content.Load<SpriteFont>("HudFont");

        _scoreSystem = new ScoreSystem();
        _gameStateManager = new GameStateManager();

        _player = new Player(
            new Vector2(200, 360),
            260f,
            100,
            _inputReader,
            _projectileFactory);

        _enemySpawner = new EnemySpawner(_enemyFactory, 1.2f);
        _collisionSystem = new CollisionSystem();
        _hud = new Hud(_player, _scoreSystem, _gameStateManager, _hudFont);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_gameStateManager.IsGameOver)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                ResetGame();

            base.Update(gameTime);
            return;
        }

        _inputReader.Update();
        _player.Update(deltaTime);
        _enemySpawner.Update(deltaTime, _player.Position);

        _collisionSystem.ResolveCollisions(
            _player,
            _enemySpawner.Enemies,
            _scoreSystem,
            _gameStateManager);

        if (_player.Health <= 0)
            _gameStateManager.EndGame();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _player.Draw(_spriteBatch);
        _enemySpawner.Draw(_spriteBatch);
        _hud.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void ResetGame()
    {
        _scoreSystem = new ScoreSystem();
        _gameStateManager = new GameStateManager();

        _player = new Player(
            new Vector2(200, 360),
            260f,
            100,
            _inputReader,
            _projectileFactory);

        _enemySpawner = new EnemySpawner(_enemyFactory, 1.2f);
        _collisionSystem = new CollisionSystem();
        _hud = new Hud(_player, _scoreSystem, _gameStateManager, _hudFont);
    }
}
'@ | Set-Content -Path "Game1.cs"
```

macOS/Linux:
```bash
cat > Game1.cs <<'EOF'
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SurviveTheSwarm;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private SpriteFont _hudFont = null!;
    private readonly IInputReader _inputReader;
    private readonly IProjectileFactory _projectileFactory;
    private readonly IEnemyFactory _enemyFactory;

    private Player _player = null!;
    private EnemySpawner _enemySpawner = null!;
    private CollisionSystem _collisionSystem = null!;
    private ScoreSystem _scoreSystem = null!;
    private GameStateManager _gameStateManager = null!;
    private Hud _hud = null!;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        Window.Title = "Survive the Swarm";

        _inputReader = new KeyboardInputReader();
        _projectileFactory = new ProjectileFactory();
        _enemyFactory = new EnemyFactory();
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        ShapeRenderer.Initialize(GraphicsDevice);
        _hudFont = Content.Load<SpriteFont>("HudFont");

        _scoreSystem = new ScoreSystem();
        _gameStateManager = new GameStateManager();

        _player = new Player(
            new Vector2(200, 360),
            260f,
            100,
            _inputReader,
            _projectileFactory);

        _enemySpawner = new EnemySpawner(_enemyFactory, 1.2f);
        _collisionSystem = new CollisionSystem();
        _hud = new Hud(_player, _scoreSystem, _gameStateManager, _hudFont);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_gameStateManager.IsGameOver)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                ResetGame();

            base.Update(gameTime);
            return;
        }

        _inputReader.Update();
        _player.Update(deltaTime);
        _enemySpawner.Update(deltaTime, _player.Position);

        _collisionSystem.ResolveCollisions(
            _player,
            _enemySpawner.Enemies,
            _scoreSystem,
            _gameStateManager);

        if (_player.Health <= 0)
            _gameStateManager.EndGame();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _player.Draw(_spriteBatch);
        _enemySpawner.Draw(_spriteBatch);
        _hud.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void ResetGame()
    {
        _scoreSystem = new ScoreSystem();
        _gameStateManager = new GameStateManager();

        _player = new Player(
            new Vector2(200, 360),
            260f,
            100,
            _inputReader,
            _projectileFactory);

        _enemySpawner = new EnemySpawner(_enemyFactory, 1.2f);
        _collisionSystem = new CollisionSystem();
        _hud = new Hud(_player, _scoreSystem, _gameStateManager, _hudFont);
    }
}
EOF
```

Code explanation:
- This is the main game loop
- `Update` handles changing game state
- `Draw` renders everything
- `LoadContent` creates the game’s initial objects

This is where the audience sees “game logic orchestration” rather than “game logic buried in one giant file”

20) Build the project

Run this:
```bash
dotnet restore
dotnet build
dotnet run
```

Windows PowerShell and macOS/Linux both use the same command:
```bash
dotnet restore
dotnet build
dotnet run
```

What this does:
- restores dependencies
- compiles the code
- starts the game

21) Explain where each SOLID principle appears

Single Responsibility Principle (SRP)
- Player handles player logic only
- Enemy handles enemy logic only
- EnemySpawner handles spawning only
- CollisionSystem handles collision only
- Hud handles drawing only
- ScoreSystem handles score only

When this is implemented in the demo:
- After we create Player, Enemy, EnemySpawner, ScoreSystem, Hud, and CollisionSystem
- It becomes obvious that Game1 is no longer doing everything

Open/Closed Principle (OCP)
- We can add more enemy types later without modifying the game loop
- Example:
  - `FastEnemy : IEnemy`
  - `TankEnemy : IEnemy`
- The rest of the code does not need to change much

When this is implemented:
- After `IEnemy` is defined and the spawner creates enemies through `IEnemyFactory`

Liskov Substitution Principle (LSP)
- `Enemy` implements `IEnemy`
- Anything expecting `IEnemy` can receive an `Enemy`
- If we later add `FastEnemy` or `TankEnemy`, they should also be valid replacements

When this is implemented:
- Right after the `IEnemy` interface and enemy class are created

Interface Segregation Principle (ISP)
- We do not force a giant “everything” interface
- Instead we use small, purpose-built interfaces:
  - `IInputReader`
  - `IEnemy`
  - `IProjectileFactory`
  - `IEnemyFactory`

When this is implemented:
- After input and factory interfaces are created

Dependency Inversion Principle (DIP)
- `Player` depends on `IInputReader`, not on `KeyboardInputReader`
- `Game1` depends on interfaces for factories and input
- Higher-level code relies on abstractions, not concrete implementations

When this is implemented:
- Immediately after introducing `IInputReader` and the factory interfaces

22) Teaching flow for a live classroom

Use this presentation order:

Step 1: Show the bad version
- One class does everything
- You can explain “this works, but it is hard to change”

Step 2: Break out `Player`
- Player handles movement and shooting
- This demonstrates SRP

Step 3: Add `Enemy` and `IEnemy`
- This shows interface-based design and LSP

Step 4: Add `EnemySpawner`
- This demonstrates separated responsibilities

Step 5: Add `CollisionSystem`
- This demonstrates a service or manager class

Step 6: Add `IInputReader`
- This demonstrates DIP

Step 7: Add factories
- This demonstrates OCP and SRP

Step 8: Summarize SOLID
- Each class has one job
- High-level code depends on interfaces
- New behavior can be added without breaking the old code

23) Common beginner mistakes

- Putting everything in `Game1`
  - This is the classic “god class” problem
- Hard-coding keyboard calls directly inside Player
  - This breaks DIP
- Having every class inherit from every concept
  - This leads to big, fragile classes
- Making giant interfaces
  - This breaks ISP

24) Final expected gameplay controls

- W / Up = move up
- S / Down = move down
- A / Left = move left
- D / Right = move right
- Space = fire
- Escape = exit
- Enter = restart after game over

25) If you want the exact “lesson script,” use this outline

- “In the beginning, everything was in Game1.”
- “The class did too much. It was responsible for input, bullets, enemies, collisions, and score.”
- “That is why SOLID starts with SRP.”
- “Next, we moved movement and shooting into Player.”
- “Then we created a separate Enemy class and `IEnemy` interface.”
- “Now we can add FastEnemy and TankEnemy later without rewriting the main loop.”
- “We also used interface-based design so the game depends on behavior, not implementation.”
- “This is Dependency Inversion and Interface Segregation in action.”
- “The result is code that is easier to extend, easier to test, and easier to explain.”
