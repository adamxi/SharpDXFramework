using DXFramework;
using DXFramework.SceneManagement;
using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using VoxelEngine;
using VoxelEngine.Primitives;

namespace XManager.Scenes
{
	public class GameWorld : Scene
	{
		private Vector3 cameraPosition;
		private Matrix view;
		private Matrix projection;
		private Vector2 rot;
		private VoxelChunk chunk;
		private VoxelEngine.Primitives.GeometricPrimitive marker;

		public override void LoadContent()
		{
			InputManager.LockMouseToCenter = true;
			game.IsMouseVisible = false;

			VoxelRenderer.Init( Game );
			Voxelizer.Init();

			cameraPosition = new Vector3( 25, 50, 150 );
			chunk = Voxelizer.CreateChunk( 0.5f );
			marker = new SpherePrimitive( 8, 16 );
		}

		public override void UnloadContent()
		{
			InputManager.LockMouseToCenter = false;
		}

		private void UpdateViewMatrix( GameTime gameTime )
		{
			float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			float rotationSpeed = 0.003f;
			float moveSpeed = 0.2f;

			rot += InputManager.MouseDelta * rotationSpeed;
			rot.Y = MathUtil.Clamp( rot.Y, -MathUtil.PiOverTwo, MathUtil.PiOverTwo );

			Matrix cameraRotation = Matrix.RotationX( -rot.Y ) * Matrix.RotationY( -rot.X );
			Vector3 cameraRotatedTarget = Vector3.TransformCoordinate( Vector3.ForwardRH, cameraRotation );
			Vector3 cameraRotatedUpVector = Vector3.TransformCoordinate( Vector3.Up, cameraRotation );

			Vector3 moveVector = Vector3.Zero;
			if( InputManager.Held( Keys.W ) )
			{
				moveVector += Vector3.ForwardRH;
			}
			else if( InputManager.Held( Keys.S ) )
			{
				moveVector += Vector3.BackwardRH;
			}
			if( InputManager.Held( Keys.A ) )
			{
				moveVector += Vector3.Left;
			}
			else if( InputManager.Held( Keys.D ) )
			{
				moveVector += Vector3.Right;
			}
			if( InputManager.Held( Keys.Q ) )
			{
				moveVector += Vector3.Up;
			}
			else if( InputManager.Held( Keys.E ) )
			{
				moveVector += Vector3.Down;
			}

			Vector3 rotatedMoveVector = Vector3.TransformCoordinate( moveVector, cameraRotation );
			cameraPosition += rotatedMoveVector * time * moveSpeed;
			Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;
			view = Matrix.LookAtRH( cameraPosition, cameraFinalTarget, cameraRotatedUpVector );

			marker.Position = cameraPosition + cameraRotatedTarget * 50;
		}

		public override void Update( GameTime gameTime )
		{
			UpdateViewMatrix( gameTime );

			if( InputManager.Pressed( Keys.Escape ) )
			{
				game.Exit();
			}
			if( InputManager.Pressed( Keys.T ) )
			{
				VoxelRenderer.Wireframe = !VoxelRenderer.Wireframe;
			}
			if( InputManager.Held( MouseButton.Left ) )
			{

			}
			if( InputManager.Held( MouseButton.Right ) )
			{

			}
		}

		public override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.CornflowerBlue );

			projection = Matrix.PerspectiveFovRH( MathUtil.PiOverFour, GraphicsDevice.Viewport.AspectRatio, .1f, 10000f );

			VoxelRenderer.Begin( view, projection );
			VoxelRenderer.Draw( chunk );
			VoxelRenderer.End();

			VoxelRenderer.Begin( view, projection );
			VoxelRenderer.Draw( marker );
			VoxelRenderer.End();

			spriteBatch.Begin();
			spriteBatch.DrawString( Engine.DefaultFont, "Vertices: " + chunk.VertexCount.ToString(), new Vector2( 10, 10 ), Color.White );
			spriteBatch.End();
		}
	}
}