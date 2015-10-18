using DXFramework.Tweening;
using SharpDX;
using SharpDX.Toolkit;

namespace DXFramework.UI
{
	public class UITweenPanel : UIPanel
	{
		private Tweener interpolator;
		private Vector2 targetPosition;

		public UITweenPanel()
		{
			DebugColor = Color.Green;
			TargetPosition = new Vector2(-1);
			EntryTween = new Elastic.EaseOut();
			ExitTween = new Back.EaseIn();
			Duration = 0.7f;
		}

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			base.DoLayout(category);

			Vector2 size = ViewportSize;
			Rectangle rect = new Rectangle(0, 0, (int)size.X, (int)size.Y);
			if (rect.Intersects(location))
			{
				State = TweenState.Shown;
			}
			else
			{
				State = TweenState.Hidden;
			}
		}

		public Vector2 EntryVector { get; set; }

		public Vector2 ExitVector { get; set; }

		public Vector2 TargetPosition
		{
			get { return targetPosition; }
			set { targetPosition = value; }
		}

		public TweenState State { get; private set; }

		public IEaseFunction EntryTween { get; set; }

		public IEaseFunction ExitTween { get; set; }

		public float Duration { get; set; }

		public Vector2 ViewportSize
		{
			get
			{
				if (HasParent)
				{
					return Parent.Size;
				}
				return Engine.ScreenSize;
			}
		}

		public bool IsFullyShown
		{
			get { return State == TweenState.Shown; }
		}

		public bool IsFullyHidden
		{
			get { return State == TweenState.Hidden; }
		}

		public bool InShowingState
		{
			get { return State == TweenState.Entering || State == TweenState.Shown; }
		}

		public bool InHidingState
		{
			get { return State == TweenState.Hidden || State == TweenState.Leaving; }
		}

		private Edge GetClosestEdge()
		{
			Vector2 center = LocalCenter;
			Vector2 screenSize = ViewportSize;
			Vector2 halfScreenSize = screenSize * 0.5f;

			Vector2[] edges = new Vector2[]
			{
				new Vector2( 0, halfScreenSize.Y ),				// Left center
				new Vector2( screenSize.X, halfScreenSize.Y ),	// Right center
				new Vector2( halfScreenSize.X, 0 ),				// Top center
				new Vector2( halfScreenSize.X, screenSize.Y )	// Bottom center
			};

			int index = 0;
			float minDist = float.MaxValue;

			for (int i = edges.Length; --i >= 0;)
			{
				Vector2 dist = edges[i] - center;
				float lengthSquared = dist.LengthSquared();

				if (lengthSquared < minDist)
				{
					minDist = lengthSquared;
					index = i;
				}
			}

			return (Edge)(1 << index);
		}

		public bool Toggle(Edge edge = Edge.None, float duration = -1, IEaseFunction easeFunction = null)
		{
			if (IsFullyShown)
			{
				return Hide(edge, duration, easeFunction);
			}
			else if (IsFullyHidden)
			{
				return Show(edge, duration, easeFunction);
			}
			return false;
		}

		public bool Show(Edge edge = Edge.None, float duration = -1, IEaseFunction easeFunction = null)
		{
			switch (State)
			{
				case TweenState.Entering:
				case TweenState.Shown:
					return false;
			}

			if (edge == Edge.None)
			{
				edge = GetClosestEdge();
			}
			if (duration == -1)
			{
				duration = Duration;
			}
			if (easeFunction == null)
			{
				easeFunction = EntryTween;
			}

			Vector2 view = ViewportSize;

			if (targetPosition.X == -1)
			{
				switch (edge)
				{
					case Edge.Left:
						targetPosition.X = 0;
						break;
					case Edge.Right:
						targetPosition.X = view.X - Width;
						break;
				}
			}
			if (targetPosition.Y == -1)
			{
				switch (edge)
				{
					case Edge.Top:
						targetPosition.Y = 0;
						break;
					case Edge.Bottom:
						targetPosition.Y = view.Y - Height;
						break;
				}
			}

			State = TweenState.Entering;
			Enabled = true;
			Visible = true;
			Vector2 from = targetPosition;

			switch (edge)
			{
				case Edge.Left:
					from.X = -Width;
					break;
				case Edge.Right:
					from.X = view.X;
					break;
				case Edge.Top:
					from.Y = -Height;
					break;
				case Edge.Bottom:
					from.Y = view.Y;
					break;
			}

			interpolator = new Tweener(from, targetPosition, duration, easeFunction);
			return true;
		}

		public bool Hide(Edge edge = Edge.None, float duration = -1, IEaseFunction easeFunction = null)
		{
			switch (State)
			{
				case TweenState.Leaving:
				case TweenState.Hidden:
					return false;
			}

			if (edge == Edge.None)
			{
				edge = GetClosestEdge();
			}
			if (duration == -1)
			{
				duration = Duration;
			}
			if (easeFunction == null)
			{
				easeFunction = ExitTween;
			}
			if (targetPosition.X == -1)
			{
				targetPosition.X = Location.X;
			}
			if (targetPosition.Y == -1)
			{
				targetPosition.Y = Location.Y;
			}

			State = TweenState.Leaving;
			Enabled = true;
			Visible = true;
			Vector2 to = location;
			Vector2 view = ViewportSize;

			switch (edge)
			{
				case Edge.Left:
					to.X = -Width;
					break;
				case Edge.Right:
					to.X = view.X;
					break;
				case Edge.Top:
					to.Y = -Height;
					break;
				case Edge.Bottom:
					to.Y = view.Y;
					break;
			}

			interpolator = new Tweener(location, to, duration, easeFunction);
			return true;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			switch (State)
			{
				case TweenState.Entering:
					interpolator.Update(gameTime);
					Location = interpolator.Value;

					if (interpolator.Done)
					{
						State = TweenState.Shown;
					}
					break;

				case TweenState.Leaving:
					interpolator.Update(gameTime);
					Location = interpolator.Value;

					if (interpolator.Done)
					{
						State = TweenState.Hidden;
					}
					break;

				case TweenState.Hidden:
					Enabled = false;
					Visible = false;
					break;
			}
		}

		public enum TweenState
		{
			None,
			Entering,
			Shown,
			Leaving,
			Hidden
		}
	}
}