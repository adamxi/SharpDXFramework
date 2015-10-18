using System;
using SharpDX;

namespace DXFramework.Util
{
	public class Accelerator2D
	{
		private int bufferSize;
		private int bufferIndex;
		private int bufferCount;
		private Vector2[] buffer;
		private bool canUpdate;

		public Accelerator2D() : this( Vector2.Zero, new Vector2( 0.1f ), 2 ) { }

		public Accelerator2D( Vector2 maxSpeed ) : this( maxSpeed, new Vector2( 0.1f ), 2 ) { }

		public Accelerator2D( Vector2 maxSpeed, Vector2 deAccelerationCoefficient, int sampleBufferSize = 2 )
		{
			MaxSpeed = maxSpeed;
			DeAccelerationCoefficient = deAccelerationCoefficient;
			bufferSize = sampleBufferSize;
			Reset();
		}

		public Vector2 Speed { get; private set; }

		public Vector2 MaxSpeed { get; set; }

		public Vector2 DeAccelerationCoefficient { get; set; }

		public void Reset()
		{
			bufferIndex = 0;
			bufferCount = 0;
			buffer = new Vector2[ bufferSize ];
		}

		public void Stop()
		{
			Speed = Vector2.Zero;
		}

		public void Begin()
		{
			canUpdate = false;
			Reset();
		}

		public void End()
		{
			canUpdate = true;
			if( MaxSpeed != Vector2.Zero )
			{
				Speed = Clamp( CalcAverage(), -MaxSpeed, MaxSpeed );
			}
			else
			{
				Speed = CalcAverage();
			}
		}

		private Vector2 Clamp( Vector2 value, Vector2 min, Vector2 max )
		{
			value.X = MathUtil.Clamp( value.X, min.X, max.X );
			value.Y = MathUtil.Clamp( value.Y, min.Y, max.Y );
			return value;
		}

		public void AddSample( Vector2 sample )
		{
			if( MaxSpeed != Vector2.Zero )
			{
				Speed = Clamp( sample, -MaxSpeed, MaxSpeed );
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

		private Vector2 CalcAverage()
		{
			if( bufferCount <= 0 )
			{
				return Vector2.Zero;
			}

			Vector2 sum = Vector2.Zero;
			for( int i = bufferCount; --i >= 0; )
			{
				sum += buffer[ i ];
			}
			return sum / bufferCount;
		}

		public bool Update()
		{
			if( !canUpdate || Speed == Vector2.Zero )
			{
				return false;
			}
			Speed *= Vector2.One - DeAccelerationCoefficient;
			return true;
		}
	}
}