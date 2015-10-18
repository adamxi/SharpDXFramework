using DXFramework;
using DXFramework.SceneManagement;
using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace XManager.Scenes
{
	/// <summary>
	/// Scene for displaying a loading screen when loading a game or going between maps or other scenes.
	/// </summary>
	public class Loading : Scene
	{
		private SpriteFont font;
		private Timer timer;

		public override void LoadContent()
		{
			font = Content.Load<SpriteFont>( "fonts/Debug" );
			timer = new Timer( 2000 );
		}

		public override void Update( GameTime gameTime )
		{
			timer.Update( gameTime );

			if( timer.Done )
			{
				//SceneManager.Set( typeof( WorldScene ).Name, new TransitionFade( 600, 200, TransitionFade.FadeStyle.FadeInOut ) );
			}

			if( InputManager.AnyMousePressed )
			{
				//SceneManager.Set( typeof( WorldScene ).Name );
				//SceneManager.Set(typeof(WorldScene).Name, new TransitionFade(600, 200, TransitionFade.FadeStyle.FadeInOut));
			}
		}

		public override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.Black );
			SpriteBatch.Begin();
			SpriteBatch.DrawString( font, "Loading Scene", new Vector2( 20, 20 ), Color.Red );
			SpriteBatch.End();
		}
	}
}