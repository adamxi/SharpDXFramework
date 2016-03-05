using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;

namespace DXFramework.UI
{
	public class UIScrollBar : UIPanel
	{
		private EventHandler<EventArgs> onValueChanged;
		private UIControl sliderBackground;
		private UIPanel slider;
		private UIButton arrowA;
		private UIButton arrowB;
		private float scrollStep;

		public UIScrollBar(ScrollBarOrientation orientation = ScrollBarOrientation.Vertical)
		{
			Orientation = orientation;
			string arrowResource = string.Empty;
			Vector2 sliderSize = Vector2.Zero;

			switch (orientation)
			{
				case ScrollBarOrientation.Vertical:
					sliderSize = new Vector2(0, 20);
					arrowResource = "graphics/arrow_down";
					break;

				case ScrollBarOrientation.Horizontal:
					sliderSize = new Vector2(20, 0);
					arrowResource = "graphics/arrow_left";
					break;
			}

			AutoSize = false;
			scrollStep = 0.02f;
			Color = new Color(240, 240, 240);
			Alpha = 1f;
			DrawBounds = true;

			sliderBackground = new UIPanel();
			sliderBackground.Color = new Color(240, 240, 240);
			sliderBackground.Alpha = 1f;
			sliderBackground.InputMoved += sliderBackground_MouseMoved;
			sliderBackground.InputPressed += sliderBackground_MousePressed;

			slider = new UIPanel();
			slider.Color = new Color(205, 205, 205);
			slider.Alpha = 1f;
			slider.AutoSize = false;
			slider.AbsorbPointer = false;
			slider.Size = sliderSize;
			//slider.InputDown += sliderBackground_MousePressed;

			arrowA = new UIButton();
			var arrowAImg = new UIImage(arrowResource);
			arrowAImg.Color = new Color(96, 96, 96);
			arrowAImg.AddConstraint(Edge.Dock, arrowA, Edge.Dock);
			arrowA.Tag = "bla";
			arrowA.AddDecoration(arrowAImg);
			arrowA.PointedColor = new Color(190, 190, 190);
			arrowA.PressedColor = new Color(120, 120, 120);
			arrowA.HighlightZoom = false;
			arrowA.InputDown += arrowA_MouseHeld;

			arrowB = new UIButton();
			var arrowBImg = new UIImage(arrowResource);
			arrowBImg.Color = new Color(96, 96, 96);
			arrowBImg.AddConstraint(Edge.Dock, arrowB, Edge.Dock);
			arrowB.AddDecoration(arrowBImg);
			arrowB.PointedColor = new Color(190, 190, 190);
			arrowB.PressedColor = new Color(120, 120, 120);
			arrowB.HighlightZoom = false;
			arrowB.InputDown += arrowB_MouseHeld;
			
			switch (orientation)
			{
				case ScrollBarOrientation.Vertical:
					arrowAImg.SpriteEffect = SpriteEffects.FlipVertically;
					arrowA.AddConstraint(Edge.Horizontal | Edge.Top, this, Edge.Horizontal | Edge.Top);
					arrowB.AddConstraint(Edge.Horizontal, this, Edge.Horizontal);
					arrowB.AddConstraint(Edge.Bottom, this, Edge.Bottom);
					sliderBackground.AddConstraint(Edge.Horizontal, this, Edge.Horizontal);
					sliderBackground.AddConstraint(Edge.Top, arrowA, Edge.Bottom);
					sliderBackground.AddConstraint(Edge.Bottom, arrowB, Edge.Top);
					slider.AddConstraint(Edge.Top, sliderBackground, Edge.Top, ConstraintCategory.Initialization);
					slider.AddConstraint(Edge.Horizontal, sliderBackground, Edge.Horizontal);
					break;

				case ScrollBarOrientation.Horizontal:
					arrowBImg.SpriteEffect = SpriteEffects.FlipHorizontally;
					arrowA.AddConstraint(Edge.Vertical | Edge.Left, this, Edge.Vertical | Edge.Left);
					arrowB.AddConstraint(Edge.Vertical, this, Edge.Vertical);
					arrowB.AddConstraint(Edge.Right, this, Edge.Right);
					sliderBackground.AddConstraint(Edge.Vertical, this, Edge.Vertical);
					sliderBackground.AddConstraint(Edge.Left, arrowA, Edge.Right);
					sliderBackground.AddConstraint(Edge.Right, arrowB, Edge.Left);
					slider.AddConstraint(Edge.Left, sliderBackground, Edge.Left, ConstraintCategory.Initialization);
					slider.AddConstraint(Edge.Vertical, sliderBackground, Edge.Vertical);
					break;
			}


			AbsorbPointer = false;
			//base.AbsorbPointer = true;
			//slider.AbsorbPointer = true;
			//arrowDown.AbsorbPointer = true;
			//arrowUp.AbsorbPointer = true;
			sliderBackground.AbsorbPointer = true;

			AddChild(arrowA);
			AddChild(arrowB);
			AddChild(sliderBackground);
			AddChild(slider);
		}

