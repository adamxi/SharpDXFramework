using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.Util
{
	/// <summary>
	/// Manages screen view.
	/// Based on: http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/.
	/// </summary>
	public class Camera
	{
		private GraphicsDevice graphics;
		private Matrix transform;
		private Matrix orthographicTransform;
		private Vector2 cameraPos;
		private Vector2 localCursorPos;
		private Vector2 oldLocalCursorPos;
		private float zoom;
		private float invZoom;
		private float rotation;
		private float radianRotation;
		private bool updateTransform;
		private bool updateOrthographicTransform;

		public Camera( GraphicsDevice graphics )
		{
			this.graphics = graphics;
			UpdateTransformations();
			Zoom = 1.0f;
		}

		#region Properties
		/// <summary>
		/// Camera zoom factor. Set to 1.0f for no zoom.
		/// </summary>
		public float Zoom
		{
			get { return zoom; }
			set
			{
				if( zoom != value )
				{
					zoom = value;
					if( zoom < 0.002f ) // Negative zoom will flip image
					{
						zoom = 0.002f;
					}
					invZoom = 1f / zoom;
					UpdateTransformations();
				}
			}
		}

		/// <summary>
		/// The inverse of the camera zoom.
		/// </summary>
		public float InverseZoom
		{
			get { return invZoom; }
		}

		/// <summary>
		/// Camera rotation in degrees.
		/// </summary>
		public float Rotation
		{
			get { return rotation; }
			set
			{
				if( rotation != value )
				{
					rotation = value % 360;
					while( rotation < 0 )
					{
						rotation += 360;
					}
					radianRotation = MathUtil.DegreesToRadians( rotation );
					UpdateTransformations();
				}
			}
		}

		/// <summary>
		/// Camera rotation in radians.
		/// </summary>
		public float RadianRotation
		{
			get { return radianRotation; }
		}

		/// <summary>
		/// Camera position. This position is always in the center of the viewport.
		/// </summary>
		public Vector2 Position
		{
			get { return cameraPos; }
			set
			{
				cameraPos = value;
				UpdateTransformations();
			}
		}

		/// <summary>
		/// Camera-space cursor position.
		/// </summary>
		public Vector2 CursorPosition
		{
			get { return cameraPos + ( ( InputManager.MousePosition - HalfViewportSize ) * invZoom ); }
		}

		/// <summary>
		/// How far the cursor has moved in camera-space, since last call to 'MoveCursor'.
		/// </summary>
		public Vector2 CursorMoveDist
		{
			get { return ( oldLocalCursorPos - localCursorPos ) * invZoom; }
		}

		/// <summary>
		/// Returns the center position of the camera's viewport in camera-space.
		/// </summary>
		public Vector2 ViewportCenter
		{
			get { return -( cameraPos * zoom ) + HalfViewportSize; }
		}

		private ViewportF Viewport
		{
			get { return graphics.Viewport; }
		}

		private Vector2 HalfViewportSize
		{
			get { return new Vector2( graphics.Viewport.Width * 0.5f, graphics.Viewport.Height * 0.5f ); }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initializes camera movement.
		/// </summary>
		public void InitializeMovement()
		{
			localCursorPos = InputManager.MousePosition;
		}

		/// <summary>
		/// Handles camera movement.
		/// </summary>
		public void DoMovement()
		{
			oldLocalCursorPos = localCursorPos;
			localCursorPos = InputManager.MousePosition;

			Vector2 dist = CursorMoveDist;
			if( radianRotation != 0 )
			{
				Position += dist.RotateAroundOrigin( Vector2.Zero, radianRotation );
			}
			else
			{
				Position += dist;
			}
		}

		/// <summary>
		/// Returns camera-space position, from a local-space positon.
		/// </summary>
		/// <param name="localSpacePosition">Local-space position to convert.</param>
		public Vector2 GetCameraPos( Vector2 localSpacePosition )
		{
			return cameraPos + ( ( localSpacePosition - HalfViewportSize ) * invZoom );
		}

		/// <summary>
		/// Returns camera-space position, from a local-space positon.
		/// </summary>
		/// <param name="localSpacePosition">Local-space position to convert.</param>
		public void GetCameraPos( ref Vector2 localSpacePosition, out Vector2 cameraSpacePosition )
		{
			cameraSpacePosition = cameraPos + ( ( localSpacePosition - HalfViewportSize ) * invZoom );
		}

		/// <summary>
		/// Moves the camera by a set amount.
		/// </summary>
		/// <param name="amount">Amount to move the camera in local-space.</param>
		public void Move( Vector2 amount )
		{
			Position -= amount * invZoom;
		}

		/// <summary>
		/// Centers the Camera to the level center.
		/// </summary>
		public void CenterToLevel()
		{
			Position = Vector2.Zero;
		}

		/// <summary>
		/// Centers the Camera to the cursor.
		/// </summary>
		public void CenterToCursor()
		{
			Position = CursorPosition;
		}

		/// <summary>
		/// Centers the Camera to a position.
		/// </summary>
		/// <param name="pos">Camera-space position.</param>
		public void CenterToPosition( Vector2 pos )
		{
			Position = pos;
		}

		/// <summary>
		/// Zooms the camera by a set amount to a given position.
		/// </summary>
		/// <param name="zoomAmount">Zoom increment.</param>
		/// <param name="pos">Position to zoom too.</param>
		public void ZoomToPos( int zoomPercentAmount, Vector2 pos )
		{
			Vector2 oldPos;
			GetCameraPos( ref pos, out oldPos );

			Zoom += ( zoom * 0.01f ) * zoomPercentAmount;

			Vector2 newPos;
			GetCameraPos( ref pos, out newPos );

			Position -= newPos - oldPos;
		}

		/// <summary>
		/// Forces the camera to update its transformation matrices.
		/// </summary>
		public void UpdateTransformations()
		{
			updateTransform = true;
			updateOrthographicTransform = true;
		}

		/// <summary>
		/// Returns the Camera's transformation matrix calculated by position, rotation, zoom and viewport where {x:0, y:0} is the center of the viewport.
		/// Used when drawing in 2D spaces.
		/// If no changes has happened to the camera, the matrix will not be recalculated, and the previous matrix will be returned.
		/// </summary>
		public Matrix GetTransformation()
		{
			if( updateTransform )
			{
				updateTransform = false;
				transform =
					Matrix.Translation( -cameraPos.X, -cameraPos.Y, 0 ) *
					Matrix.RotationZ( radianRotation ) *
					Matrix.Scaling( zoom, zoom, 0 ) *
					Matrix.Translation( graphics.Viewport.Width * 0.5f, graphics.Viewport.Height * 0.5f, 0 );
			}
			return transform;
		}

		/// <summary>
		/// Returns the Camera's Orthographic transformation matrix. Used when drawing in 3D spaces and when drawing primitives.
		/// If no changes has happened to the camera, the matrix will not be recalculated, and the previous matrix will be returned.
		/// </summary>
		public Matrix GetOrthographicTransformation()
		{
			if( updateOrthographicTransform )
			{
				updateOrthographicTransform = false;
				orthographicTransform =
					GetTransformation() *
					Matrix.OrthoOffCenterLH( 0f, graphics.Viewport.Width, graphics.Viewport.Height, 0f, 0f, 1f );
			}
			return orthographicTransform;
		}
		#endregion
	}
}