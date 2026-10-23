using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveTheSwarm;

public class Hud
{
    private readonly Player _player;
    private readonly ScoreSystem _scoreSystem;
    private readonly GameStateManager _gameStateManager;

    public Hud(Player player, ScoreSystem scoreSystem, GameStateManager gameStateManager)
    {
        _player = player;
        _scoreSystem = scoreSystem;
        _gameStateManager = gameStateManager;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var healthBarWidth = 200;
        var healthPercent = (float)_player.Health / _player.MaxHealth;
        var currentHealthWidth = (int)(healthBarWidth * healthPercent);

        ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(20, 20, healthBarWidth, 20), Color.DarkGray);
        ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(20, 20, currentHealthWidth, 20), Color.LimeGreen);

        ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(20, 50, 120, 20), Color.DarkSlateGray);

        if (_gameStateManager.IsGameOver)
        {
            ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(250, 250, 780, 220), Color.Black);

            ShapeRenderer.DrawRectangle(spriteBatch, new Rectangle(320, 320, 620, 60), Color.Red);
        }
    }
}
