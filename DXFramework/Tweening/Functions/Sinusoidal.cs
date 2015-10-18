using System;
using SharpDX;

namespace DXFramework.Tweening
{
	/// <summary>
	/// Sinusoidal ease functions.
	/// </summary>
	public static class Sinusoidal
	{
		public class EaseIn : IEaseFunction
		{
			public float Update( float amount )
			{
				return (float)Math.Sin( amount * MathUtil.PiOverTwo - MathUtil.PiOverTwo ) + 1;
			}
		}

		public class EaseOut : IEaseFunction
		{
			public float Update( float amount )
			{
				return (float)Math.Sin( amount * MathUtil.PiOverTwo );
			}
		}

		public class EaseInOut : IEaseFunction
		{
			public float Update( float amount )
			{
				return (float)( Math.Sin( amount * MathUtil.Pi - MathUtil.PiOverTwo ) + 1 ) * 0.5f;
			}
		}
	}
}