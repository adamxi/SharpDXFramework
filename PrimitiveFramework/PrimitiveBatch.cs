using SharpDX;
using SharpDX.Toolkit.Graphics;
using System.Linq;

namespace DXPrimitiveFramework
{
	public static class PrimitiveBatch
	{
		private static BasicEffect basicEffect;
		private static PrimitiveBatch<VertexPositionColor> batch;
		private static GraphicsDevice graphicsDevice;

		public static void Initialize(GraphicsDevice graphicsDevice)
		{
			PrimitiveBatch.graphicsDevice = graphicsDevice;
			graphicsDevice.SetBlendState(graphicsDevice.BlendStates.AlphaBlend);
			basicEffect = new BasicEffect(graphicsDevice);
			basicEffect.VertexColorEnabled = true;
			basicEffect.View = Matrix.Identity;
			basicEffect.World = Matrix.Identity;

			batch = new PrimitiveBatch<VertexPositionColor>(graphicsDevice);
		}

		public static int DrawCount { get; private set; }

		#region Begin overloads
		/// <summary>
		/// Must be called prior to any Draw() calls.
		/// Prepares the PrimitiveBatch.
		/// </summary>
		public static void Begin()
		{
			DrawCount = 0;
			basicEffect.Projection = Matrix.OrthoOffCenterRH(0f, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0f, 0f, 1f);
			ApplyEffect();
			batch.Begin();
		}

		/// <summary>
		/// Must be called prior to any Draw() calls.
		/// Prepares the PrimitiveBatch.
		/// </summary>
		/// <param name="projection">The projection.</param>
		public static void Begin(Matrix projection)
		{
			DrawCount = 0;
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
		public static void Begin(Matrix projection, Matrix view)
		{
			DrawCount = 0;
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
		public static void Begin(ref Matrix projection)
		{
			DrawCount = 0;
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
		public static void Begin(ref Matrix projection, ref Matrix view)
		{
			DrawCount = 0;
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
			basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		/// <summary>
		/// Draws a primitive.
		/// </summary>
		/// <param name="primitive">Primitive to draw.</param>
		public static void Draw(Primitive primitive)
		{
			primitive.InitializeForDrawing();
			_Draw(primitive);
		}

		private static void _Draw(Primitive primitive)
		{
			if (primitive is CompoundPrimitive)
			{
				(primitive as CompoundPrimitive).Primitives.ForEach(p => _Draw(p));
			}
			else
			{
				DrawCount++;
				batch.Draw(primitive.PrimitiveType, primitive.TransformedVertexPositionColors);
			}
		}
	}
}