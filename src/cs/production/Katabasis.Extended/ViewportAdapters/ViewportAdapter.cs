using bottlenoselabs.Katabasis;
using System.Numerics;

namespace Katabasis.Extended.ViewportAdapters
{
    public abstract class ViewportAdapter : IDisposable
    {
        public virtual void Dispose()
        {
        }

        public GraphicsDevice GraphicsDevice => GraphicsDevice.Instance;
        public Viewport Viewport => GraphicsDevice.Viewport;

        public abstract int VirtualWidth { get; }
        public abstract int VirtualHeight { get; }
        public abstract int ViewportWidth { get; }
        public abstract int ViewportHeight { get; }

        public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);
        // TODO: Implement
        // public Point Center => BoundingRectangle.Center;
        public abstract Matrix4x4 GetScaleMatrix();

        // TODO: Implement

        //public Point PointToScreen(Point point)
        //{
        //    return PointToScreen(point.X, point.Y);
        //}

        //public virtual Point PointToScreen(int x, int y)
        //{
        //    var scaleMatrix4x4 = GetScaleMatrix();
        //    var invertedMatrix4x4 = Matrix4x4.Invert(scaleMatrix);
        //    return Vector2.Transform(new Vector2(x, y), invertedMatrix).ToPoint();
        //}

        public virtual void Reset()
        {
        }
    }
}
