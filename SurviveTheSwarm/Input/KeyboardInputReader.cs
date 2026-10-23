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

        MoveDirection = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            MoveDirection.Y = -1;

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            MoveDirection.Y = 1;

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            MoveDirection.X = -1;

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            MoveDirection.X = 1;

        IsFiring = keyboard.IsKeyDown(Keys.Space);
    }
}
