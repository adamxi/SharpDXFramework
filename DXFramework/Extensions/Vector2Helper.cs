using System;

namespace SharpDX
{
	/// <summary>
	/// Legacy helper class implementing old XNA Vector2 funtionality
	/// </summary>
	[Obsolete( "Use SharpDX framework instead" )]
	public static class Vector2Helper
	{
		public static void Transform( ref Vector2 position, ref Matrix matrix, out Vector2 result )
		{
			float x = ( ( position.X * matrix.M11 ) + ( position.Y * matrix.M21 ) ) + matrix.M41;
			float y = ( ( position.X * matrix.M12 ) + ( position.Y * matrix.M22 ) ) + matrix.M42;
			result.X = x;
			result.Y = y;
		}

		public static Vector2 Transform( Vector2 position, Matrix matrix )
		{
			Vector2 vector;
			float x = ( ( position.X * matrix.M11 ) + ( position.Y * matrix.M21 ) ) + matrix.M41;
			float y = ( ( position.X * matrix.M12 ) + ( position.Y * matrix.M22 ) ) + matrix.M42;
			vector.X = x;
			vector.Y = y;
			return vector;
		}
	}
}