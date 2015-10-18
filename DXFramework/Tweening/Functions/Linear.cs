namespace DXFramework.Tweening
{
	/// <summary>
	/// Linear ease functions.
	/// </summary>
	public static class Linear
	{
		public class EaseNone : IEaseFunction
		{
			public float Update( float amount )
			{
				return amount;
			}
		}
	}
}