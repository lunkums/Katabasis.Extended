using bottlenoselabs.Katabasis;
using System.Numerics;

namespace Katabasis.Extended.ViewportAdapters
{
    public class WindowViewportAdapter : ViewportAdapter
    {
        protected readonly GameWindow Window;

        public WindowViewportAdapter(GameWindow window)
        {
            Window = window;
            window.ClientSizeChanged += OnClientSizeChanged;
        }

        public override int ViewportWidth => Window.ClientBounds.Width;
        public override int ViewportHeight => Window.ClientBounds.Height;
        public override int VirtualWidth => Window.ClientBounds.Width;
        public override int VirtualHeight => Window.ClientBounds.Height;

        public override Matrix4x4 GetScaleMatrix()
        {
            return Matrix4x4.Identity;
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var x = Window.ClientBounds.Width;
            var y = Window.ClientBounds.Height;

            GraphicsDevice.Viewport = new Viewport(0, 0, x, y);
        }
    }
}
