using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;

namespace DXFramework
{
	public static class Engine
	{
		public static Game Game { get; private set; }
		public static ContentManager Content { get; private set; }
		public static SpriteFont DefaultFont { get; private set; }
		public static Vector2 ScreenSize { get; private set; }
		public static Texture2D Texture1x1 { get; private set; }

		public static void Initialize(Game game)
		{
			Game = game;
			Content = game.Content;
			DefaultFont = Content.Load<SpriteFont>("Fonts/Debug");

			Texture1x1 = Texture2D.New(game.GraphicsDevice, 1, 1, PixelFormat.B8G8R8A8.UNorm);
			Texture1x1.SetData(new Color[] { Color.White });

			UpdateScreenSize();
		}

		public static void UpdateScreenSize()
		{
			ScreenSize = new Vector2(Game.GraphicsDevice.BackBuffer.Width, Game.GraphicsDevice.BackBuffer.Height);
		}
	}
}