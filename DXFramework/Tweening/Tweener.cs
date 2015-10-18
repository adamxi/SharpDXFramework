using SharpDX;
using SharpDX.Toolkit;

namespace DXFramework.Tweening
{
	public class Tweener
	{
		private IEaseFunction easeFunction;
		private Vector2 from;
		private Vector2 to;
		private Vector2 distance;
		private float duration;
		private float invDuration;
		private float elapsed;

		public Tweener( Vector2 from, Vector2 to, float duration, IEaseFunction easeFunction )
		{
			this.from = from;
			this.to = to;
			this.duration = duration;
			this.easeFunction = easeFunction;

			invDuration = 1f / duration;
			distance = to - from;
			Restart();
		}

		public TweenState State { get; private set; }

		public Vector2 Value { get; private set; }

		public bool Done
		{
			get { return State == TweenState.Stopped; }
		}

		public void Restart()
		{
			State = TweenState.Running;
			Value = from;
			elapsed = 0;
		}

		public void Pause()
		{
			if( State == TweenState.Running )
			{
				State = TweenState.Paused;
			}
		}

		public void Resume()
		{
			switch( State )
			{
				case TweenState.Paused:
					State = TweenState.Running;
					break;

				case TweenState.Stopped:
					Restart();
					break;
			}
		}

		public void Stop()
		{
			Value = to;
			State = TweenState.Stopped;
		}

		public void Revert()
		{
			to = from;
			from = Value;
			distance = to - from;
			State = TweenState.Running;
			elapsed = 0;
		}

		public void Update( GameTime gameTime )
		{
			if( State != TweenState.Running )
			{
				return;
			}

			elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if( elapsed >= duration )
			{
				Stop();
				return;
			}

			Value = from + distance * easeFunction.Update( elapsed * invDuration );
		}

		public enum TweenState
		{
			Running,
			Paused,
			Stopped,
		}
	}
}