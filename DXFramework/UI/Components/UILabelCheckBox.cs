namespace DXFramework.UI
{
	public class UILabelCheckBox : UIPanel
	{
		public UILabelCheckBox(string label, string checkmarkResource = "graphics/checkmark")
		{
			Alpha = 0f;

			var uiLabel = new UILabel(label);
			CheckBox = new UICheckBox(checkmarkResource);

			uiLabel.AddConstraint(Edge.Left, this, Edge.Left);
			CheckBox.AddConstraint(Edge.Left, uiLabel, Edge.Right, -10);

			AddChild(uiLabel);
			AddChild(CheckBox);
		}

		public UICheckBox CheckBox { get; private set; }
	}
}