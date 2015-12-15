using fun.Basics.Shapes;

namespace fun.Basics
{
    public interface ICollider
    {
        float? Intersects(Ray ray);
    }
}