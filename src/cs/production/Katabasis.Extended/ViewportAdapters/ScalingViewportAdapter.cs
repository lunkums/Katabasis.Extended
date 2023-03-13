using System.Numerics;

namespace Katabasis.Extended.ViewportAdapters
{
    public class ScalingViewportAdapter : ViewportAdapter
    {
        public ScalingViewportAdapter(int virtualWidth, int virtualHeight)
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
        }

        public override int VirtualWidth { get; }
        public override int VirtualHeight { get; }
        public override int ViewportWidth => GraphicsDevice.Viewport.Width;
        public override int ViewportHeight => GraphicsDevice.Viewport.Height;

        public override Matrix4x4 GetScaleMatrix()
        {
            var scaleX = (float) ViewportWidth/VirtualWidth;
            var scaleY = (float) ViewportHeight/VirtualHeight;
            return Matrix4x4.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}
