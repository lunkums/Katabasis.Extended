// ReSharper disable once CheckNamespace

using System.Numerics;

namespace Katabasis.Extended.ViewportAdapters
{
    public class DefaultViewportAdapter : ViewportAdapter
    {
        public override int VirtualWidth => GraphicsDevice.Viewport.Width;
        public override int VirtualHeight => GraphicsDevice.Viewport.Height;
        public override int ViewportWidth => GraphicsDevice.Viewport.Width;
        public override int ViewportHeight => GraphicsDevice.Viewport.Height;

        public override Matrix4x4 GetScaleMatrix()
        {
            return Matrix4x4.Identity;
        }
    }
}
