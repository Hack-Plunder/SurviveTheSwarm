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
