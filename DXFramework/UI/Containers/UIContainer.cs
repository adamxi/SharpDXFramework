using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;

namespace DXFramework.UI
{
	public abstract class UIContainer : UIControl
	{
		protected List<UIControl> controls;

		public UIContainer()
		{
			controls = new List<UIControl>(4);
		}

		public List<UIControl> Controls
		{
			get { return controls; }
		}

		public bool HasChildren
		{
			get { return controls.Count > 0; }
		}

		public override bool SuspendLayout
		{
			get { return base.SuspendLayout; }
			set
			{
				base.SuspendLayout = value;
				foreach (UIControl control in controls)
				{
					control.SuspendLayout = value;
				}
			}
		}

		public override Vector2 NormalizedOrigin
		{
			get { return base.NormalizedOrigin; }
			set
			{
				base.NormalizedOrigin = value;
				foreach (UIControl control in controls)
				{
					control.NormalizedOrigin = value;
				}
			}
		}

		#region Child control methods
		public virtual bool AddChild(UIControl control)
		{
			if (control != null && !controls.Contains(control))
			{
				control.AssignParent(this);
				controls.Add(control);
				return true;
			}
			return false;
		}

		public virtual bool RemoveChild(UIControl control)
		{
			if (control != null && controls.Remove(control))
			{
				control.AssignParent(null);
				return true;
			}
			return false;
		}

		public virtual void ClearChildren()
		{
			foreach (UIControl control in controls)
			{
				RemoveChild(control);
			}
		}

		public virtual void ReorderChild(UIControl control, int zDepth)
		{
			if (control != null && controls.Contains(control))
			{
				int removeIndex = controls.IndexOf(control);
				controls.Insert(zDepth, control);
				controls.RemoveAt(removeIndex);
			}
		}

		public virtual void MoveChildToFront(UIControl control)
		{
			if (control != null && controls.Remove(control))
			{
				controls.Add(control);
			}
		}

		public virtual void MoveChildToBack(UIControl control)
		{
			if (control != null && controls.Remove(control))
			{
				controls.Insert(0, control);
			}
		}

		public virtual void MoveChildBefore(UIControl control, UIControl relative)
		{
			if (control != null && relative != null && controls.Contains(control) && controls.Contains(relative))
			{
				controls.Remove(control);
				controls.Insert(controls.IndexOf(relative), control);
			}
		}

		public virtual void MoveChildAfter(UIControl control, UIControl relative)
		{
			if (control != null && relative != null && controls.Contains(control) && controls.Contains(relative))
			{
				controls.Remove(control);
				controls.Insert(controls.IndexOf(relative) + 1, control);
			}
		}
		#endregion

		public RectangleF CalcContentBounds(bool includeMargins = false)
		{
			if (HasChildren && Visible)
			{
				Vector2 topLeft = new Vector2(float.MaxValue);
				Vector2 bottomRight = new Vector2(float.MinValue);

				foreach (UIControl control in controls)
				{
					RectangleF rect = RectangleF.Empty;

					if (control is UIContainer)
					{
						rect = (control as UIContainer).CalcContentBounds(includeMargins);
						rect.Location = control.position;
						if (includeMargins)
						{
							rect.Left -= control?.Constrainer?.GetEdgeDistance(Edge.Left) ?? 0;
							rect.Right += control?.Constrainer?.GetEdgeDistance(Edge.Right) ?? 0;
							rect.Top -= control?.Constrainer?.GetEdgeDistance(Edge.Top) ?? 0;
							rect.Bottom += control?.Constrainer?.GetEdgeDistance(Edge.Bottom) ?? 0;
						}
					}
					else if (control.Visible)
					{
						rect = control.SourceRect;
						rect.Location = control.position;
						if (includeMargins)
						{
							rect.Left -= control?.Constrainer?.GetEdgeDistance(Edge.Left) ?? 0;
							rect.Right += control?.Constrainer?.GetEdgeDistance(Edge.Right) ?? 0;
							rect.Top -= control?.Constrainer?.GetEdgeDistance(Edge.Top) ?? 0;
							rect.Bottom += control?.Constrainer?.GetEdgeDistance(Edge.Bottom) ?? 0;
						}
					}

					topLeft.X = Math.Min(topLeft.X, rect.Left);
					topLeft.Y = Math.Min(topLeft.Y, rect.Top);
					bottomRight.X = Math.Max(bottomRight.X, rect.Right);
					bottomRight.Y = Math.Max(bottomRight.Y, rect.Bottom);
				}

				return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
			}
			return RectangleF.Empty;
		}

		public override void DoLayoutOnChildren(ConstraintCategory category = ConstraintCategory.All)
		{
			base.DoLayoutOnChildren(category);
			controls.ForEach(c => c.DoLayout(category));
		}

		public override void Update(GameTime gameTime)
		{
			for (int i = controls.Count; --i >= 0;) // Reverse loop. Start with top-most drawn control (last in collection), and work its way down.
			{
				UIControl control = controls[i];
				if (control.Enabled && control.Visible)
				{
					control.Update(gameTime);
				}
			}
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			foreach (UIControl control in controls)
			{
				if (control.Enabled && control.Visible)
				{
					control.Draw(spriteBatch);
				}
			}
		}
	}
}