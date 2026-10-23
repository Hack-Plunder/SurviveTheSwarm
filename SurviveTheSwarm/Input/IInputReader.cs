using Microsoft.Xna.Framework;

namespace SurviveTheSwarm;

public interface IInputReader
{
    Vector2 MoveDirection { get; }
    bool IsFiring { get; }
    void Update();
}
