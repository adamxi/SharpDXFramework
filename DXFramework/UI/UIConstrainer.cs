using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace DXFramework.UI
{
	public class UIConstraint
	{
		public UIConstraint(Edge controlEdge, UIControl anchor, Edge anchorEdge, float distance, ConstraintCategory constraintCategory)
		{
			this.ControlEdge = controlEdge;
			this.Anchor = anchor;
			this.AnchorEdge = anchorEdge;
			this.Distance = distance;
			this.Category = constraintCategory;
		}

		public Edge ControlEdge { get; }
		public UIControl Anchor { get; }
		public Edge AnchorEdge { get; }
		public float Distance { get; }
		public ConstraintCategory Category { get; }
	}

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

		/// <summary>
		/// Adds a UI constraint to this control.
		/// </summary>
		/// <param name="controlEdge">Control edge to constraint.</param>
		/// <param name="anchor">Anchoring control on which the control edge is constrained. Setting this to 'null' the control is anchored relative to the screen.</param>
		/// <param name="anchorEdge">Anchor edge, on which the control edge is constrained relative to.</param>
		/// <param name="edgeDistance">Distance between control and anchor constraint edge.</param>
		internal void AddConstraint(Edge controlEdge, UIControl anchor, Edge anchorEdge, float edgeDistance, ConstraintCategory category)
		{
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

				if (!OnSameAxis(cEdge, anchorEdges[i]))
				{
					throw new ArgumentException($"Control and anchor edge and must be on the same axis: {cEdge}, {anchorEdges[i]}");
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

				Constraints.Add(new UIConstraint(cEdge, anchor, anchorEdges[i], edgeDistance, category));
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

		public bool ControlAnchoredOpposite(Edge edge)
		{
			switch (edge)
			{
				case Edge.Left:
					return ControlContainsEdge(Edge.Right);
				case Edge.Right:
					return ControlContainsEdge(Edge.Left);
				case Edge.Top:
					return ControlContainsEdge(Edge.Bottom);
				case Edge.Bottom:
					return ControlContainsEdge(Edge.Top);
			}
			return false;
		}

		private bool ControlContainsEdge(Edge edge)
		{
			return Constraints.Select(c => c.ControlEdge).Contains(edge);
		}

		/// <summary>
		/// Returns the opposite of a given edge.
		/// </summary>
		/// <param name="edge">Edge to get opposite edge from.</param>
		private Edge GetOpposite(Edge edge)
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
			if (edgeA == edgeB || GetOpposite(edgeA) == edgeB)
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

	public static class UIConstrainerEx
	{
		/// <summary>
		/// Adds a UI constraint to this control.
		/// </summary>
		/// <param name="control">Control instance.</param>
		/// <param name="controlEdge">Control edge to constraint.</param>
		/// <param name="anchor">Anchoring control on which the control edge is constrained. If 'null', the control is anchored relative to the viewport.</param>
		/// <param name="anchorEdge">Anchor edge on which the control edge is constrained relative to.</param>
		/// <param name="category">The category this constaint belongs to.</param>
		public static void AddConstraint(this UIControl control, Edge controlEdge, UIControl anchor, Edge anchorEdge, ConstraintCategory category)
		{
			control.AddConstraint(controlEdge, anchor, anchorEdge, 0f, category);
		}

		/// <summary>
		/// Adds a UI constraint to this control.
		/// </summary>
		/// <param name="control">Control instance.</param>
		/// <param name="controlEdge">Control edge to constraint.</param>
		/// <param name="anchor">Anchoring control on which the control edge is constrained. If 'null', the control is anchored relative to the viewport.</param>
		/// <param name="anchorEdge">Anchor edge on which the control edge is constrained relative to.</param>
		/// <param name="edgeDistance">Distance between control and anchor constraint edges.</param>
		/// <param name="category">The category this constaint belongs to.</param>
		public static void AddConstraint(this UIControl control, Edge controlEdge, UIControl anchor, Edge anchorEdge, float edgeDistance = 0, ConstraintCategory category = ConstraintCategory.All)
		{
            if (control == anchor)
			{
				throw new ArgumentException("Cannot anchor control to itself.");
			}

			List<Edge> controlEdges = new List<Edge>(4);
			List<Edge> anchorEdges = new List<Edge>(4);
			foreach (Edge edgeType in UIConstrainer.EdgeTypes)
			{
				if (controlEdge.ContainsFlag(edgeType))
				{
					controlEdges.Add(edgeType);
				}
				if (anchorEdge.ContainsFlag(edgeType))
				{
					anchorEdges.Add(edgeType);
				}
			}

			if (controlEdges.Count != anchorEdges.Count)
			{
				throw new ArgumentException("There must be an equal amount of control and anchor edges.");
			}
			if (control.Constrainer == null)
			{
				control.Constrainer = new UIConstrainer();
			}

			for (int i = 0; i < controlEdges.Count; i++)
			{
				control.Constrainer.AddConstraint(controlEdges[i], anchor, anchorEdges[i], edgeDistance, category);
			}
		}

		internal static void EnforceConstraints(this UIControl control, ConstraintCategory category = ConstraintCategory.All)
		{
			if (control.SuspendLayout)
			{
				return;
			}

			UIManager.layoutProfiler.Start();
			UIConstrainer constrainer = control.Constrainer;
			if (constrainer != null)
			{
				foreach (UIConstraint constraint in constrainer.Constraints)
				{
					// Check whether the constaint should be enforced.
					if (category != ConstraintCategory.All && !category.ContainsFlag(constraint.Category))
					{
						continue;
					}

					Edge controlEdge = constraint.ControlEdge;
					UIControl anchor = constraint.Anchor;
					float distance = constraint.Distance;

					Vector2 pos = Vector2.Zero;
					Vector2 anchorSize = Engine.ScreenSize;

					if (anchor != null)
					{
						anchorSize = anchor.Size * anchor.scale;
						if (anchor.Parent == control.Parent)    // Is anchor and control children of the same parent? 
						{
							pos = anchor.location;              // ..then set control position relative to the anchor.
						}

						Vector2 scaleDelta = anchor.ScaledSize - anchor.Size;
						pos -= anchor.NormalizedOrigin * scaleDelta;
					}

					Vector2 halfAnchorSize = anchorSize * 0.5f;

					switch (constraint.AnchorEdge)
					{
						case Edge.Left:
							pos.X += distance;
							break;
						case Edge.Top:
							pos.Y += distance;
							break;
						case Edge.Right:
							pos.X += anchorSize.X - distance;
							break;
						case Edge.Bottom:
							pos.Y += anchorSize.Y - distance;
							break;
						case Edge.CenterX:
							pos.X += halfAnchorSize.X + distance;
							break;
						case Edge.CenterY:
							pos.Y += halfAnchorSize.Y + distance;
							break;
					}
					if (!constrainer.ControlAnchoredOpposite(controlEdge))
					{
						switch (controlEdge)
						{
							case Edge.Left:
								control.location.X = pos.X;
								break;
							case Edge.Top:
								control.location.Y = pos.Y;
								break;
							case Edge.Right:
								control.location.X = pos.X - control.Width;
								break;
							case Edge.Bottom:
								control.location.Y = pos.Y - control.Height;
								break;
							case Edge.CenterX:
								control.location.X = pos.X - control.HalfWidth;
								break;
							case Edge.CenterY:
								control.location.Y = pos.Y - control.HalfHeight;
								break;
						}
					}
					else
					{
						switch (controlEdge)
						{
							case Edge.Left:
							case Edge.CenterX:
								control.size.X += control.location.X - pos.X;
								control.location.X = pos.X;
								break;
							case Edge.Top:
							case Edge.CenterY:
								control.size.Y += control.location.Y - pos.Y;
								control.location.Y = pos.Y;
								break;
							case Edge.Right:
								control.size.X += (pos.X - control.Width) - control.location.X;
								//control.location.X += pos.X - control.size.X;
								break;
							case Edge.Bottom:
								control.size.Y += (pos.Y - control.Height) - control.location.Y;
								//control.location.Y += pos.Y - control.size.Y;
								break;
						}
					}
				}
			}

			_UpdateDrawLocation(control);
			UIManager.layoutProfiler.Stop();
		}

		internal static void UpdateDrawLocation(this UIControl control)
		{
			if (control.SuspendLayout)
			{
				return;
			}

			_UpdateDrawLocation(control);

			if (control is UIContainer)
			{
				UIContainer container = control as UIContainer;
				foreach (UIControl child in container.Controls)
				{
					UpdateDrawLocation(child);
				}
			}
			if (control.HasDecorations)
			{
				control.Decorations.ForEach(c => UpdateDrawLocation(c));
			}
		}

		private static void _UpdateDrawLocation(UIControl control)
		{
			if (control.HasParent)
			{
				control.DrawPosition = control.Parent.DrawPosition + control.Location;
			}
			else
			{
				control.DrawPosition = control.Location;
			}
		}

		/// <summary>
		/// Returns true if a flag is contained within a given enum of flags.
		/// </summary>
		/// <param name="flag">Flag to check.</param>
		public static bool ContainsFlag(this Enum e, Enum flag)
		{
			return (Convert.ToInt64(e) & Convert.ToInt64(flag)) != 0;
		}
	}

	[Flags]
	public enum Edge
	{
		None = 0,
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
		CenterX = 16,
		CenterY = 32,
		TopLeft = Top | Left,
		TopCenter = Top | CenterX,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomCenter = Bottom | CenterX,
		BottomRight = Bottom | Right,
		LeftCenter = Left | CenterY,
		RightCenter = Right | CenterY,
		CenterXY = CenterX | CenterY,
		Horizontal = Left | Right,
		Vertical = Top | Bottom,
		Dock = Left | Right | Top | Bottom
	}

	[Flags]
	public enum ConstraintCategory
	{
		/// <summary>
		/// This constraint is by default enforced during layout updates.
		/// </summary>
		Update = 1,

		/// <summary>
		/// This constraint is by default enforced during layout initialization.
		/// </summary>
		Initialization = 2,

		/// <summary>
		/// This constraint is by default enforced on all layout events.
		/// </summary>
		All = Update | Initialization
	}

	public enum ScrollRestriction
	{
		/// <summary>
		/// Restrict scrolling to the horizontal axis.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Restrict scrolling to the vertical axis.
		/// </summary>
		Vertical = 1,

		/// <summary>
		/// Unrestricted scrolling on both horizontal and vertical axis.
		/// </summary>
		Unrestricted = 2
	}
}