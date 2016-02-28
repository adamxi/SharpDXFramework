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

		public int MarginAll
		{
			set
			{
				MarginTop = value;
				MarginLeft = value;
				MarginBottom = value;
				MarginRight = value;
			}
		}

		public int MarginTop { get; set; }

		public int MarginLeft { get; set; }

		public int MarginBottom { get; set; }

		public int MarginRight { get; set; }

		//public override Vector2 Size
		//{
		//	get { return base.Size; }
		//	set
		//	{
		//		if (size != value)
		//		{
		//			size = value;
		//			if (!SuspendLayout)
		//			{
		//				base.DoLayout(ConstraintCategory.Update);
		//			}
		//			UpdateTransformation = true;
		//		}
		//	}
		//}

		public void ResizeToContent(ConstraintCategory category = ConstraintCategory.All)
		{
			RectangleF rect = CalcContentBounds(true);
			size = new Vector2(rect.Width, rect.Height);
			UpdateTransformation = true;
			base.DoLayout(category);

			//SetMargin();
		}

		//private void SetMargin()
		//{
		//	if (MarginLeft != 0 || MarginRight != 0 || MarginTop != 0 || MarginBottom != 0)
		//	{
		//		DrawPosition += new Vector2(-MarginLeft, -MarginTop);
		//		size += new Vector2(MarginRight + MarginLeft, MarginBottom + MarginTop);

		//		UpdateTransformation = true;
		//		//base.DoLayout(ConstraintCategory.Update);
		//	}
		//}

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

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			if (AutoSize)
			{
				ResizeToContent(category);
			}
			else
			{
				base.DoLayout(category);
			}
		}
	}
}