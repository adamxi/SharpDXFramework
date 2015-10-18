using System;

namespace DXFramework.Tweening
{
	/// <summary>
	/// Exponential ease functions.
	/// </summary>
	public static class Exponential
	{
		public class EaseIn : IEaseFunction
		{
			public float Update( float amount )
			{
				return (float)Math.Pow( 2, 10 * ( amount - 1 ) ) - 0.001f;
			}
		}

		public class EaseOut : IEaseFunction
		{
			public float Update( float amount )
			{
				return -(float)Math.Pow( 2, -10 * amount ) + 1;
			}
		}

		public class EaseInOut : IEaseFunction
		{
			public float Update( float amount )
			{
				amount *= 2;
				if( amount < 1 )
				{
					return 0.5f * (float)Math.Pow( 2, 10 * ( amount - 1 ) );
				}
				return 0.5f * ( -(float)Math.Pow( 2, -10 * ( amount - 1 ) ) + 2 );
			}
		}
	}
}