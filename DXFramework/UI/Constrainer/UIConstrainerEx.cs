using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DXFramework.UI
{
	public static class UIConstrainerEx
	{
		/// <summary>
		/// Adds a UI constraint to this control.
		/// </summary>
		/// <param name="control">Control instance.</param>
		/// <param name="controlEdge">Control edge to constraint.</param>
		/// <param name="anchor">Anchoring control on which the control edge is constrained. If 'null', the control is anchored relative to the viewport.</param>
		/// <param name="anchorEdge">Anchor edge on which the control edge is constrained relative to.</param>
		/// <param name="category">The category this constraint belongs to.</param>
		public static void AddConstraint(this UIControl control, Edge controlEdge, UIControl anchor, Edge anchorEdge, ConstraintCategory category)
		{
			control.AddConstraint(controlEdge, anchor, anchorEdge, 0f, category);
		}

		/// <summary>
		/// Overload added to prevent implicit casting to ConstraintCategory when edgeDistance is set to 0.
		/// http://geekswithblogs.net/BlackRabbitCoder/archive/2012/01/26/c.net-little-pitfalls-implicit-zero-to-enum-conversion.aspx
		/// </summary>
		public static void AddConstraint(this UIControl control, Edge controlEdge, UIControl anchor, Edge anchorEdge, int edgeDistance)
		{
			control.AddConstraint(controlEdge, anchor, anchorEdge, edgeDistance, ConstraintCategory.All);
		}

		/// <summary>
		/// Adds a UI constraint to this control.
		/// </summary>
		/// <param name="control">Control instance.</param>
		/// <param name="controlEdge">Control edge to constraint.</param>
		/// <param name="anchor">Anchoring control on which the control edge is constrained. If 'null', the control is anchored relative to the viewport.</param>
		/// <param name="anchorEdge">Anchor edge on which the control edge is constrained relative to.</param>
		/// <param name="edgeDistance">Distance between control and anchor constraint edges.</param>
		/// <param name="category">The category this constraint belongs to.</param>
		public static void AddConstraint(this UIControl control, Edge controlEdge, UIControl anchor, Edge anchorEdge, float edgeDistance = 0, ConstraintCategory category = ConstraintCategory.All)
		{
			if (control == anchor)
			{
				throw new ArgumentException("Cannot anchor control to itself.");
			}

			//// Separate edges into their basic flags.
			//// E.g. 'Edge.BottomRight' is separated into 'Edge.Bottom' and 'Edge.Right', and added as individual edges.
			//List<Edge> controlEdges = new List<Edge>(4);
			//List<Edge> anchorEdges = new List<Edge>(4);
			//foreach (Edge edgeType in UIConstrainer.EdgeTypes)
			//{
			//	if (controlEdge.ContainsFlag(edgeType))
			//	{
			//		controlEdges.Add(edgeType);
			//	}
			//	if (anchorEdge.ContainsFlag(edgeType))
			//	{
			//		anchorEdges.Add(edgeType);
			//	}
			//}

			//if (controlEdges.Count != anchorEdges.Count)
			//{
			//	throw new ArgumentException("There must be an equal amount of control and anchor edges.");
			//}
			if (control.Constrainer == null)
			{
				control.Constrainer = new UIConstrainer();
			}
			control.Constrainer.AddConstraint(controlEdge, anchor, anchorEdge, edgeDistance, category);

			//for (int i = 0; i < controlEdges.Count; i++)
			//{
			//	control.Constrainer.AddConstraint(controlEdges[i], anchor, anchorEdges[i], edgeDistance, category);
			//}
		}

		/// <summary>
		/// Single-use constraint enforcement.
		/// </summary>
		public static void EnforceConstraint(this UIControl control, UIConstraint constraint)
		{
			var constrainer = new UIConstrainer();
			constrainer.AddConstraint(constraint);

			EnforceConstraints(control, constrainer);
		}

		internal static void EnforceConstraints(this UIControl control, ConstraintCategory category = ConstraintCategory.All)
		{
			if (control.SuspendLayout)
			{
				return;
			}

			UIManager.layoutProfiler.Start();
			EnforceConstraints(control, control.Constrainer, category);
			UIManager.layoutProfiler.Stop();
		}

		private static void EnforceConstraints(this UIControl control, UIConstrainer constrainer, ConstraintCategory category = ConstraintCategory.All)
		{
			if (constrainer != null)
			{
				foreach (UIConstraint constraint in constrainer.Constraints)
				{
					// Check whether the constraint should be enforced.
					if (category != ConstraintCategory.All && !category.ContainsFlag(constraint.Category))
					{
						continue;
					}

					Edge controlEdge = constraint.ControlEdge;
					UIControl anchor = constraint.Anchor;
					float distance = constraint.Distance;
					bool anchorVisible = true;

					Vector2 pos = Vector2.Zero;
					Vector2 anchorSize = Engine.ScreenSize;

					if (anchor != null)
					{
						anchorVisible = anchor.Visible;
						anchorSize = anchorVisible ? (anchor.Size * anchor.scale).Max(anchor.MinimumSize * anchor.scale) : Vector2.Zero;
						if (anchor.Parent == control.Parent)    // Is anchor and control children of the same parent? 
						{
							pos = anchor.position;              // ..then set control position relative to the anchor.
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
					if (!constrainer.AnchoredOpposite(controlEdge))
					{
						switch (controlEdge)
						{
							case Edge.Left:
								control.position.X = pos.X;
								break;
							case Edge.Top:
								control.position.Y = pos.Y;
								break;
							case Edge.Right:
								control.position.X = pos.X - Math.Max(control.Width, control.MinimumSize.X);
								break;
							case Edge.Bottom:
								control.position.Y = pos.Y - Math.Max(control.Height, control.MinimumSize.Y);
								break;
							case Edge.CenterX:
								control.position.X = pos.X - Math.Max(control.HalfWidth, control.MinimumSize.X * 0.5f);
								break;
							case Edge.CenterY:
								control.position.Y = pos.Y - Math.Max(control.HalfHeight, control.MinimumSize.Y * 0.5f);
								break;
						}
					}
					else
					{
						switch (controlEdge)
						{
							case Edge.Left:
							case Edge.CenterX:
								control.size.X += control.position.X - pos.X;
								control.position.X = pos.X;
								break;
							case Edge.Top:
							case Edge.CenterY:
								control.size.Y += control.position.Y - pos.Y;
								control.position.Y = pos.Y;
								break;
							case Edge.Right:
								control.size.X += (pos.X - control.Width) - control.position.X;
								if (!anchorVisible)
								{
									control.size.X += anchor.size.X; // TODO: Needs proper testing
								}
								//control.location.X += pos.X - control.size.X;
								break;
							case Edge.Bottom:
								control.size.Y += (pos.Y - control.Height) - control.position.Y;
								if (!anchorVisible)
								{
									control.size.Y += anchor.size.Y; // TODO: Needs proper testing
								}
								//control.location.Y += pos.Y - control.size.Y;
								break;
						}
						control.UpdateTransformation = true; // Control transformation needs a recalculation when its size is updated.
					}
				}
			}

			if (control.MinimumSize != Vector2.Zero)
			{
				control.size = control.size.Max(control.MinimumSize);
				control.UpdateTransformation = true;
			}

			_UpdateDrawPosition(control);
		}

		internal static void UpdateDrawPosition(this UIControl control)
		{
			if (control.SuspendLayout)
			{
				return;
			}

			_UpdateDrawPosition(control);

			if (control is UIContainer)
			{
				UIContainer container = control as UIContainer;
				foreach (UIControl child in container.Controls)
				{
					UpdateDrawPosition(child);
				}
			}
			if (control.HasDecorations)
			{
				control.Decorations.ForEach(c => UpdateDrawPosition(c));
			}
		}

		private static void _UpdateDrawPosition(UIControl control)
		{
			if (control.HasParent)
			{
				control.DrawPosition = control.Parent.DrawPosition + control.Position;
			}
			else
			{
				control.DrawPosition = control.Position;
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
}