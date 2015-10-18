namespace DXFramework.Tweening
{
	/// <summary>
	/// Bouncing ease functions.
	/// </summary>
	public static class Bounce
	{
		public class EaseIn : IEaseFunction
		{
			private EaseOut easeOut;

			public EaseIn()
			{
				easeOut = new EaseOut();
			}

			public float Update( float amount )
			{
				return 1 - easeOut.Update( 1 - amount );
			}
		}

		public class EaseOut : IEaseFunction
		{
			private const float INV_2_75 = 1f / 2.75f;

			public float Update( float amount )
			{
				if( amount < INV_2_75 )
				{
					return 7.5625f * amount * amount;
				}
				if( amount < 2f * INV_2_75 )
				{
					amount -= 1.5f * INV_2_75;
					return 7.5625f * amount * amount + 0.75f;
				}
				if( amount < 2.5f * INV_2_75 )
				{
					amount -= 2.25f * INV_2_75;
					return 7.5625f * amount * amount + 0.9375f;
				}
				amount -= 2.625f * INV_2_75;
				return 7.5625f * amount * amount + 0.984375f;
			}
		}

		public class EaseInOut : IEaseFunction
		{
			private EaseIn easeIn;
			private EaseOut easeOut;

			public EaseInOut()
			{
				easeIn = new EaseIn();
				easeOut = new EaseOut();
			}

			public float Update( float amount )
			{
				if( amount < 0.5f )
				{
					return easeIn.Update( amount * 2 ) * 0.5f;
				}
				return easeOut.Update( amount * 2 - 1 ) * 0.5f + 0.5f;
			}
		}
	}
}