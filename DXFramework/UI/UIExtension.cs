using DXFramework.Util;

namespace DXFramework.UI
{
	public static class UIExtension
	{
		public static T Copy<T>( this T c ) where T : UIControl
		{
            T control = c.DeepClone();
			control.CheckInitialize();
			return control;
		}
	}
}