		void sliderBackground_MousePressed(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				switch (Orientation)
				{
					case ScrollBarOrientation.Vertical:
						SetValue((e.ClientPosition.Y - slider.HalfHeight) / SliderRange);
						break;

					case ScrollBarOrientation.Horizontal:
						SetValue((e.ClientPosition.X - slider.HalfWidth) / SliderRange);
						break;
				}

				e.Absorbed = true;
			}
		}

		void sliderBackground_MouseMoved(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				switch (Orientation)
				{
					case ScrollBarOrientation.Vertical:
						SetValue((e.ClientPosition.Y - slider.HalfHeight) / SliderRange);
						break;

					case ScrollBarOrientation.Horizontal:
						SetValue((e.ClientPosition.X - slider.HalfWidth) / SliderRange);
						break;
				}

				e.Absorbed = true;
			}
		}

		void arrowA_MouseHeld(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				IncrementValue(-scrollStep);
				e.Absorbed = true;
			}
		}

		void arrowB_MouseHeld(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				IncrementValue(scrollStep);
				e.Absorbed = true;
			}
		}

		public event EventHandler<EventArgs> ValueChanged
		{
			add { onValueChanged += value.MakeWeak(e => onValueChanged -= e); }
			remove { onValueChanged -= onValueChanged.Unregister(value); }
		}

		public float Value { get; private set; }

		public ScrollBarOrientation Orientation { get; private set; }

		private float SliderRange
		{
			get
			{
				switch (Orientation)
				{
					case ScrollBarOrientation.Vertical:
						return sliderBackground.Height - slider.Height;

					case ScrollBarOrientation.Horizontal:
						return sliderBackground.Width - slider.Width;

					default:
						return 0;
				}
			}
		}

		//public override bool AbsorbPointer
		//{
		//	get { return base.AbsorbPointer; }
		//	set
		//	{
		//		base.AbsorbPointer = value;
		//		if (slider != null)
		//		{
		//			slider.AbsorbPointer = value;
		//			arrowDown.AbsorbPointer = value;
		//			arrowUp.AbsorbPointer = value;
		//			sliderBackground.AbsorbPointer = value;
		//		}
		//	}
		//}

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			switch (Orientation)
			{
				case ScrollBarOrientation.Vertical:
					arrowA.Size = new Vector2(Width);
					arrowB.Size = new Vector2(Width);
					break;

				case ScrollBarOrientation.Horizontal:
					arrowA.Size = new Vector2(Height);
					arrowB.Size = new Vector2(Height);
					break;
			}

			base.DoLayout(category);
		}

		public void IncrementValue(float increment)
		{
			SetValue(Value + increment);
		}

		public void SetValue(float value, bool surppressEvent = false)
		{
			float newValue = MathUtil.Clamp(value, 0, 1);
			if (Value != newValue)
			{
				Value = newValue;

				switch (Orientation)
				{
					case ScrollBarOrientation.Vertical:
						slider.Position = new Vector2(slider.Position.X, Value * SliderRange + sliderBackground.Position.Y);
						break;

					case ScrollBarOrientation.Horizontal:
						slider.Position = new Vector2(Value * SliderRange + sliderBackground.Position.X, slider.Position.Y);
						break;
				}

				if (!surppressEvent && onValueChanged != null)
				{
					onValueChanged.Invoke(this, null);
				}
			}
		}

		public enum ScrollBarOrientation
		{
			Vertical,
			Horizontal
		}
	}
}