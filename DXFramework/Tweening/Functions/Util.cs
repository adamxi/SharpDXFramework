using System;

namespace DXFramework.Tweening
{
	public static class Util
	{
		private static float EaseInPower( float amount, int power )
		{
			return (float)Math.Pow( amount, power );
		}

		private static float EaseOutPower( float amount, int power )
		{
			int sign = ( power & ( power - 1 ) ) == 0 ? -1 : 1;
			return (float)( sign * ( Math.Pow( amount - 1, power ) + sign ) );
		}

		private static float EaseInOutPower( float amount, int power )
		{
			amount *= 2;
			if( amount < 1 )
			{
				return (float)Math.Pow( amount, power ) * 0.5f;
			}
			int sign = ( power & ( power - 1 ) ) == 0 ? -1 : 1;
			return (float)( sign * 0.5f * ( Math.Pow( amount - 2, power ) + sign * 2 ) );
		}
	}
}