using System;
using SharpDX;

namespace DXFramework.Util
{
	public class Accelerator1D
	{
		private int bufferSize;
		private int bufferIndex;
		private int bufferCount;
		private float[] buffer;
		private bool canUpdate;

		public Accelerator1D( float maxSpeed = 0, float deAccelerationCoefficient = 0.1f, int sampleBufferSize = 2 )
		{
			MaxSpeed = maxSpeed;
			DeAccelerationCoefficient = deAccelerationCoefficient;
			bufferSize = sampleBufferSize;
			Reset();
		}

		public float Speed { get; private set; }

		public float MaxSpeed { get; set; }

		public float DeAccelerationCoefficient { get; set; }

		private void Reset()
		{
			bufferIndex = 0;
			bufferCount = 0;
			buffer = new float[ bufferSize ];
		}

		public void Start()
		{
			canUpdate = false;
			Reset();
		}

		public void Stop()
		{
			canUpdate = true;
			if( MaxSpeed != 0 )
			{
				Speed = MathUtil.Clamp( CalcAverage(), -MaxSpeed, MaxSpeed );
			}
			else
			{
				Speed = CalcAverage();
			}
		}

		public void AddSample( float sample )
		{
			if( MaxSpeed != 0 )
			{
				Speed = MathUtil.Clamp( sample, -MaxSpeed, MaxSpeed );
			}
			else
			{
				Speed = sample;
			}

			buffer[ bufferIndex++ ] = sample;
			if( bufferIndex >= bufferSize )
			{
				bufferIndex = 0;
			}
			bufferCount = Math.Min( bufferCount + 1, bufferSize );
		}

		private float CalcAverage()
		{
			if( bufferCount <= 0 )
			{
				return 0;
			}

			float sum = 0;
			for( int i = bufferCount; --i >= 0; )
			{
				sum += buffer[ i ];
			}
			return sum / bufferCount;
		}

		public void Update()
		{
			if( !canUpdate || Speed == 0 )
			{
				return;
			}
			Speed *= 1 - DeAccelerationCoefficient;
		}
	}
}