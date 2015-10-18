using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.PrimitiveFramework
{
	public static class PrimitiveBatch
	{
		private static BasicEffect basicEffect;
		private static PrimitiveBatch<VertexPositionColor> batch;
		private static GraphicsDevice graphicsDevice;

		public static void Initialize( Game game )
		{
			graphicsDevice = game.GraphicsDevice;
			graphicsDevice.SetBlendState( graphicsDevice.BlendStates.AlphaBlend );
			basicEffect = new BasicEffect( graphicsDevice );
			basicEffect.VertexColorEnabled = true;
			basicEffect.View = Matrix.Identity;
			basicEffect.World = Matrix.Identity;
			
			batch = new PrimitiveBatch<VertexPositionColor>( graphicsDevice );
		}

		#region Begin overloads
		/// <summary>
		/// Must be called prior to any Draw() calls.
		/// Prepares the PrimitiveBatch.
		/// </summary>
		public static void Begin()
		{
			basicEffect.Projection = Matrix.OrthoOffCenterRH( 0f, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0f, 0f, 1f );
			ApplyEffect();
			batch.Begin();
		}

		/// <summary>
		/// Must be called prior to any Draw() calls.
		/// Prepares the PrimitiveBatch.
		/// </summary>
		/// <param name="projection">The projection.</param>
		public static void Begin( Matrix projection )
		{
			basicEffect.Projection = projection;
			ApplyEffect();
			batch.Begin();
		}

		/// <summary>
		/// Must be called prior to any Draw() calls.
		/// Prepares the PrimitiveBatch.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="view">The view.</param>
		public static void Begin( Matrix projection, Matrix view )
		{
			basicEffect.Projection = projection;
			basicEffect.View = view;
			ApplyEffect();
			batch.Begin();
		}

		/// <summary>
		/// Must be called prior to any Draw() calls.
		/// Prepares the PrimitiveBatch.
		/// </summary>
		/// <param name="projection">The projection.</param>
		public static void Begin( ref Matrix projection )
		{
			basicEffect.Projection = projection;
			ApplyEffect();
			batch.Begin();
		}

		/// <summary>
		/// Must be called prior to any Draw() calls.
		/// Prepares the PrimitiveBatch.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="view">The view.</param>
		public static void Begin( ref Matrix projection, ref Matrix view )
		{
			basicEffect.Projection = projection;
			basicEffect.View = view;
			ApplyEffect();
			batch.Begin();
		}
		#endregion

		/// <summary>
		/// Will flush the vertex data stored in the buffer and submit it to the graphics card to be rendered.
		/// </summary>
		public static void End()
		{
			batch.End();
		}

		private static void ApplyEffect()
		{
			basicEffect.CurrentTechnique.Passes[ 0 ].Apply();
		}

		/// <summary>
		/// Draws a primitive.
		/// </summary>
		/// <param name="primitive">Primitive to draw.</param>
		public static void Draw( Primitive primitive )
		{
			primitive.Create();
			primitive.UpdateTransformation();
			batch.Draw( primitive.PrimitiveType, primitive.TransformedVertexPositionColors );
		}
	}
}