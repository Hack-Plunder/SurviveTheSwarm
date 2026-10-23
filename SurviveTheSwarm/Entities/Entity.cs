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
