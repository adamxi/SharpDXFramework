using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.SceneManagement.Transitions
{
	/// <summary>
	/// Transition class which can fade in various ways.
	/// </summary>
	public class TransitionFade : Transition
	{
		private byte targetAlpha;
		private byte fromAlpha;
		private int fadeDuration;
		private int halfwayDuration;
		private float elapsed;
		private Color color;
		private Texture2D gradient;
		private FadeStyle style;
		private Rectangle screenRect;

		/// <summary>
		/// Transiton that fades to black between scenes.
		/// </summary>
		/// <param name="fadeDuration">Duration of each fade in milliseconds.</param>
		/// <param name="halfwayDuration">Duration to wait before starting to fade out in milliseconds. Only relevant if fadeStyle is set to 'FadeInOut', otherwise set to 0.</param>
		/// <param name="fadeStyle">How to fade the scene.</param>
		public TransitionFade( int fadeDuration = 1000, int halfwayDuration = 500, FadeStyle fadeStyle = FadeStyle.FadeInOut )
		{
			this.fadeDuration = fadeDuration;
			this.halfwayDuration = halfwayDuration;
			this.style = fadeStyle;
		}

		public override void LoadContent()
		{
			color = Color.White;
			switch( style )
			{
				case FadeStyle.FadeInOut:
				case FadeStyle.FadeIn:
					fromAlpha = 0;
					targetAlpha = 255;
					break;

				case FadeStyle.FadeOut:
					fromAlpha = 255;
					targetAlpha = 0;
					break;
			}

			screenRect = new Rectangle( 0, 0, sceneManager.ViewportWidth, sceneManager.ViewportHeight );
			gradient = Texture2D.New( GraphicsDevice, 1, 1, PixelFormat.B8G8R8A8.UNorm );
			gradient.SetData( new Color[] { Color.Black } );
			base.LoadContent();
		}

		#region Logic
		public override void Update( GameTime gameTime )
		{
			int deltaTime = game.TargetElapsedTime.Milliseconds;
			elapsed += deltaTime;

			switch( style )
			{
				case FadeStyle.FadeIn:
				case FadeStyle.FadeOut:
					OneWayFade( deltaTime );
					break;

				case FadeStyle.FadeInOut:
					TwoWayFade( deltaTime );
					break;
			}
		}

		private void OneWayFade( int deltaTime )
		{
			switch( State )
			{
				case TransitionState.Intro:
					if( elapsed + deltaTime > fadeDuration )
					{
						State = TransitionState.Halfway;
						color.A = targetAlpha;
						SceneManager.AdvanceScene();
						UnloadContent();
					}
					else
					{
						float step = elapsed / fadeDuration;
						color.A = MathUtil.Lerp( fromAlpha, targetAlpha, step );
					}
					break;
			}
		}

		private void TwoWayFade( int deltaTime )
		{
			switch( State )
			{
				case TransitionState.Intro:
					if( elapsed + deltaTime > fadeDuration )
					{
						State = TransitionState.Halfway;
						elapsed = 0;
						color.A = targetAlpha;
						SceneManager.AdvanceScene();
					}
					else
					{
						float step = elapsed / fadeDuration;
						color.A = MathUtil.Lerp( 0, targetAlpha, step );
					}
					break;

				case TransitionState.Halfway:
					if( elapsed + deltaTime > halfwayDuration )
					{
						State = TransitionState.Outro;
						elapsed = 0;
					}
					break;

				case TransitionState.Outro:
					if( elapsed >= fadeDuration )
					{
						color.A = 0;
						UnloadContent();
					}
					else
					{
						float step = elapsed / fadeDuration;
						color.A = MathUtil.Lerp( targetAlpha, 0, step );
					}
					break;
			}
		}

		public override void Draw( GameTime gameTime )
		{
			SpriteBatch.Begin();
			SpriteBatch.Draw( gradient, screenRect, color );
			SpriteBatch.End();
		}
		#endregion

		public enum FadeStyle
		{
			/// <summary>
			/// During this transition the scene will only fade in.
			/// </summary>
			FadeIn,

			/// <summary>
			/// During this transition the scene will only fade out.
			/// </summary>
			FadeOut,

			/// <summary>
			/// During this transition the scene will fade in, wait a little, and then fade out.
			/// </summary>
			FadeInOut
		}
	}
}