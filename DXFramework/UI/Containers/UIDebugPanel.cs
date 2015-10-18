using System.Collections.Generic;

namespace DXFramework.UI
{
	public class UIDebugPanel : UIPanel
	{
		private Dictionary<string, UILabel> valueLabels;

		public UIDebugPanel()
		{
			this.valueLabels = new Dictionary<string, UILabel>();
			this.AutoSize = false;
			this.AddConstraint(Edge.TopRight, null, Edge.TopRight, 10);
		}

		public void SetDebugValue(string label, string value = null)
		{
			UILabel valueLabel;
			if (valueLabels.TryGetValue(label, out valueLabel))
			{
				if (valueLabel.SetText(value))
				{
					ResizeToContent();
				}
			}
			else
			{
				AddDebugPair(label, value);
			}
		}

		private UIControl previousAnchor;

		private void AddDebugPair(string labelText, string labelValue = null)
		{
			UILabel label = new UILabel(labelText);
			UILabel value = new UILabel(labelValue);

			label.AddConstraint(Edge.Left, this, Edge.Left, 5);
			value.AddConstraint(Edge.Left, this, Edge.Left, 140);

			if (previousAnchor == null)
			{
				label.AddConstraint(Edge.Top, this, Edge.Top, 5);
				value.AddConstraint(Edge.Top, this, Edge.Top, 5);
			}
			else
			{
				label.AddConstraint(Edge.Top, previousAnchor, Edge.Bottom);
				value.AddConstraint(Edge.Top, previousAnchor, Edge.Bottom);
			}

			previousAnchor = label;

			valueLabels.Add(labelText, value);
			AddChild(label);
			AddChild(value);

			if (!SuspendLayout)
			{
				label.SuspendLayout = false;
				value.SuspendLayout = false;
				label.DoLayout();
				value.DoLayout();
			}
		}
	}
}