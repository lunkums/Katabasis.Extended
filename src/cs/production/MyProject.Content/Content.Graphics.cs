using bottlenoselabs.Katabasis;

namespace MyProject;

public static partial class Content
{
    public static class Graphics
    {
        public static Texture2D Pixel = null!;
        public static Texture2D Terran = null!;

        internal static void LoadGraphics()
        {
            Pixel = new Texture2D(1, 1);
            Pixel.SetData(new [] { Color.White });
            
            Terran = Texture2D.FromFile("assets/graphics/terran.png");
        }
    }   
}
