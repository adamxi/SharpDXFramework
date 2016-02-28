using System;
using System.Collections.Generic;
using System.Linq;

namespace DXFramework.UI
{
	internal class UIConstrainer
	{
		static UIConstrainer()
		{
			EdgeTypes = new List<Edge>(6) { Edge.Left, Edge.Right, Edge.Top, Edge.Bottom, Edge.CenterX, Edge.CenterY };
		}

		internal static List<Edge> EdgeTypes { get; }

		internal UIConstrainer()
		{
			Constraints = new List<UIConstraint>(4);
		}

		public List<UIConstraint> Constraints { get; }

		internal void AddConstraint(UIConstraint constraint)
		{
			AddConstraint(constraint.ControlEdge, constraint.Anchor, constraint.AnchorEdge, constraint.Distance, constraint.Category);
		}

		/// <summary>
		/// Adds a UI constraint to this control.
		/// </summary>
		/// <param name="controlEdge">Control edge to constraint.</param>
		/// <param name="anchor">Anchoring control on which the control edge is constrained. Setting this to 'null' the control is anchored relative to the screen.</param>
		/// <param name="anchorEdge">Anchor edge, on which the control edge is constrained relative to.</param>
		/// <param name="edgeDistance">Distance between control and anchor constraint edge.</param>
		internal void AddConstraint(Edge controlEdge, UIControl anchor, Edge anchorEdge, float edgeDistance, ConstraintCategory category)
		{
			// Separate edges into their basic flags.
			// E.g. 'Edge.BottomRight' is separated into 'Edge.Bottom' and 'Edge.Right', and added as individual edges.
			List<Edge> controlEdges = new List<Edge>(4);
			List<Edge> anchorEdges = new List<Edge>(4);
			foreach (Edge edgeType in EdgeTypes)
			{
				if (controlEdge.HasFlag(edgeType))
				{
					controlEdges.Add(edgeType);
				}
				if (anchorEdge.HasFlag(edgeType))
				{
					anchorEdges.Add(edgeType);
				}
			}

			if (controlEdges.Count != anchorEdges.Count)
			{
				throw new ArgumentException("There must be an equal amount of control and anchor edges.");
			}

			for (int i = 0; i < controlEdges.Count; i++)
			{
				var cEdge = controlEdges[i];
				var aEdge = anchorEdges[i];

				if (!OnSameAxis(cEdge, aEdge))
				{
					throw new ArgumentException($"Control and anchor edge and must be on the same axis: {cEdge}, {aEdge}");
				}

				foreach (var edge in Constraints.Select(c => c.ControlEdge))
				{
					if (edge.Equals(cEdge))
					{
						throw new ArgumentException($"Control edge already bound: {cEdge}");
					}
					if (Edge.CenterXY.ContainsFlag(cEdge | edge) && OnSameAxis(cEdge, edge)) // Special case to check that no edge is bound to an axis along with a center edge. E.g.: CenterY and Top.
					{
						throw new ArgumentException($"Control axis already bound: {cEdge}, {edge}");
					}
				}

				Constraints.Add(new UIConstraint(cEdge, anchor, aEdge, edgeDistance, category));
			}
		}

		private bool HasHorizontalEdge()
		{
			foreach (Edge edge in Constraints.Select(c => c.ControlEdge))
			{
				switch (edge)
				{
					case Edge.Left:
					case Edge.Right:
						return true;
				}
			}
			return false;
		}

		private bool HasVerticalEdge()
		{
			foreach (Edge edge in Constraints.Select(c => c.ControlEdge))
			{
				switch (edge)
				{
					case Edge.Top:
					case Edge.Bottom:
						return true;
				}
			}
			return false;
		}

		public bool AnchoredOpposite(Edge edge)
		{
			switch (edge)
			{
				case Edge.Left:
					return ContainsEdge(Edge.Right);
				case Edge.Right:
					return ContainsEdge(Edge.Left);
				case Edge.Top:
					return ContainsEdge(Edge.Bottom);
				case Edge.Bottom:
					return ContainsEdge(Edge.Top);
			}
			return false;
		}

		private bool ContainsEdge(Edge edge)
		{
			return Constraints.Select(c => c.ControlEdge).Contains(edge);
		}

		/// <summary>
		/// Returns the opposite of a given edge.
		/// </summary>
		/// <param name="edge">Edge to get opposite edge from.</param>
		private Edge GetOppositeEdge(Edge edge)
		{
			switch (edge)
			{
				case Edge.Left:
					return Edge.Right;
				case Edge.Right:
					return Edge.Left;
				case Edge.Top:
					return Edge.Bottom;
				case Edge.Bottom:
					return Edge.Top;
			}
			return Edge.None;
		}

		private bool OnSameAxis(Edge edgeA, Edge edgeB)
		{
			if (edgeA == edgeB || GetOppositeEdge(edgeA) == edgeB)
			{
				return true;
			}

			switch (edgeA)
			{
				case Edge.Left:
				case Edge.Right:
					return edgeB == Edge.CenterX;

				case Edge.Top:
				case Edge.Bottom:
					return edgeB == Edge.CenterY;

				case Edge.CenterX:
					return edgeB == Edge.Left || edgeB == Edge.Right;

				case Edge.CenterY:
					return edgeB == Edge.Top || edgeB == Edge.Bottom;
			}
			return false;
		}

		public float GetEdgeDistance(Edge edge)
		{
			return Constraints.Where(c => c.ControlEdge.ContainsFlag(edge)).Sum(c => c.Distance);
		}
	}
}