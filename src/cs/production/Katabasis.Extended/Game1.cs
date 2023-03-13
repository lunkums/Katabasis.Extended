using bottlenoselabs.Katabasis;

namespace Katabasis.Extended
{
    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch = null!;

        public Game1()
        {
            Window.Title = "My Project";
            IsMouseVisible = false;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // Calling the base method is important for updating music!
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
        }
    }
}
