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
			Engine.Game = game;
			Engine.Content = game.Content;
			Engine.DefaultFont = Engine.Content.Load<SpriteFont>("Fonts/Debug");
			Engine.ScreenSize = new Vector2(game.GraphicsDevice.BackBuffer.Width, game.GraphicsDevice.BackBuffer.Height);

			Engine.Texture1x1 = Texture2D.New(game.GraphicsDevice, 1, 1, PixelFormat.B8G8R8A8.UNorm);
			Texture1x1.SetData(new Color[] { Color.White });
		}
	}
}