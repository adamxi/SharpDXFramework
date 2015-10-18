namespace DXFramework.Tweening
{
	/// <summary>
	/// Cubic ease functions (x^3).
	/// </summary>
	public static class Cubic
	{
		public class EaseIn : IEaseFunction
		{
			public float Update( float amount )
			{
				return amount * amount * amount;
			}
		}

		public class EaseOut : IEaseFunction
		{
			public float Update( float amount )
			{
				amount -= 1;
				return amount * amount * amount + 1;
			}
		}

		public class EaseInOut : IEaseFunction
		{
			public float Update( float amount )
			{
				amount *= 2;
				if( amount < 1 )
				{
					return amount * amount * amount * 0.5f;
				}
				amount -= 2;
				return 0.5f * ( amount * amount * amount + 2 );
			}
		}
	}
}