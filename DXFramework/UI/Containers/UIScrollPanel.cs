using System;
using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;

namespace DXFramework.UI
{
	public class UIScrollPanel : UIPanel
	{
		private EventHandler<EventArgs> onContentScrolled;
		public UIPanel contentPanel;
		private Accelerator2D accel;

		public UIScrollPanel()
		{
			this.AutoSize = false;
			this.Restriction = ScrollRestriction.Unrestricted;
			this.accel = new Accelerator2D();
			this.ClipContent = true;
			this.ClampContent = true;
			this.InputDown += UIScrollPanel_MouseHeld;
			this.InputPressed += UIScrollPanel_MousePressed;
			this.InputReleasedAnywhere += UIScrollPanel_MouseReleasedAnywhere;

			contentPanel = new UIPanel();
			contentPanel.AbsorbPointer = false;
			contentPanel.AutoSize = true;
			contentPanel.DrawBounds = false;

			contentPanel.AddConstraint( Edge.TopLeft, this, Edge.TopLeft );

			base.AddChild( contentPanel );
		}

		public bool ClampContent { get; set; }

		public override Vector2 Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		void UIScrollPanel_MouseHeld( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButton.Left )
			{
				accel.AddSample( InputManager.MouseDelta );
			}
			else
			{
				e.Absorbed = false;
			}
		}

		void UIScrollPanel_MousePressed( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButton.Left )
			{
				accel.Begin();
			}
			else
			{
				e.Absorbed = false;
			}
		}

		void UIScrollPanel_MouseReleasedAnywhere( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButton.Left )
			{
				accel.End();
			}
		}

		public event EventHandler<EventArgs> ContentScrolled
		{
			add { onContentScrolled += value.MakeWeak( e => onContentScrolled -= e ); }
			remove { onContentScrolled -= onContentScrolled.Unregister( value ); }
		}

		public ScrollRestriction Restriction { get; set; }

		public UIPanel ContentPanel
		{
			get { return contentPanel; }
		}

		public override bool AddChild( UIControl control )
		{
			bool success = contentPanel.AddChild( control );
			if( success && AutoSize )
			{
				ResizeToContent();
			}
			return success;
		}

		public override bool RemoveChild( UIControl control )
		{
			bool added = contentPanel.RemoveChild( control );
			if( added && AutoSize )
			{
				ResizeToContent();
			}
			return added;
		}

		public override void ClearChildren()
		{
			contentPanel.ClearChildren();
			if( AutoSize )
			{
				ResizeToContent();
			}
		}

		public void StopMovement()
		{
			accel.Stop();
		}

		private void OnContentScrolled()
		{
			if( onContentScrolled != null )
			{
				onContentScrolled.Invoke( this, null );
			}
		}

		public void ScrollToLocation( Vector2 location )
		{
			if( ClampContent )
			{
				contentPanel.Location = new Vector2( GetClampedX( location.X ), GetClampedY( location.Y ) );
			}
			else
			{
				contentPanel.Location = location;
			}
		}

		private float GetClampedX( float x )
		{
			if( contentPanel.size.X < size.X )
			{
				return MathUtil.Clamp( x, 0, size.X - contentPanel.size.X );
			}
			else
			{
				return MathUtil.Clamp( x, size.X - contentPanel.size.X, 0 );
			}
		}

		private float GetClampedY( float y )
		{
			if( contentPanel.size.Y < size.Y )
			{
				return MathUtil.Clamp( y, 0, size.Y - contentPanel.size.Y );
			}
			else
			{
				return MathUtil.Clamp( y, size.Y - contentPanel.size.Y, 0 );
			}
		}

		private void SetContentX( float x )
		{
			if( contentPanel.X != x )
			{
				contentPanel.X = x;
				OnContentScrolled();
			}
		}

		private void SetContentY( float y )
		{
			if( contentPanel.Y != y )
			{
				contentPanel.Y = y;
				OnContentScrolled();
			}
		}

		private void SetContentPos( Vector2 pos )
		{
			if( contentPanel.location != pos )
			{
				contentPanel.Location = pos;
				OnContentScrolled();
			}
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );
			accel.Update();

			switch( Restriction )
			{
				case ScrollRestriction.Horizontal:
					if( ClampContent )
					{
						SetContentX( GetClampedX( contentPanel.X + accel.Speed.X ) );
					}
					else
					{
						SetContentX( contentPanel.X + accel.Speed.X );
					}
					break;

				case ScrollRestriction.Vertical:
					if( ClampContent )
					{
						SetContentY( GetClampedY( contentPanel.Y + accel.Speed.Y ) );
					}
					else
					{
						SetContentY( contentPanel.Y + accel.Speed.Y );
					}
					break;

				case ScrollRestriction.Unrestricted:
					if( ClampContent )
					{
						Vector2 pos = Vector2.Zero;
						pos.X = GetClampedX( contentPanel.X + accel.Speed.X );
						pos.Y = GetClampedY( contentPanel.Y + accel.Speed.Y );
						SetContentPos( pos );
					}
					else
					{
						SetContentPos( contentPanel.Location + accel.Speed );
					}
					break;
			}
		}
	}
}