using Microsoft.Xna.Framework;

namespace fun.Basics
{
    public interface ICollider
    {
        float? Intersects(Ray ray);
    }
}