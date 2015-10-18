namespace DXFramework.Tweening
{
	public static class Back
	{
		public class EaseIn : IEaseFunction
		{
			private float overshoot;

			public EaseIn( float overshoot = 0.85079f )
			{
				this.overshoot = overshoot;
			}

			public float Update( float amount )
			{
				return amount * amount * ( ( overshoot + 1 ) * amount - overshoot );
			}
		}

		public class EaseOut : IEaseFunction
		{
			private float overshoot;

			public EaseOut( float overshoot = 0.85079f )
			{
				this.overshoot = overshoot;
			}

			public float Update( float amount )
			{
				amount -= 1;
				return amount * amount * ( ( overshoot + 1 ) * amount + overshoot ) + 1;
			}
		}

		public class EaseInOut : IEaseFunction
		{
			private float overshoot;

			public EaseInOut( float overshoot = 0.85079f )
			{
				this.overshoot = overshoot;
			}

			public float Update( float amount )
			{
				amount *= 2;
				if( amount < 1 )
				{
					return 0.5f * ( amount * amount * ( ( overshoot + 1 ) * amount - overshoot ) );
				}
				amount -= 2;
				return 0.5f * ( amount * amount * ( ( overshoot + 1 ) * amount + overshoot ) + 2 );
			}
		}
	}
}