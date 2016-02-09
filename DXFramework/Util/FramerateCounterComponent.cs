using System;
using System.Globalization;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.Util
{
	/// <summary>
	/// Displays the FPS
	/// </summary>
	public class FrameRateCounter : GameSystem
	{
		private static readonly TimeSpan oneSec = TimeSpan.FromSeconds(1);
		private TimeSpan elapsedTime = TimeSpan.Zero;
		private NumberFormatInfo format;
		private int frameCounter;
		private int frameRate;
		private Vector2 position;
		private SpriteBatch spriteBatch;
		private SpriteFont font;
		private string fpsString;

		public FrameRateCounter( Game game ) : base( game ) { }

		public override void Initialize()
		{
			base.Initialize();
			Enabled = true;
			Visible = true;

			spriteBatch = new SpriteBatch( GraphicsDevice );
			format = new NumberFormatInfo();
			format.NumberDecimalSeparator = ".";
			font = Content.Load<SpriteFont>( "Fonts/Debug" );
			fpsString = string.Format( format, "{0}", frameRate );

			GraphicsDeviceManager graphicsDeviceManager = Content.ServiceProvider.GetService( typeof( IGraphicsDeviceManager ) ) as GraphicsDeviceManager;
			position = new Vector2( graphicsDeviceManager.PreferredBackBufferWidth - 40, 20 );
		}

		public override void Update( GameTime gameTime )
		{
			elapsedTime += gameTime.ElapsedGameTime;

			if( elapsedTime <= oneSec)
			{
				return;
			}

			elapsedTime -= oneSec;
			frameRate = frameCounter;
			frameCounter = 0;
			fpsString = string.Format( format, "{0}", frameRate );
		}

		public override void Draw( GameTime gameTime )
		{
			frameCounter++;

			spriteBatch.Begin();
			spriteBatch.DrawString( font, fpsString, position + Vector2.One, Color.Black );
			spriteBatch.DrawString( font, fpsString, position, Color.White );
			spriteBatch.End();
		}
	}
}