using DXFramework;
using DXFramework.SceneManagement;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace XManager.Scenes
{
	/// <summary>
	/// Pre-menu is show right after the Intro scene. Should display a splash screen and some flashing "Tap screen" text.
	/// </summary>
	public class PreMenu : Scene
	{
		private SpriteFont font;

		public override void LoadContent()
		{
			font = Content.Load<SpriteFont>( "Fonts/Debug" );
		}

		public override void Update( GameTime gameTime )
		{
			if( IsTransitionDone() )
			{
				if( InputManager.AnyMousePressed )
				{
					SceneManager.Set<UITestSceneMain>();
				}
			}
		}

		public override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.Black );

			SpriteBatch.Begin();

			string text = "Tap screen to play!";
			Vector2 textSize = font.MeasureString( text );

			SpriteBatch.DrawString( font, "Pre-Menu Scene", new Vector2( 20, 20 ), Color.Red );
			SpriteBatch.DrawString( font, text, new Vector2( ( GraphicsDevice.Viewport.Width - textSize.X ) / 2, GraphicsDevice.Viewport.Height / 2 ), Color.Red );

			SpriteBatch.End();
		}
	}
}