using SharpDX.Toolkit.Graphics;
using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit;

namespace DXFramework.UI
{
	public class UICheckBox : UIControl
	{
		private bool checkboxChecked;

		private PRect rect;
		private PRect innerRect;
		private UIImage checkmark;
		private UIContainer box;

		private float defaultAlpha = 0.2f;
		private float highlightAlpha = 0.4f;

		public UICheckBox(string checkmarkResource = "graphics/checkmark")
		{
			Size = new Vector2(20);
			Alpha = defaultAlpha;
			DrawBounds = true;
			Color = Color.Black;

			rect = new PRect(boundingRect, 3);
			rect.Color = Color.Black;

			checkmark = new UIImage(checkmarkResource);
			checkmark.Color = Color.Green;
			AddDecoration(checkmark);

			//innerRect = new PRect(boundingRect, 4);
			//innerRect.Color = Color.Black;
			//innerRect.Alpha = 30;

			InputReleased += UICheckBox_InputReleased;
			InputDown += UICheckBox_InputDown;
			InputEnter += UICheckBox_InputEnter;
			InputLeave += UICheckBox_InputLeave;
		}

		public bool Checked
		{
			get { return checkboxChecked; }
			set
			{
				checkboxChecked = value;
				checkmark.Visible = value;
			}
		}

		private void UICheckBox_InputReleased(object sender, MouseEventArgs e)
		{
			Checked = !Checked;
			Alpha = defaultAlpha;
		}

		private void UICheckBox_InputDown(object sender, MouseEventArgs e)
		{
			if (IntersectsPointer())
			{
				Alpha = highlightAlpha;
			}
		}

		private void UICheckBox_InputEnter(object sender, MouseEventArgs e)
		{
			rect.Color = Color.Blue;
		}

		private void UICheckBox_InputLeave(object sender, MouseEventArgs e)
		{
			Alpha = defaultAlpha;
			rect.Color = Color.Black;
		}

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			checkmark.Size = size;
			base.DoLayout(category);
		}

		public override void CalculateBoundingRectangle(ref Matrix transform)
		{
			base.CalculateBoundingRectangle(ref transform);

			rect.Position = boundingRect.Location;
			rect.Width = boundingRect.Width;
			rect.Height = boundingRect.Height;

			//innerRect.Position = boundingRect.Location;
			//innerRect.Width = boundingRect.Width;
			//innerRect.Height = boundingRect.Height;

			//innerRect.Position = boundingRect.Location + new Vector2(rect.Thickness);
			//innerRect.Width = boundingRect.Width - rect.Thickness * 2;
			//innerRect.Height = boundingRect.Height - rect.Thickness * 2;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			//innerRect.Draw();
			rect.Draw();
		}
	}
}