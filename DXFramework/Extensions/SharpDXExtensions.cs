using System;

namespace SharpDX
{
	public static class SharpDXExtensions
	{
		#region Rectangle
		public static Rectangle ToRectangle(this RectangleF r) => new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);

		public static RectangleF ToRectangleF(this Rectangle r) => new RectangleF(r.X, r.Y, r.Width, r.Height);

		public static bool Intersects(this Rectangle r, Vector2 position)
		{
			if (position.X < r.Left || position.X > r.Right ||
				position.Y < r.Top || position.Y > r.Bottom)
			{
				return false;
			}
			return true;
		}

		public static bool Intersects(this Rectangle r, Rectangle rectangle)
		{
			if (rectangle.Right < r.Left || rectangle.Left > r.Right ||
				rectangle.Bottom < r.Top || rectangle.Top > r.Bottom)
			{
				return false;
			}
			return true;
		}

		public static bool Intersects(this RectangleF r, Vector2 position)
		{
			if (position.X < r.Left || position.X > r.Right ||
				position.Y < r.Top || position.Y > r.Bottom)
			{
				return false;
			}
			return true;
		}

		public static bool Intersects(this RectangleF r, RectangleF rectangle)
		{
			if (rectangle.Right < r.Left || rectangle.Left > r.Right ||
				rectangle.Bottom < r.Top || rectangle.Top > r.Bottom)
			{
				return false;
			}
			return true;
		}
		#endregion

		#region Point
		public static Vector2 ToVector(this Point p) => new Vector2(p.X, p.Y);

		public static Size2 ToSize2(this Point p) => new Size2(p.X, p.Y);

		public static Size2F ToSize2F(this Point p) => new Size2F(p.X, p.Y);
		#endregion

		#region Vector2
		public static Point ToPoint(this Vector2 v) => new Point((int)v.X, (int)v.Y);

		public static Size2F ToSize2F(this Vector2 v) => new Size2F(v.X, v.Y);

		public static Size2 ToSize2(this Vector2 v) => new Size2((int)v.X, (int)v.Y);

		public static void Abs(this Vector2 v)
		{
			v.X = Math.Abs(v.X);
			v.Y = Math.Abs(v.Y);
		}

		public static Vector2 GetAbs(this Vector2 v)
		{
			Vector2 vec;
			vec.X = Math.Abs(v.X);
			vec.Y = Math.Abs(v.Y);
			return vec;
		}

		/// <summary>
		/// Rounds the vector to a given decimal.
		/// </summary>
		public static void Round(this Vector2 v, int decimals)
		{
			v.X = (float)Math.Round(v.X, decimals, MidpointRounding.AwayFromZero);
			v.Y = (float)Math.Round(v.Y, decimals, MidpointRounding.AwayFromZero);
		}

		/// <summary>
		/// Returns a rounded vector to a given decimal.
		/// </summary>
		public static Vector2 RoundToVector(this Vector2 v, int decimals)
		{
			Vector2 vec;
			vec.X = (float)Math.Round(v.X, decimals, MidpointRounding.AwayFromZero);
			vec.Y = (float)Math.Round(v.Y, decimals, MidpointRounding.AwayFromZero);
			return vec;
		}

		/// <summary>
		/// Returns this vector projected onto another vector.
		/// </summary>
		public static Vector2 Project(this Vector2 v, Vector2 vector)
		{
			Vector2 proj;
			float dot = Vector2.Dot(v, vector);
			float invLenB = 1f / vector.LengthSquared();
			proj.X = (dot * invLenB) * vector.X;
			proj.Y = (dot * invLenB) * vector.Y;
			return proj;
		}

		/// <summary>
		/// Returns the unit-normal of this vector (Right-handed).
		/// </summary>
		public static Vector2 Normal(this Vector2 vector)
		{
			Vector2 v = new Vector2();
			v.X = -vector.Y;
			v.Y = vector.X;
			return v;
		}

