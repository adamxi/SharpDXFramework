using System;

namespace DXFramework.Tweening
{
	public static class Circular
	{
		public class EaseIn : IEaseFunction
		{
			public float Update( float amount )
			{
				return -( (float)Math.Sqrt( 1 - amount * amount ) - 1 );
			}
		}

		public class EaseOut : IEaseFunction
		{
			public float Update( float amount )
			{
				amount -= 1;
				return (float)Math.Sqrt( 1 - amount * amount );
			}
		}

		public class EaseInOut : IEaseFunction
		{
			public float Update( float amount )
			{
				amount *= 2;
				if( amount < 1 )
				{
					return -0.5f * ( (float)Math.Sqrt( 1 - amount * amount ) - 1 );
				}
				amount -= 2;
				return 0.5f * ( (float)Math.Sqrt( 1 - amount * amount ) + 1 );
			}
		}
	}
}