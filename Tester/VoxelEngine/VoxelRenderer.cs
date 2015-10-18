/// Strikelimit - Jekev - C#/XNA Voxel Renderer - Marching Cubes
/// Code by Jekev - Based on Paul Bourke's Polygonising A Scalar Field Article
/// http://www.paulbourke.net/geometry/polygonise/
/// http://www.strikelimit.co.uk/m/
/// Anyone is free to do what they wish with this code from my perspective - but feel free to credit firstly Paul Bourke, and secondly me if you wish.

using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using VoxelEngine.Primitives;

namespace VoxelEngine
{
	public static class VoxelRenderer
	{
		private static BasicEffect effect;
		private static PrimitiveBatch<VertexPositionColorTextureNormal> batch;
		private static GraphicsDevice graphicsDevice;

		public static void Init( Game game )
		{
			VoxelRenderer.graphicsDevice = game.GraphicsDevice;
			int maxVertexSize = 1000000;
			batch = new PrimitiveBatch<VertexPositionColorTextureNormal>( game.GraphicsDevice, maxVertexSize * Utilities.SizeOf<VertexPositionColorTextureNormal>(), maxVertexSize );
			effect = new BasicEffect( game.GraphicsDevice );
		}

		public static bool Wireframe { get; set; }

		public static void Begin( Matrix view, Matrix projection )
		{
			Vector3 lightDirection = new Vector3( 1, 1, 0 );
			Vector3 lightColor = new Vector3( 0.3f, 0.4f, 0.2f );

			effect.View = view;
			effect.Projection = projection;
			effect.VertexColorEnabled = true;
			effect.LightingEnabled = true;
			effect.EnableDefaultLighting();
			effect.DirectionalLight0.Enabled = true;
			effect.DirectionalLight0.Direction = lightDirection;
			effect.DirectionalLight0.DiffuseColor = lightColor;
			effect.DirectionalLight0.SpecularColor = lightColor;
			effect.CurrentTechnique.Passes[ 0 ].Apply();

			if( Wireframe )
			{
				graphicsDevice.SetRasterizerState( graphicsDevice.RasterizerStates.WireFrameCullNone );
			}
			else
			{
				graphicsDevice.SetRasterizerState( graphicsDevice.RasterizerStates.CullNone );
			}

			batch.Begin();
		}

		public static void Draw( VoxelEngine.Primitives.GeometricPrimitive primitive )
		{
			primitive.Update();
			if( primitive is VoxelChunk )
			{
				batch.Draw( PrimitiveType.TriangleList, primitive.VerticeArray );
			}
			else
			{
				batch.DrawIndexed( PrimitiveType.TriangleList, primitive.IndiceArray, primitive.VerticeArray );
			}
		}

		public static void End()
		{
			batch.End();
		}

		//public static void RenderTriangles( List<Triangle> triangles, GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Color color )
		//{
		//	voxeltriangles = new VertexPositionColorTextureNormal[ 3 * triangles.Count ];

		//	if( effect == null )
		//	{
		//		effect = new BasicEffect( graphicsDevice );
		//	}

		//	int count = 0;
		//	foreach( Triangle triangle in triangles )
		//	{
		//		voxeltriangles[ count ] = new VertexPositionColorTextureNormal( triangle.pointOne, color, Vector2.Zero, Vector3.Zero );
		//		voxeltriangles[ count + 1 ] = new VertexPositionColorTextureNormal( triangle.pointTwo, color, Vector2.Zero, Vector3.Zero );
		//		voxeltriangles[ count + 2 ] = new VertexPositionColorTextureNormal( triangle.pointThree, color, Vector2.Zero, Vector3.Zero );

		//		for( int index = 0; index < 3; index += 3 )
		//		{
		//			Vector3 sideA = voxeltriangles[ count + index ].Position - voxeltriangles[ count + index + 2 ].Position;
		//			Vector3 sideB = voxeltriangles[ count + index ].Position - voxeltriangles[ count + index + 1 ].Position;
		//			Vector3 firstNormal = Vector3.Cross( sideA, sideB );
		//			firstNormal.Normalize();

		//			voxeltriangles[ count + index ].Normal += firstNormal;
		//			voxeltriangles[ count + index + 1 ].Normal += firstNormal;
		//			voxeltriangles[ count + index + 2 ].Normal += firstNormal;

		//			if( firstNormal.Y < -0.7f )
		//			{
		//				voxeltriangles[ count + index ].Color = Color.Green;
		//				voxeltriangles[ count + index + 1 ].Color = Color.Green;
		//				voxeltriangles[ count + index + 2 ].Color = Color.Green;
		//			}
		//			else
		//			{
		//				voxeltriangles[ count + index ].Color = Color.Gray;
		//				voxeltriangles[ count + index + 1 ].Color = Color.Gray;
		//				voxeltriangles[ count + index + 2 ].Color = Color.Gray;
		//			}

		//			voxeltriangles[ count + index ].Normal.Normalize();
		//			voxeltriangles[ count + index + 1 ].Normal.Normalize();
		//			voxeltriangles[ count + index + 2 ].Normal.Normalize();
		//		}

		//		count += 3;
		//	}

		//	effect.View = view;
		//	effect.Projection = projection;
		//	effect.VertexColorEnabled = true;
		//	effect.LightingEnabled = true;
		//	effect.EnableDefaultLighting();

		//	Vector3 lightDirection = new Vector3( 1, 1, 0 );
		//	Vector3 lightColor = new Vector3( 0.3f, 0.4f, 0.2f );

		//	effect.DirectionalLight0.Enabled = true;
		//	effect.DirectionalLight0.Direction = lightDirection;
		//	effect.DirectionalLight0.DiffuseColor = lightColor;
		//	effect.DirectionalLight0.SpecularColor = lightColor;
		//	effect.CurrentTechnique.Passes[ 0 ].Apply();

		//	graphicsDevice.SetRasterizerState( graphicsDevice.RasterizerStates.CullNone );

		//	batch.Begin();
		//	batch.Draw( PrimitiveType.TriangleList, voxeltriangles );
		//	batch.End();
		//}
	}
}