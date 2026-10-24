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
