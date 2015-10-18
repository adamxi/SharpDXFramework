using System;
using SharpDX.Toolkit;

namespace DXFramework.Util
{
	public class Timer
	{
		private EventHandler<EventArgs> onTimerDone = delegate { };
		private bool countDown;
		private int targetValue;

		public Timer(int milliseconds, bool countDown = true)
		{
			Milliseconds = countDown ? milliseconds : 0;
			targetValue = countDown ? 0 : milliseconds;
			this.countDown = countDown;
		}

		public event EventHandler<EventArgs> TimerDone
		{
			add { onTimerDone += value.MakeWeak(e => onTimerDone -= e); }
			remove { onTimerDone -= onTimerDone.Unregister(value); }
		}

		public int Milliseconds { get; private set; }

		public float Seconds => Milliseconds * 0.001f;

		public bool Done { get; private set; }

		public void Update(GameTime gameTime)
		{
			if (!Done)
			{
				int time = (int)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (countDown)
				{
					Milliseconds -= time;
					if (Milliseconds <= 0)
					{
						SetDone();
					}
				}
				else
				{
					Milliseconds += time;
					if (Milliseconds >= targetValue)
					{
						SetDone();
					}
				}
			}
		}

		private void SetDone()
		{
			Done = true;
			onTimerDone.Invoke(this, EventArgs.Empty);
		}
	}
}