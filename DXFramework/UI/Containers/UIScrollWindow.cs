using System;
using SharpDX;

namespace DXFramework.UI
{
	/// <summary>
	/// ScrollPanel with scroll bars
	/// </summary>
	public class UIScrollWindow : UIPanel
	{
		public UIScrollWindow()
		{
			this.AutoSize = false;

			ScrollBar = new UIScrollBarWithArrows();
			ScrollBar.DrawBounds = false;
			ScrollBar.ValueChanged += scrollBar_ValueChanged;
			
			ScrollPanel = new UIScrollPanel();
			ScrollPanel.AutoSize = false;
			ScrollPanel.DrawBounds = false;
			ScrollPanel.AbsorbPointer = false;
			ScrollPanel.ContentScrolled += scrollPanel_ContentScrolled;

			ScrollBar.AddConstraint(Edge.Vertical | Edge.Right, this, Edge.Vertical | Edge.Right);
			ScrollPanel.AddConstraint(Edge.Vertical | Edge.Left, this, Edge.Vertical | Edge.Left);
			ScrollPanel.AddConstraint(Edge.Right, ScrollBar, Edge.Left);

			base.AddChild(ScrollBar);
			base.AddChild(ScrollPanel);
		}

		public UIScrollPanel ScrollPanel { get; set; }

		public UIScrollBarWithArrows ScrollBar { get; set; }

		void scrollPanel_ContentScrolled(object sender, EventArgs e)
		{
			float yDiff = ScrollPanel.ContentPanel.Height - ScrollPanel.Height;
			ScrollBar.SetValue(ScrollPanel.ContentPanel.location.Y / -yDiff, true);
		}

		void scrollBar_ValueChanged(object sender, EventArgs e)
		{
			ScrollPanel.StopMovement();

			float yDiff = ScrollPanel.ContentPanel.Height - ScrollPanel.Height;
			ScrollPanel.ScrollToLocation(new Vector2(ScrollPanel.ContentPanel.X, -yDiff * ScrollBar.Value));
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
			ScrollBar.Width = Math.Min(30, Width * 0.2f);
			base.DoLayout(category);
		}
	}
}