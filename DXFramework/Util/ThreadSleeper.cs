using System.Runtime.InteropServices;

namespace DXFramework.Util
{
	public class ThreadSleeper
	{
		[DllImport( "winmm.dll" )]
		private static extern uint timeBeginPeriod( uint period );

		[DllImport( "winmm.dll" )]
		private static extern uint timeEndPeriod( uint period );

		private static uint lastRes;

		static ThreadSleeper()
		{
			lastRes = 0;
		}

		~ThreadSleeper()
		{
			Reset();
		}

		public static void SetThreadResolution( uint milliseconds )
		{
			lastRes = milliseconds;
			timeBeginPeriod( milliseconds );
		}

		public static void DefaultThreadResolution()
		{
			Reset();
		}

		private static void Reset()
		{
			if( lastRes != 0 )
			{
				timeEndPeriod( lastRes );
				lastRes = 0;
			}
		}
	}
}