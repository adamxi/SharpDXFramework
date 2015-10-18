using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DXFramework.Util
{
	/// <summary>
	/// Lightweight high performance profiler for debug builds.
	/// Release builds will not include any profiling code.
	/// </summary>
	public struct Profiler
	{
#if DEBUG
		[DllImport( "Kernel32.dll" )]
		private static extern bool QueryPerformanceCounter( out long lpPerformanceCount );
		[DllImport( "Kernel32.dll" )]
		private static extern bool QueryPerformanceFrequency( out long lpFrequency );
		private static double inverseFreq;

		static Profiler()
		{
			long freq;
			if( !QueryPerformanceFrequency( out freq ) )
			{
				throw new Win32Exception();
			}
			inverseFreq = 1d / freq;
		}
#endif

		public static Profiler StartNew( string description = null )
		{
			Profiler p = new Profiler( description );
			p.Start();
			return p;
		}

#if DEBUG
		private double totalElapsed;
		private string description;
		private int count;
		private long startTime;
		private double lastElapsed;
		private bool running;
#endif

		public Profiler( string description = null )
		{
#if DEBUG
			this.description = description;
			this.count = 0;
			this.startTime = 0;
			this.lastElapsed = 0;
			this.totalElapsed = 0;
			this.running = false;
#endif
		}

		#region Properties
		/// <summary>
		/// Profiler description
		/// </summary>
		public string Description
		{
			get
			{
#if DEBUG
				return description;
#else
				return string.Empty;
#endif
			}
		}

		/// <summary>
		/// How many times this profiler has been started.
		/// </summary>
		public int Count
		{
			get
			{
#if DEBUG
				return count;
#else
				return 0;
#endif
			}
		}

		/// <summary>
		/// Last elapsed time in seconds.
		/// </summary>
		public double LastElapsed
		{
			get
			{
#if DEBUG
				return lastElapsed;
#else
				return 0;
#endif
			}
		}

		/// <summary>
		/// Total elapsed time in seconds.
		/// </summary>
		public double TotalElapsed
		{
			get
			{
#if DEBUG
				return totalElapsed;
#else
				return 0;
#endif
			}
		}

		/// <summary>
		/// True if started and not stopped.
		/// </summary>
		public bool Running
		{
			get
			{
#if DEBUG
				return running;
#else
				return false;
#endif
			}
		}
		#endregion

		/// <summary>
		/// Starts the profiler. Call 'Stop' to benchmark times.
		/// </summary>
		public void Start()
		{
#if DEBUG
			count++;
			running = true;
			QueryPerformanceCounter( out startTime );
#endif
		}

		/// <summary>
		/// Stops the profiler.
		/// </summary>
		/// <param name="printOut">True to print out latest benchmark.</param>
		public void Stop( bool printOut = false )
		{
#if DEBUG
			if( running )
			{
				long stopTime;
				QueryPerformanceCounter( out stopTime );
				running = false;
				lastElapsed = ( stopTime - startTime ) * inverseFreq;
				totalElapsed += lastElapsed;

				if( printOut )
				{
					Console.WriteLine( ShortOutput() );
				}
			}
			else
			{
				throw new InvalidOperationException( "Profiler instance has not been started." );
			}
#endif
		}

		/// <summary>
		/// Resets the profiler.
		/// </summary>
		public void Reset()
		{
#if DEBUG
			count = 0;
			lastElapsed = 0;
			totalElapsed = 0;
			running = false;
#endif
		}

		/// <summary>
		/// Returns profiler information as a formated output string. Numbers are in seconds.
		/// </summary>
		/// <param name="tDigits">Number of digits to round total time to [0..15].</param>
		/// <param name="lDigits">Number of digits to round last-time to [0..15].</param>
		public string FullOutput( int tDigits = 2, int lDigits = 4 )
		{
#if DEBUG
			return description +
				"Total: " + Math.Round( totalElapsed, tDigits, MidpointRounding.AwayFromZero ).ToString( "0.".PadRight( 2 + tDigits, '0' ) ) +
				" |\tLast: " + Math.Round( lastElapsed, lDigits, MidpointRounding.AwayFromZero ).ToString( "0.".PadRight( 2 + lDigits, '0' ) ) +
				" |\tHit: " + count;
#else
			return string.Empty;
#endif
		}

		/// <summary>
		/// Returns profiler information as a formated output string. Numbers are in seconds.
		/// </summary>
		/// <param name="tDigits">Number of digits to round total time to [0..15].</param>
		public string ShortOutput( int digits = 2 )
		{
#if DEBUG
			return description + ": " + Math.Round( totalElapsed, digits, MidpointRounding.AwayFromZero ).ToString() + " sec";
#else
			return string.Empty;
#endif
		}

		public override string ToString()
		{
			return FullOutput();
		}
	}
}