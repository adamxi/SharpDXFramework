using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DXFramework.Util
{
	/// <summary>
	/// High performance tick counter.
	/// </summary>
	public class TickCounter
	{
		public const long TICKS_PER_MILLISECOND = 10000;
		public const long TICKS_PER_SECOND = 10000000;
		public const long TICKS_PER_MINUTE = 600000000;
		public const long TICKS_PER_HOUR = 36000000000;
		public const long TICKS_PER_DAY = 864000000000;
		public const double MILLISECONDS_PER_TICK = 0.0001;
		public const long MILLISECONDS_PER_SECOND = 1000;
		public const long MILLISECONDS_PER_MINUTE = 60000;
		public const long MILLISECONDS_PER_HOUR = 3600000;
		public const long MILLISECONDS_PER_DAY = 86400000;
		public const double SECONDS_PER_TICK = 0.0000001;
		public const double SECONDS_PER_MILLISECOND = 0.001;
		public const long SECONDS_PER_MINUTE = 60;
		public const long SECONDS_PER_HOUR = 3600;
		public const long SECONDS_PER_DAY = 86400;
		public const double MINUTES_PER_TICK = 1.6666666666666666666666666666667e-9;
		public const double MINUTES_PER_MILLISECOND = 1.6666666666666666666666666666667e-5;
		public const double MINUTES_PER_SECOND = 0.01666666666666666666666666666667;
		public const long MINUTES_PER_HOUR = 60;
		public const long MINUTES_PER_DAY = 1440;
		public const double HOURS_PER_TICK = 2.7777777777777777777777777777778e-11;
		public const double DAYS_PER_TICK = 1.1574074074074074074074074074074e-12;

		[DllImport("KERNEL32")]
		private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long lpFrequency);

		private static long frequency;
		private static double invFrequencyTick;
		private static double invFrequencyMs;
		private static double invFrequencySec;
		private static double invFrequencyMin;
		private static double lastTime;

		static TickCounter()
		{
			if (QueryPerformanceFrequency(out frequency) == false)
			{
				throw new Win32Exception(); // Frequency not supported
			}
			else
			{
				invFrequencyTick = TICKS_PER_SECOND / (double)frequency;
				invFrequencyMs = MILLISECONDS_PER_SECOND / (double)frequency;
				invFrequencySec = 1d / frequency;
				invFrequencyMin = MINUTES_PER_SECOND / frequency;
			}
		}

		#region Timestamps
		private static double GetTick()
		{
			long tick;
			QueryPerformanceCounter(out tick);
			return tick;
		}

		/// <summary>
		/// Last timestamp retrieved.
		/// </summary>
		public static double LastTime
		{
			get { return lastTime; }
		}

		/// <summary>
		/// Ticks (sec / 10000000).
		/// </summary>
		public static double Tick
		{
			get
			{
				lastTime = GetTick() * invFrequencyTick;
				return lastTime;
			}
		}

		/// <summary>
		/// Milliseconds (sec / 1000).
		/// </summary>
		public static double Milliseconds
		{
			get
			{
				lastTime = GetTick() * invFrequencyMs;
				return lastTime;
			}
		}

		/// <summary>
		/// Seconds.
		/// </summary>
		public static double Seconds
		{
			get
			{
				lastTime = GetTick() * invFrequencySec;
				return lastTime;
			}
		}

		/// <summary>
		/// Minutes (sec / 60).
		/// </summary>
		public static double Minutes
		{
			get
			{
				lastTime = GetTick() * invFrequencyMin;
				return lastTime;
			}
		}
		#endregion

		#region Time conversions
		#region Tick
		/// <summary>
		/// Converts ticks to milliseconds.
		/// </summary>
		public static double TickToMs(long tick)
		{
			return tick * MILLISECONDS_PER_TICK;
		}

		/// <summary>
		/// Converts ticks to seconds.
		/// </summary>
		public static double TickToSec(long tick)
		{
			return tick * SECONDS_PER_TICK;
		}

		/// <summary>
		/// Converts ticks to minutes.
		/// </summary>
		public static double TickToMin(long tick)
		{
			return tick * MINUTES_PER_TICK;
		}
		#endregion

		#region Millisecond
		/// <summary>
		/// Converts milliseconds to ticks.
		/// </summary>
		public static double MsToTick(double ms)
		{
			return ms * TICKS_PER_MILLISECOND;
		}

		/// <summary>
		/// Converts milliseconds to seconds.
		/// </summary>
		public static double MsToSec(double ms)
		{
			return ms * SECONDS_PER_MILLISECOND;
		}

		/// <summary>
		/// Converts milliseconds to minutes.
		/// </summary>
		public static double MsToMin(double ms)
		{
			return ms * MINUTES_PER_MILLISECOND;
		}
		#endregion

		#region Second
		/// <summary>
		/// Converts seconds to ticks.
		/// </summary>
		public static double SecToTick(double sec)
		{
			return sec * TICKS_PER_SECOND;
		}

		/// <summary>
		/// Converts seconds to milliseconds.
		/// </summary>
		public static double SecToMs(double sec)
		{
			return sec * MILLISECONDS_PER_SECOND;
		}

		/// <summary>
		/// Converts seconds to minutes.
		/// </summary>
		public static double SecToMin(double sec)
		{
			return sec * MINUTES_PER_SECOND;
		}
		#endregion

		#region Minute
		/// <summary>
		/// Converts minutes to ticks.
		/// </summary>
		public static double MinToTick(double min)
		{
			return min * TICKS_PER_MINUTE;
		}

		/// <summary>
		/// Converts minutes to milliseconds.
		/// </summary>
		public static double MinToMs(double min)
		{
			return min * MILLISECONDS_PER_MINUTE;
		}

		/// <summary>
		/// Converts minutes to seconds.
		/// </summary>
		public static double MinToSec(double min)
		{
			return min * SECONDS_PER_MINUTE;
		}
		#endregion
		#endregion
	}
}