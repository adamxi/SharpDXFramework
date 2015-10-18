using System;
using SharpDX;

namespace DXFramework.Util
{
	public static class Interpolation
	{
		public static Vector2 Interpolate( InterpolationType type, Vector2 from, Vector2 to, float duration, float elapsed )
		{
			switch( type )
			{
				case InterpolationType.Linear:
					return Lerp( from, to, duration, elapsed );

				case InterpolationType.SinusCurve:
					return Sinus( from, to, duration, elapsed );

				case InterpolationType.SmoothStep:
					return SmoothStep( from, to, duration, elapsed );

				default:
					return Vector2.Zero;
			}
		}

		public static Vector2 Interpolate( InterpolationType type, Vector2 from, Vector2 to, float amount )
		{
			switch( type )
			{
				case InterpolationType.Linear:
					return Lerp( from, to, amount );

				case InterpolationType.SinusCurve:
					return Sinus( from, to, amount );

				case InterpolationType.SmoothStep:
					return SmoothStep( from, to, amount );

				default:
					return Vector2.Zero;
			}
		}

		public static Vector2 PixelInterpolate( InterpolationType type, Vector2 from, Vector2 to, float pixelPerSec, float elapsed )
		{
			switch( type )
			{
				case InterpolationType.Linear:
					return PixelLerp( from, to, pixelPerSec, elapsed );

				case InterpolationType.SinusCurve:
					return PixelSinus( from, to, pixelPerSec, elapsed );

				case InterpolationType.SmoothStep:
					return PixelSmoothStep( from, to, pixelPerSec, elapsed );

				default:
					return Vector2.Zero;
			}
		}

		public static Vector2 Lerp( Vector2 from, Vector2 to, float amount )
		{
			return from + ( to - from ) * amount;
		}

		public static Vector2 Lerp( Vector2 from, Vector2 to, float duration, float elapsed )
		{
			if( duration == 0 )
			{
				return from;
			}
			return from + ( to - from ) * elapsed / duration;
		}

		public static Vector2 PixelLerp( Vector2 from, Vector2 to, float pixelPerSec, float elapsed )
		{
			if( elapsed == 0 )
			{
				return from;
			}

			float length = ( to - from ).Length();
			float amount = ( pixelPerSec / length ) * elapsed;
			if( amount >= 1 )
			{
				return to;
			}

			return from + ( to - from ) * amount;
		}

		public static Vector2 Sinus( Vector2 from, Vector2 to, float amount )
		{
			float pos = MathUtil.PiOverTwo + ( MathUtil.Pi * amount );
			return from + ( to - from ) * ( 0.5f - (float)Math.Sin( pos ) * 0.5f );
		}

		public static Vector2 Sinus( Vector2 from, Vector2 to, float duration, float elapsed )
		{
			if( duration == 0 )
			{
				return from;
			}
			if( elapsed >= duration )
			{
				return to;
			}
			float pos = MathUtil.PiOverTwo + ( MathUtil.Pi * elapsed / duration );
			return from + ( to - from ) * ( 0.5f - (float)Math.Sin( pos ) * 0.5f );
		}

		public static Vector2 PixelSinus( Vector2 from, Vector2 to, float pixelPerSec, float elapsed )
		{
			if( elapsed == 0 )
			{
				return from;
			}

			float length = ( to - from ).Length();
			float amount = ( pixelPerSec / length ) * elapsed;
			if( amount >= 1 )
			{
				return to;
			}

			float pos = MathUtil.PiOverTwo + ( MathUtil.Pi * amount );
			return from + ( to - from ) * ( 0.5f - (float)Math.Sin( pos ) * 0.5f );
		}

		public static Vector2 SmoothStep( Vector2 from, Vector2 to, float amount )
		{
			float num = MathUtil.Clamp( amount, 0f, 1f );
			return Lerp( from, to, ( num * num ) * ( 3f - ( 2f * num ) ) );
		}

		public static Vector2 SmoothStep( Vector2 from, Vector2 to, float duration, float elapsed )
		{
			float num = MathUtil.Clamp( elapsed / duration, 0f, 1f );
			return Lerp( from, to, ( num * num ) * ( 3f - ( 2f * num ) ) );
		}

		public static Vector2 PixelSmoothStep( Vector2 from, Vector2 to, float pixelPerSec, float elapsed )
		{
			if( elapsed == 0 )
			{
				return from;
			}

			float length = ( to - from ).Length();
			float amount = ( pixelPerSec / length ) * elapsed;
			if( amount >= 1 )
			{
				return to;
			}

			float num = MathUtil.Clamp( amount, 0f, 1f );
			return Lerp( from, to, ( num * num ) * ( 3f - ( 2f * num ) ) );
		}

		public enum InterpolationType
		{
			/// <summary>
			/// Linear interpolation.
			/// </summary>
			Linear,

			/// <summary>
			/// Sinus interpolation. The sinus curve will make the interpolation step smaller at the beginning and the end of the range.
			/// </summary>
			SinusCurve,

			SmoothStep,
		}
	}
}