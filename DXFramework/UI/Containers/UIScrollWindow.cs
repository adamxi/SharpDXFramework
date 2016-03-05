using SharpDX;
using System;

namespace DXFramework.UI
{
	/// <summary>
	/// ScrollPanel with scroll bars
	/// </summary>
	public class UIScrollWindow : UIPanel
	{
		private ScrollBarMode mode;
		private UIPanel fillerPanel;

		public UIScrollWindow(ScrollBarMode mode = ScrollBarMode.Vertical)
		{
			this.mode = mode;
			AutoSize = false;
			DebugColor = Color.White;

			ScrollPanel = new UIScrollPanel();
			ScrollPanel.AutoSize = true;
			ScrollPanel.DrawBounds = false;
			ScrollPanel.AbsorbPointer = false;
			ScrollPanel.AllowInsideScrolling = false;
			ScrollPanel.ContentScrolled += scrollPanel_ContentScrolled;

			if (mode == ScrollBarMode.Both)
			{
				fillerPanel = new UIPanel();
				fillerPanel.Color = new Color(240, 240, 240);
				fillerPanel.AutoSize = false;
				fillerPanel.Size = new Vector2(16, 16);
				fillerPanel.Alpha = 1f;
				fillerPanel.AddConstraint(Edge.BottomRight, this, Edge.BottomRight);
				base.AddChild(fillerPanel);
			}

			if (mode.ContainsFlag(ScrollBarMode.Vertical))
			{
				VerticalScrollBar = new UIScrollBar(UIScrollBar.ScrollBarOrientation.Vertical);
				VerticalScrollBar.ValueChanged += scrollBar_ValueChanged;
				VerticalScrollBar.Width = 16f;
				VerticalScrollBar.AddConstraint(Edge.TopRight, this, Edge.TopRight);

				if (fillerPanel != null)
				{
					VerticalScrollBar.AddConstraint(Edge.Bottom, fillerPanel, Edge.Top);
				}
				else
				{
					VerticalScrollBar.AddConstraint(Edge.Bottom, this, Edge.Bottom, mode.ContainsFlag(ScrollBarMode.Horizontal) ? VerticalScrollBar.Width : 0);
				}
				ScrollPanel.AddConstraint(Edge.Left, this, Edge.Left, 1);
				ScrollPanel.AddConstraint(Edge.Right, VerticalScrollBar, Edge.Left, -1);
				base.AddChild(VerticalScrollBar);
			}
			else
			{
				ScrollPanel.AddConstraint(Edge.Left, this, Edge.Left, 1);
				ScrollPanel.AddConstraint(Edge.Right, this, Edge.Right, -1);
			}

			if (mode.ContainsFlag(ScrollBarMode.Horizontal))
			{
				HorizontalScrollBar = new UIScrollBar(UIScrollBar.ScrollBarOrientation.Horizontal);
				HorizontalScrollBar.ValueChanged += scrollBar_ValueChanged;
				HorizontalScrollBar.Height = 16f;
				HorizontalScrollBar.AddConstraint(Edge.BottomLeft, this, Edge.BottomLeft);

				if (fillerPanel != null)
				{
					HorizontalScrollBar.AddConstraint(Edge.Right, fillerPanel, Edge.Left);
				}
				else
				{
					HorizontalScrollBar.AddConstraint(Edge.Right, this, Edge.Right, mode.ContainsFlag(ScrollBarMode.Vertical) ? HorizontalScrollBar.Height : 0);
				}
				
				ScrollPanel.AddConstraint(Edge.Top, this, Edge.Top, 1);
				ScrollPanel.AddConstraint(Edge.Bottom, HorizontalScrollBar, Edge.Top, -1);
				base.AddChild(HorizontalScrollBar);
			}
			else
			{
				ScrollPanel.AddConstraint(Edge.Top, this, Edge.Top, 1);
				ScrollPanel.AddConstraint(Edge.Bottom, this, Edge.Bottom, -1);
			}



			base.AddChild(ScrollPanel);
		}

		public UIScrollPanel ScrollPanel { get; private set; }

		public UIScrollBar VerticalScrollBar { get; private set; }

		public UIScrollBar HorizontalScrollBar { get; private set; }

		void scrollPanel_ContentScrolled(object sender, EventArgs e)
		{
			UpdateScrollBarPosition();
		}

		void scrollBar_ValueChanged(object sender, EventArgs e)
		{
			ScrollPanel.StopMovement();

			Vector2 pos = ScrollPanel.ContentPanel.Position;

			if (mode.ContainsFlag(ScrollBarMode.Vertical))
			{
				float yDiff = ScrollPanel.ContentPanel.Height - ScrollPanel.Height;
				pos.Y = -yDiff * VerticalScrollBar.Value;
			}
			if (mode.ContainsFlag(ScrollBarMode.Horizontal))
			{
				float xDiff = ScrollPanel.ContentPanel.Width - ScrollPanel.Width;
				pos.X = -xDiff * HorizontalScrollBar.Value;
			}

			ScrollPanel.ScrollToLocation(pos);
		}

		private void UpdateScrollBarPosition()
		{
			if (mode.ContainsFlag(ScrollBarMode.Vertical))
			{
				float yDiff = ScrollPanel.ContentPanel.Height - ScrollPanel.Height;
				VerticalScrollBar.SetValue(ScrollPanel.ContentPanel.position.Y / -yDiff, true);
			}
			if (mode.ContainsFlag(ScrollBarMode.Horizontal))
			{
				float xDiff = ScrollPanel.ContentPanel.Width - ScrollPanel.Width;
				HorizontalScrollBar.SetValue(ScrollPanel.ContentPanel.position.X / -xDiff, true);
			}
		}

		public void CheckScrollBarsVisible(ConstraintCategory category = ConstraintCategory.All)
		{
			if (mode.ContainsFlag(ScrollBarMode.Vertical))
			{
				var v = ScrollPanel.ContentPanel.size.Y > size.Y;
				if (v != VerticalScrollBar.Visible)
				{
					VerticalScrollBar.Visible = v;
					VerticalScrollBar.DoLayout(category);
				}
			}
			if (mode.ContainsFlag(ScrollBarMode.Horizontal))
			{
				var v = ScrollPanel.ContentPanel.size.X > size.X;
				if (v != HorizontalScrollBar.Visible)
				{
					HorizontalScrollBar.Visible = v;
					HorizontalScrollBar.DoLayout(category);
				}
			}

			if (mode == ScrollBarMode.Both)
			{
				var showFiller = VerticalScrollBar.Visible && HorizontalScrollBar.Visible;
				if (showFiller != fillerPanel.Visible)
				{
					fillerPanel.Visible = showFiller;
					base.DoLayout(category);
				}
			}
		}

		public override bool AddChild(UIControl control)
		{
			bool success = ScrollPanel.AddChild(control);
			if (success && AutoSize)
			{
				ResizeToContent();
			}
			return success;
		}

		public override bool RemoveChild(UIControl control)
		{
			bool added = ScrollPanel.RemoveChild(control);
			if (added && AutoSize)
			{
				ResizeToContent();
			}
			return added;
		}

		public override void ClearChildren()
		{
			ScrollPanel.ClearChildren();
			if (AutoSize)
			{
				ResizeToContent();
			}
		}

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			base.DoLayout(category);
			CheckScrollBarsVisible(category);
			UpdateScrollBarPosition();
		}

		public enum ScrollBarMode
		{
			Vertical = 1,
			Horizontal = 2,
			Both = Vertical | Horizontal
		}
	}
}