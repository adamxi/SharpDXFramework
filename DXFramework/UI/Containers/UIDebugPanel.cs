using System.Collections.Generic;

namespace DXFramework.UI
{
	public class UIDebugPanel : UIPanel
	{
		private Dictionary<string, UILabel> valueLabels;
		private UIControl previousAnchor;
		private int nextYOffset;

		public UIDebugPanel()
		{
			this.valueLabels = new Dictionary<string, UILabel>();
			this.AutoSize = true;
			this.AddConstraint(Edge.TopRight, null, Edge.TopRight, 10);
		}

		public void SetDebugValue(string labelText, object value = null)
		{
			UILabel valueLabel;
			if (valueLabels.TryGetValue(labelText, out valueLabel))
			{
				if (valueLabel.SetText(value?.ToString()))
				{
					ResizeToContent();
				}
			}
			else
			{
				AddDebugPair(labelText, value?.ToString());
			}
		}

		public void AddLineBreak(int space = 10)
		{
			nextYOffset += -space;
		}

		public void SetStaticLabel(string labelText)
		{
			UILabel label = new UILabel(labelText);

			label.AddConstraint(Edge.Left, this, Edge.Left, 5);

			if (previousAnchor == null)
			{
				label.AddConstraint(Edge.Top, this, Edge.Top, 5 - nextYOffset);
			}
			else
			{
				label.AddConstraint(Edge.Top, previousAnchor, Edge.Bottom, nextYOffset);
			}

			nextYOffset = 0;
			previousAnchor = label;

			AddChild(label);

			if (!SuspendLayout)
			{
				label.DoLayout();
			}
		}

		private void AddDebugPair(string labelText, string labelValue = null)
		{
			UILabel label = new UILabel(labelText);
			UILabel value = new UILabel(labelValue);

			label.AddConstraint(Edge.Left, this, Edge.Left, 5);
			value.AddConstraint(Edge.Left, this, Edge.Left, 140);


			if (previousAnchor == null)
			{
				label.AddConstraint(Edge.Top, this, Edge.Top, 5 - nextYOffset);
				value.AddConstraint(Edge.Top, this, Edge.Top, 5 - nextYOffset);
			}
			else
			{
				label.AddConstraint(Edge.Top, previousAnchor, Edge.Bottom, nextYOffset);
				value.AddConstraint(Edge.Top, previousAnchor, Edge.Bottom, nextYOffset);
			}

			nextYOffset = 0;
			previousAnchor = label;

			valueLabels.Add(labelText, value);
			AddChild(label);
			AddChild(value);

			if (!SuspendLayout)
			{
				label.DoLayout();
				value.DoLayout();
			}
		}
	}
}