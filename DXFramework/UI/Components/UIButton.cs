using DXFramework.Tweening;
using SharpDX;
using SharpDX.Toolkit;

namespace DXFramework.UI
{
	public class UIButton : UIControl
	{
		private Tweener tweener;
		private Vector2 orgScale;
		private Color defaultColor;
		private float defaultAlpha;

		public UIButton()
		{
			DebugColor = Color.Blue;
			AbsorbPointer = true;
			DrawBounds = true;

			HighlightZoom = true;
			Color = Color.Black;
			PointedColor = Color.Red;
			PressedColor = Color.Red;
			Alpha = 0f;
			PointedAlpha = 0.9f;
			PressedAlpha = 0.6f;

			InputEnter += UIButton_InputEnter;
			InputLeave += UIButton_InputLeave;
			InputDown += UIButton_InputDown;
			InputReleased += UIButton_InputReleased;
		}

		public bool HighlightZoom { get; set; }

		public Color PointedColor { get; set; }

		public Color PressedColor { get; set; }

		public float PointedAlpha { get; set; }

		public float PressedAlpha { get; set; }

		public override Color Color
		{
			get { return base.Color; }
			set
			{
				base.Color = value;
				defaultColor = value;
			}
		}

		public override float Alpha
		{
			get { return base.Alpha; }
			set
			{
				base.Alpha = value;
				defaultAlpha = value;
			}
		}

		public override T AddDecoration<T>(T decoration)
		{
			if (decoration == null)
			{
				return default(T);
			}
			T deco = base.AddDecoration<T>(decoration);
			Size = Vector2.Max(Size, deco.Size);
			return deco;
		}

		void UIButton_InputDown(object sender, MouseEventArgs e)
		{
			if (IntersectsPointer())
			{
				base.Color = PressedColor;
				base.Alpha = PressedAlpha;
			}
		}

		void UIButton_InputReleased(object sender, MouseEventArgs e)
		{
			base.Color = PointedColor;
			base.Alpha = PointedAlpha;
		}

		void UIButton_InputEnter(object sender, MouseEventArgs e)
		{
			base.Color = PointedColor;
			base.Alpha = PointedAlpha;

			if (HighlightZoom)
			{
				if (tweener == null || tweener.Done)
				{
					orgScale = Scale;
				}
				tweener = new Tweener(Scale, orgScale * 1.20f, 0.15f, new Linear.EaseNone());
			}
		}

		void UIButton_InputLeave(object sender, MouseEventArgs e)
		{
			base.Color = defaultColor;
			base.Alpha = defaultAlpha;

			if (HighlightZoom)
			{
				tweener = new Tweener(Scale, orgScale, 0.1f, new Linear.EaseNone());
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (tweener != null)
			{
				tweener.Update(gameTime);
				SetScale(tweener.Value, true);
			}
		}
	}
}