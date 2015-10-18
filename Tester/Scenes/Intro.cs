using DXFramework;
using DXFramework.SceneManagement;
using DXFramework.SceneManagement.Transitions;
using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace XManager.Scenes
{
	/// <summary>
	/// Game intro and first scene to be displayed when the game is starting.
	/// </summary>
	public class Intro : Scene
	{
		private SpriteFont font;
		private Timer timer;

		public override void LoadContent()
		{
			font = Content.Load<SpriteFont>( "Fonts/Debug" );
			timer = new Timer( 5000 );
		}

		public override void Update( GameTime gameTime )
		{
			timer.Update( gameTime );

			if( timer.Done || InputManager.AnyMousePressed )
			{
				SceneManager.Set<PreMenu>( new TransitionFade() );
			}
		}

		public override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.Black );
			SpriteBatch.Begin();

			int sec = (int)( timer.Seconds + 1 );
			string timeString = sec.ToString();
			Vector2 timeSize = font.MeasureString( timeString );

			string text = "Tap screen to skip.";
			Vector2 textSize = font.MeasureString( text );

			SpriteBatch.DrawString( font, "Intro Scene", new Vector2( 20, 20 ), Color.Red );
			SpriteBatch.DrawString( font, timeString, new Vector2( ( sceneManager.ViewportWidth - timeSize.X ) / 2, sceneManager.ViewportHeight / 2 ), Color.Red );
			SpriteBatch.DrawString( font, text, new Vector2( ( sceneManager.ViewportWidth - textSize.X ) / 2, sceneManager.ViewportHeight - 60 ), Color.Red );

			SpriteBatch.End();
		}
	}
}