		public static void RotateAroundOrigin(this Vector2 point, ref Vector2 origin, ref float radians, out Vector2 vector)
		{
			vector = Vector2.Transform(point - origin, Quaternion.RotationYawPitchRoll(radians, 0, 0)) + origin;
		}

		public static Vector2 RotateAroundOrigin(this Vector2 point, Vector2 origin, float radians)
		{
			return Vector2.Transform(point - origin, Quaternion.RotationYawPitchRoll(radians, 0, 0)) + origin;
		}
		#endregion

		#region Vector3
		/// <summary>
		/// Rounds the vector to a given decimal.
		/// </summary>
		public static Vector3 RoundToVector(this Vector3 v, int decimals)
		{
			Vector3 vec;
			vec.X = (float)Math.Round(v.X, decimals, MidpointRounding.AwayFromZero);
			vec.Y = (float)Math.Round(v.Y, decimals, MidpointRounding.AwayFromZero);
			vec.Z = (float)Math.Round(v.Z, decimals, MidpointRounding.AwayFromZero);
			return vec;
		}

		public static Vector2 ToVector2(this Vector3 v)
		{
			Vector2 vec;
			vec.X = v.X;
			vec.Y = v.Y;
			return vec;
		}
		#endregion

		#region Vector4
		/// <summary>
		/// Rounds the vector to a given decimal.
		/// </summary>
		public static Vector4 RoundToVector(this Vector4 v, int decimals)
		{
			Vector4 vec;
			vec.X = (float)Math.Round(v.X, decimals, MidpointRounding.AwayFromZero);
			vec.Y = (float)Math.Round(v.Y, decimals, MidpointRounding.AwayFromZero);
			vec.Z = (float)Math.Round(v.Z, decimals, MidpointRounding.AwayFromZero);
			vec.W = (float)Math.Round(v.W, decimals, MidpointRounding.AwayFromZero);
			return vec;
		}

		public static Vector3 ToVector3(this Vector4 v)
		{
			Vector3 vec;
			vec.X = v.X;
			vec.Y = v.Y;
			vec.Z = v.Z;
			return vec;
		}
		#endregion

		#region Size
		public static Vector2 ToVector(this Size2 s) => new Vector2(s.Width, s.Height);

		public static Vector2 ToVector(this Size2F s) => new Vector2(s.Width, s.Height);

		public static Point ToPoint(this Size2 s) => new Point(s.Width, s.Height);

		public static Point ToPoint(this Size2F s) => new Point((int)s.Width, (int)s.Height);
		#endregion

		#region Texture2D
		//private static Dictionary<object, Texture2D> textureThumbnails = new Dictionary<object, Texture2D>();

		//public static Texture2D Thumbnail( this Texture2D texture, GraphicsDevice device, SpriteBatch spriteBatch, int height )
		//{
		//	Texture2D tex;
		//	object thumbnailId = texture.Tag;

		//	if( textureThumbnails.TryGetValue( thumbnailId, out tex ) )
		//	{
		//		return tex;
		//	}

		//	int srcWidth = texture.Width;
		//	int srcHeight = texture.Height;
		//	int destWidth = (int)( ( srcWidth / (float)srcHeight ) * height );
		//	int destHeight = height;

		//	RenderTarget2D renderTarget = new RenderTarget2D( device, destWidth, destHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents );
		//	renderTarget.Tag = thumbnailId;

		//	device.SetRenderTarget( renderTarget );
		//	device.Clear( Color.White );

		//	spriteBatch.Begin();
		//	spriteBatch.Draw( texture, new Rectangle( 0, 0, destWidth, destHeight ), Color.White );
		//	spriteBatch.End();
		//	device.SetRenderTarget( null );

		//	textureThumbnails.Add( thumbnailId, renderTarget );

		//	return renderTarget;
		//}
		#endregion
	}
}