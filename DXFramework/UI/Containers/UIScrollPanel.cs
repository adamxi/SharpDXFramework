using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;
using System;

namespace DXFramework.UI
{
	public class UIScrollPanel : UIPanel
	{
		private EventHandler<EventArgs> onContentScrolled;
		public UIPanel contentPanel;
		private Accelerator2D accel;

		public UIScrollPanel()
		{
			AutoSize = false;
			Restriction = ScrollRestriction.Unrestricted;
			accel = new Accelerator2D();
			ClipContent = true;
			ClampContent = true;
			AbsorbPointer = false;
			AllowInsideScrolling = true;
			InputDown += UIScrollPanel_MouseHeld;
			InputPressed += UIScrollPanel_MousePressed;
			InputReleasedAnywhere += UIScrollPanel_MouseReleasedAnywhere;

			contentPanel = new UIPanel();
			contentPanel.AbsorbPointer = false;
			contentPanel.AutoSize = true;
			contentPanel.DrawBounds = false;

			contentPanel.AddConstraint(Edge.TopLeft, this, Edge.TopLeft);

			base.AddChild(contentPanel);
		}

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			var oldPos = contentPanel.Position;

			base.DoLayout(category);
			contentPanel.ResizeToContent(category);

			contentPanel.Position = oldPos;
		}

		public bool ClampContent { get; set; }

		public bool AllowInsideScrolling { get; set; }

		public override Vector2 Position
		{
			get { return base.Position; }
			set { base.Position = value; }
		}

		void UIScrollPanel_MouseHeld(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				accel.AddSample(InputManager.MouseDelta);
			}
			else
			{
				e.Absorbed = false;
			}
		}

		void UIScrollPanel_MousePressed(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				accel.Begin();
			}
			else
			{
				e.Absorbed = false;
			}
		}

		void UIScrollPanel_MouseReleasedAnywhere(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				accel.End();
			}
		}

		public event EventHandler<EventArgs> ContentScrolled
		{
			add { onContentScrolled += value.MakeWeak(e => onContentScrolled -= e); }
			remove { onContentScrolled -= onContentScrolled.Unregister(value); }
		}

		public ScrollRestriction Restriction { get; set; }

		public UIPanel ContentPanel
		{
			get { return contentPanel; }
		}

		public override bool AddChild(UIControl control)
		{
			bool success = contentPanel.AddChild(control);
			if (success && AutoSize)
			{
				ResizeToContent();
			}
			return success;
		}

		public override bool RemoveChild(UIControl control)
		{
			bool added = contentPanel.RemoveChild(control);
			if (added && AutoSize)
			{
				ResizeToContent();
			}
			return added;
		}

		public override void ClearChildren()
		{
			contentPanel.ClearChildren();
			if (AutoSize)
			{
				ResizeToContent();
			}
		}

		public void StopMovement()
		{
			accel.Stop();
		}

		private void OnContentScrolled()
		{
			if (onContentScrolled != null)
			{
				onContentScrolled.Invoke(this, null);
			}
		}

		public void ScrollToLocation(Vector2 location)
		{
			if (ClampContent)
			{
				contentPanel.Position = new Vector2(GetClampedX(location.X), GetClampedY(location.Y));
			}
			else
			{
				contentPanel.Position = location;
			}
		}

		private float GetClampedX(float x)
		{
			if (contentPanel.size.X < size.X)
			{
				return MathUtil.Clamp(x, 0, size.X - contentPanel.size.X);
			}
			else
			{
				return MathUtil.Clamp(x, size.X - contentPanel.size.X, 0);
			}
		}

		private float GetClampedY(float y)
		{
			if (contentPanel.size.Y < size.Y)
			{
				return MathUtil.Clamp(y, 0, size.Y - contentPanel.size.Y);
			}
			else
			{
				return MathUtil.Clamp(y, size.Y - contentPanel.size.Y, 0);
			}
		}

		private void SetContentX(float x)
		{
			if (contentPanel.X != x && (AllowInsideScrolling || contentPanel.Width > Width))
			{
				contentPanel.X = x;
				OnContentScrolled();
			}
		}

		private void SetContentY(float y)
		{
			if (contentPanel.Y != y && (AllowInsideScrolling || contentPanel.Height > Height))
			{
				contentPanel.Y = y;
				OnContentScrolled();
			}
		}

		private void SetContentPos(Vector2 pos)
		{
			if (contentPanel.position != pos)
			{
				if (!AllowInsideScrolling && contentPanel.Height < Height)
				{
					pos.Y = contentPanel.Position.Y;
				}
				if (!AllowInsideScrolling && contentPanel.Width < Width)
				{
					pos.X = contentPanel.Position.X;
				}

				contentPanel.Position = pos;
				OnContentScrolled();
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			accel.Update();

			switch (Restriction)
			{
				case ScrollRestriction.Horizontal:
					float x = contentPanel.X + accel.Speed.X;

					if (ClampContent)
					{
						x = GetClampedX(x);
					}

					SetContentX(x);
					break;

				case ScrollRestriction.Vertical:
					float y = contentPanel.Y + accel.Speed.Y;

					if (ClampContent)
					{
						y = GetClampedY(y);
					}

					SetContentY(y);
					break;

				case ScrollRestriction.Unrestricted:
					Vector2 pos = contentPanel.Position + accel.Speed;

					if (ClampContent)
					{
						pos.X = GetClampedX(pos.X);
						pos.Y = GetClampedY(pos.Y);
					}

					SetContentPos(pos);
					break;
			}
		}
	}
}