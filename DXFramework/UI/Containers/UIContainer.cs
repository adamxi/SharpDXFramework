using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.UI
{
	public abstract class UIContainer : UIControl
	{
		protected List<UIControl> controls;

		public UIContainer()
		{
			controls = new List<UIControl>();
		}

		public List<UIControl> Controls
		{
			get { return controls; }
			set { controls = value; }
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
				if( controls != null )
				{
					foreach( UIControl control in controls )
					{
						control.SuspendLayout = value;
					}
				}
			}
		}

		#region Child control methods
		public virtual bool AddChild( UIControl control )
		{
			if( control != null && !controls.Contains( control ) )
			{
				control.AssignParent( this );
				controls.Add( control );
				return true;
			}
			return false;
		}

		public virtual bool RemoveChild( UIControl control )
		{
			if( control != null && controls.Remove( control ) )
			{
				control.AssignParent( null );
				return true;
			}
			return false;
		}

		public virtual void ClearChildren()
		{
			foreach( UIControl control in controls )
			{
				RemoveChild( control );
			}
		}

		public virtual void ReorderChild( UIControl control, int zDepth )
		{
			if( control != null && controls.Contains( control ) )
			{
				int removeIndex = controls.IndexOf( control );
				controls.Insert( zDepth, control );
				controls.RemoveAt( removeIndex );
			}
		}

		public virtual void MoveChildToFront( UIControl control )
		{
			if( control != null && controls.Remove( control ) )
			{
				controls.Add( control );
			}
		}

		public virtual void MoveChildToBack( UIControl control )
		{
			if( control != null && controls.Remove( control ) )
			{
				controls.Insert( 0, control );
			}
		}

		public virtual void MoveChildBefore( UIControl control, UIControl relative )
		{
			if( control != null && relative != null && controls.Contains( control ) && controls.Contains( relative ) )
			{
				controls.Remove( control );
				controls.Insert( controls.IndexOf( relative ), control );
			}
		}

		public virtual void MoveChildAfter( UIControl control, UIControl relative )
		{
			if( control != null && relative != null && controls.Contains( control ) && controls.Contains( relative ) )
			{
				controls.Remove( control );
				controls.Insert( controls.IndexOf( relative ) + 1, control );
			}
		}
		#endregion

		public RectangleF CalcContentBounds(bool includeMargins = false)
		{
			if( HasChildren && Visible )
			{
				Vector2 topLeft = new Vector2( float.MaxValue );
				Vector2 bottomRight = new Vector2( float.MinValue );

				foreach( UIControl control in controls )
				{
					RectangleF rect = RectangleF.Empty;

					if( control is UIContainer )
					{
						rect = ( control as UIContainer ).CalcContentBounds();
					}
					else if( control.Visible )
					{
						rect = control.Bounds;
						if (includeMargins)
						{
							rect.Left -= Constrainer.GetEdgeDistance(Edge.Left);
							rect.Right += Constrainer.GetEdgeDistance(Edge.Right);
							rect.Top -= Constrainer.GetEdgeDistance(Edge.Top);
							rect.Bottom += Constrainer.GetEdgeDistance(Edge.Bottom);
						}
					}

					topLeft.X = Math.Min( topLeft.X, rect.Left );
					topLeft.Y = Math.Min( topLeft.Y, rect.Top );
					bottomRight.X = Math.Max( bottomRight.X, rect.Right );
					bottomRight.Y = Math.Max( bottomRight.Y, rect.Bottom );
				}

				return new RectangleF( topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y );
			}
			return RectangleF.Empty;
		}

		public override void DoLayout( ConstraintCategory category = ConstraintCategory.All )
		{
			base.DoLayout( category );
			foreach( UIControl control in controls )
			{
				if( control.Enabled && control.Visible )
				{
					control.DoLayout( category );
				}
			}
		}

		public override void Update( GameTime gameTime )
		{
			for( int i = controls.Count; --i >= 0; ) // Reverse loop. Start with top-most drawn control (last in collection), and work its way down.
			{
				UIControl control = controls[ i ];
				if( control.Enabled && control.Visible )
				{
					control.Update( gameTime );
				}
			}
			base.Update( gameTime );
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
			base.Draw( spriteBatch );
			foreach( UIControl control in controls )
			{
				if( control.Enabled && control.Visible )
				{
					control.Draw( spriteBatch );
				}
			}
		}
	}
}