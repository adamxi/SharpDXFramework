using System;
using SharpDX;

namespace DXFramework.Tweening
{
	/// <summary>
	/// Elastic ease functions.
	/// </summary>
	public static class Elastic
	{
		public class EaseIn : IEaseFunction
		{
			private float amplitude;
			private float s;
			private float invPeriod;

			public EaseIn( float period = 0.4f, float amplitude = 0f )
			{
				if( Math.Abs( amplitude ) < 1f )
				{
					amplitude = 1f;
					s = period * 0.25f;
				}
				else
				{
					s = period / MathUtil.TwoPi * (float)Math.Asin( 1f / amplitude );
				}

				this.amplitude = amplitude;
				this.invPeriod = 1f / period;
			}

			public float Update( float amount )
			{
				amount -= 1;
				return -(float)( amplitude * Math.Pow( 2, 10 * amount ) * Math.Sin( ( amount - s ) * MathUtil.TwoPi * invPeriod ) );
			}
		}

		public class EaseOut : IEaseFunction
		{
			private float amplitude;
			private float s;
			private float invPeriod;

			public EaseOut( float period = 0.4f, float amplitude = 0f )
			{
				if( Math.Abs( amplitude ) < 1f )
				{
					amplitude = 1f;
					s = period * 0.25f;
				}
				else
				{
					s = period / MathUtil.TwoPi * (float)Math.Asin( 1f / amplitude );
				}

				this.amplitude = amplitude;
				this.invPeriod = 1f / period;
			}

			public float Update( float amount )
			{
				return (float)( amplitude * Math.Pow( 2, -10 * amount ) * Math.Sin( ( amount - s ) * MathUtil.TwoPi * invPeriod ) + 1 );
			}
		}

		public class EaseInOut : IEaseFunction
		{
			private float amplitude;
			private float s;
			private float invPeriod;

			public EaseInOut( float period = 0.5f, float amplitude = 0f )
			{
				if( Math.Abs( amplitude ) < 1f )
				{
					amplitude = 1f;
					s = period * 0.25f;
				}
				else
				{
					s = period / MathUtil.TwoPi * (float)Math.Asin( 1f / amplitude );
				}

				this.amplitude = amplitude;
				this.invPeriod = 1f / period;
			}

			public float Update( float amount )
			{
				amount *= 2;
				if( amount < 1 )
				{
					amount -= 1;
					return (float)( -0.5f * amplitude * Math.Pow( 2, 10 * amount ) * Math.Sin( ( amount - s ) * MathUtil.TwoPi * invPeriod ) );
				}
				amount -= 1;
				return (float)( amplitude * Math.Pow( 2, -10 * amount ) * Math.Sin( ( amount - s ) * MathUtil.TwoPi * invPeriod ) * 0.5f + 1 );
			}
		}
	}
}