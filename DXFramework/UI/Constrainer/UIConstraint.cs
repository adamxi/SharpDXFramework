namespace DXFramework.UI
{
	public class UIConstraint
	{
		public UIConstraint(Edge controlEdge, UIControl anchor, Edge anchorEdge, float distance = 0f, ConstraintCategory constraintCategory = ConstraintCategory.All)
		{
			ControlEdge = controlEdge;
			Anchor = anchor;
			AnchorEdge = anchorEdge;
			Distance = distance;
			Category = constraintCategory;
		}

		public Edge ControlEdge { get; }

		public UIControl Anchor { get; }

		public Edge AnchorEdge { get; }

		public float Distance { get; }

		public ConstraintCategory Category { get; }
	}
}