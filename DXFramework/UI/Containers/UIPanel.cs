using SharpDX;

namespace DXFramework.UI
{
	public class UIPanel : UIContainer
	{
		public UIPanel()
		{
			AbsorbPointer = true;
			AutoSize = true;
			DrawBounds = true;
			Alpha = 0.5f;
			Color = Color.Black;
			DebugColor = Color.Green;
		}

		public bool AutoSize { get; set; }

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			base.DoLayout(category);
			if (AutoSize)
			{
				ResizeToContent();
			}
		}

		public override bool AddChild(UIControl control)
		{
			if (base.AddChild(control))
			{
				if (AutoSize)
				{
					ResizeToContent();
				}
				return true;
			}
			return false;
		}

		public override bool RemoveChild(UIControl control)
		{
			if (base.RemoveChild(control))
			{
				if (AutoSize)
				{
					ResizeToContent();
				}
				return true;
			}
			return false;
		}

		public override void ClearChildren()
		{
			base.ClearChildren();
			if (AutoSize)
			{
				ResizeToContent();
			}
		}

		public void ResizeToContent()
		{
			RectangleF rect = CalcContentBounds(true);
			Size = new Vector2(rect.Width, rect.Height);
		}
	}
}