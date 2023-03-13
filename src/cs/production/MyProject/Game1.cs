using System.Numerics;
using bottlenoselabs.Katabasis;

namespace MyProject
{
	// In C# game dev, prefer to use structs over classes; see https://github.com/bottlenoselabs/katabasis/blob/main/docs/GAMEDEV-CODING-BEST-PRACTICES.md
	public struct State // The state of the game; just data!
	{
		private const int MinimumAsteroids = 10;
		private const int MaximumAsteroids = 50000;
		
		public bool IsComplete;
		public TimeSpan IsCompleteElapsed;
		public int CurrentLevel;
		public int CurrentAsteroidsCount;
		public Vector2 MousePosition;
		public int MouseSize;
		public TimeSpan HitSoundTimer;

		public void NextLevel() // Just change the data!
		{
			IsComplete = false;
			IsCompleteElapsed = TimeSpan.Zero;
			CurrentLevel += 1;
			HitSoundTimer = TimeSpan.Zero;
			CurrentAsteroidsCount = (int)MathHelper.Clamp((float)Math.Pow(MinimumAsteroids, CurrentLevel), MinimumAsteroids, MaximumAsteroids);
			MouseSize = (int)((float)Math.Pow(CurrentLevel, 3) + 7);
		}
	}
	
	public struct Asteroid
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public Color Color;
		public Vector2 Size;
	}

	public class Game1 : Game
	{
		private readonly Random _random = new();
		private SpriteBatch _spriteBatch = null!;
		private State _state;

		private Asteroid[] _asteroids = null!;

		public Game1()
		{
			Window.Title = "My Project";
			IsMouseVisible = false;
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch();
			Content.Load();
			Reset();
			
			Content.Sounds.Greeting.Play();

			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(Content.Music.DnaLoop8Bit);
		}

		private void Reset()
		{
			_state.NextLevel();

			Array.Resize(ref _asteroids, _state.CurrentAsteroidsCount); // This can cause major lag if your hardware can't handle a very large number!
			for (var i = 0; i < _state.CurrentAsteroidsCount; i++)
			{
				var viewport = GraphicsDevice.Viewport;
				var x = RandomFloat(viewport.Width * 0.1f, viewport.Width * 0.9f);
				var y = RandomFloat(viewport.Height * 0.1f, viewport.Height * 0.9f);
				_asteroids[i].Position = new Vector2(x, y);
				_asteroids[i].Color = new Color(RandomFloat(0.5f, 1f), RandomFloat(0.5f, 1f), RandomFloat(0.5f, 1f));
				_asteroids[i].Size = new Vector2(RandomFloat(1, 2.5f), RandomFloat(1, 2.5f));
			}
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime); // Calling the base method is important for updating music!
			
			if (_state.IsComplete)
			{
				if (_state.IsCompleteElapsed == TimeSpan.Zero)
				{
					if (_state.CurrentLevel == 5)
					{
						Content.Sounds.GameOver.Play();
						_state.CurrentLevel = 0;
					}
					else
					{
						Content.Sounds.ObjectiveComplete.Play();
					}
				}
				
				_state.IsCompleteElapsed += gameTime.ElapsedGameTime;
				if (_state.IsCompleteElapsed.TotalSeconds >= 3.5f)
				{
					Reset();
				}

				return;
			}
			
			// The hit sound can stack and be really annoying; this prevents sound stacking by adding a timer
			if (_state.HitSoundTimer > TimeSpan.Zero)
			{
				_state.HitSoundTimer -= gameTime.ElapsedGameTime;
			}

			var mouseState = Mouse.GetState();
			_state.MousePosition = new Vector2(mouseState.X, mouseState.Y);
			
			var viewport = GraphicsDevice.Viewport;
			var centerScreenPosition = new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f);
			
			// Use a reverse loop so we can "remove" items from the array while we iterate by swapping them to the end
			// This works only because of two reasons: (1) we don't assume anything about the positions of the items in the array and (2) items at the end of array are already processed as the loop continues
			for (var i = _state.CurrentAsteroidsCount - 1; i >= 0; i--)
			{
				ref var asteroid = ref _asteroids[i];
				
				// Vectors: http://immersivemath.com/ila/ch02_vectors/ch02.html
				// The distance vector from A to B is: B - A
				var distanceVector = centerScreenPosition - asteroid.Position;
				var directionVector = Vector2.Zero;

				// When the distance vector has a value which is non-zero, normalize it so it's distance is always 1
				if (distanceVector != Vector2.Zero)
				{
					// The normalized distance vector is the direction vector
					directionVector = Vector2.Normalize(distanceVector);
				}

				var speed = asteroid.Size;
				var acceleration = directionVector * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
				asteroid.Velocity += acceleration;
				asteroid.Position += asteroid.Velocity;

				// Clamp the position so it never goes off screen
				var minimumPosition = Vector2.Zero;
				var maximumPosition = new Vector2(viewport.Width - asteroid.Size.X, viewport.Height - asteroid.Size.Y);
				_asteroids[i].Position = Vector2.Clamp(_asteroids[i].Position, minimumPosition, maximumPosition);

				// Basic collision detection
				var asteroidRectangle = new Rectangle((int)asteroid.Position.X, (int)asteroid.Position.Y, (int)asteroid.Size.X, (int)asteroid.Size.Y);
				var mouseRectangle = new Rectangle((int)_state.MousePosition.X, (int)_state.MousePosition.Y, _state.MouseSize, _state.MouseSize);
				var isHit = asteroidRectangle.Intersects(mouseRectangle);
				if (isHit)
				{
					if (_state.HitSoundTimer <= TimeSpan.Zero)
					{
						_state.HitSoundTimer = TimeSpan.FromSeconds(0.1f);
						Content.Sounds.Hit.Play();
					}

					_state.CurrentAsteroidsCount--;
					_asteroids[i] = _asteroids[_state.CurrentAsteroidsCount];
					
					if (_state.CurrentAsteroidsCount == 0)
					{
						_state.IsComplete = true;
						break;
					}
				}
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null, null);

			// Terran
			var centerScreenPosition = new Vector2(GraphicsDevice.Viewport.Width * 0.5f, GraphicsDevice.Viewport.Height * 0.5f);
			var origin = new Vector2(Content.Graphics.Terran.Width * 0.5f, Content.Graphics.Terran.Height * 0.5f);
			var angle = MathHelper.WrapAngle((float)gameTime.TotalGameTime.TotalSeconds);
			_spriteBatch.Draw(Content.Graphics.Terran, centerScreenPosition, null, Color.White, angle, origin, new Vector2(2), SpriteEffects.None, 0);
			
			// Asteroids
			for (var i = 0; i < _state.CurrentAsteroidsCount; i++)
			{
				var asteroid = _asteroids[i];
				_spriteBatch.Draw(Content.Graphics.Pixel, asteroid.Position, null, asteroid.Color, 0, Vector2.Zero,
					asteroid.Size, SpriteEffects.None, 0);
			}

			// Cursor
			var cursorColor = new Color(0.5f * (MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) + 1), 0.5f * (MathF.Cos((float)gameTime.TotalGameTime.TotalSeconds) + 1), 1);
			_spriteBatch.Draw(Content.Graphics.Pixel, _state.MousePosition, null, cursorColor, 0, Vector2.Zero, _state.MouseSize, SpriteEffects.None, 0);

			_spriteBatch.End();
		}

		private float RandomFloat(float minValue, float maxValue)
		{
			return (float)_random.NextDouble() * (maxValue - minValue) + minValue;
		}
	}
}
