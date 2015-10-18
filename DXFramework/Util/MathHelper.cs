using System;
using SharpDX;

namespace DXFramework.Util
{
	public static class MathHelper
	{
		private const float INV_TWOPI = 1f / MathUtil.TwoPi;

		/// <summary>
		/// Returns the revolutons [0..1] between a current- and target position, form a facing angle.
		/// </summary>
		/// <param name="position">Current position.</param>
		/// <param name="target">Target position.</param>
		/// <param name="facingRadians">Current position facing radians.</param>
		public static float RevolutionsToTarget( Vector2 position, Vector2 target, Vector2 facingRadians )
		{
			return RadiansToTarget( position, target, facingRadians ) * INV_TWOPI;
		}

		/// <summary>
		/// Returns the radians between a current- and target position, form a facing angle.
		/// </summary>
		/// <param name="position">Current position.</param>
		/// <param name="target">Target position.</param>
		/// <param name="facingRadians">Current position facing radians.</param>
		public static float RadiansToTarget( Vector2 position, Vector2 target, Vector2 facingRadians )
		{
			Vector2 rightAngle = Vector2.Zero;
			rightAngle.X = -facingRadians.Y;
			rightAngle.Y = facingRadians.X;
			Vector2 diff = target - position;

			float dot1;
			float dot2;
			Vector2.Dot( ref facingRadians, ref diff, out dot1 );
			Vector2.Dot( ref rightAngle, ref diff, out dot2 );

			return (float)Math.Atan2( dot2, dot1 );
		}

		/// <summary>
		/// Warps an angle between 0 and TwoPI (6.28319).
		/// NOTE: Single-pass. Values below -(2 * TwoPI) or above (2 * TwoPI) will not be correctly warped.
		/// </summary>
		/// <param name="radians">Radians to warp.</param>
		public static float WarpAngle( float radians )
		{
			if( radians > MathUtil.TwoPi )
			{
				radians -= MathUtil.TwoPi;
			}
			else if( radians < 0 )
			{
				radians += MathUtil.TwoPi;
			}
			return radians;
		}
	}
}