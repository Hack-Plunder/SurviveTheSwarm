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
