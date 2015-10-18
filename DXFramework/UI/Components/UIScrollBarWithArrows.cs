using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.UI
{
	public class UIScrollBarWithArrows : UIPanel
	{
		private EventHandler<EventArgs> onValueChanged;
		private UIImage sliderBackground;
		private UIImage slider;
		private UIImage arrowUp;
		private UIImage arrowDown;
		private float scrollStep;

		public UIScrollBarWithArrows()
		{
			this.AutoSize = false;
			this.scrollStep = 0.02f;

			sliderBackground = new UIImage( "graphics/slider_back" );
			sliderBackground.InputMoved += sliderBackground_MouseMoved;
			sliderBackground.InputPressed += sliderBackground_MousePressed;

			slider = new UIImage( "graphics/slider_handle" );

			arrowUp = new UIImage( "graphics/arrow_down" );
			arrowUp.SpriteEffect = SpriteEffects.FlipVertically;
			arrowUp.InputDown += arrowUp_MouseHeld;

			arrowDown = new UIImage( "graphics/arrow_down" );
			arrowDown.InputDown += arrowDown_MouseHeld;

			arrowUp.AddConstraint( Edge.Horizontal | Edge.Top, this, Edge.Horizontal | Edge.Top );
			arrowDown.AddConstraint( Edge.Horizontal | Edge.Bottom, this, Edge.Horizontal | Edge.Bottom );
			sliderBackground.AddConstraint( Edge.Horizontal, this, Edge.Horizontal );
			sliderBackground.AddConstraint( Edge.Top, arrowUp, Edge.Bottom );
			sliderBackground.AddConstraint( Edge.Bottom, arrowDown, Edge.Top );
			slider.AddConstraint( Edge.TopRight, sliderBackground, Edge.TopRight );

			AddChild( arrowUp );
			AddChild( arrowDown );
			AddChild( sliderBackground );
			AddChild( slider );
		}

		void sliderBackground_MousePressed( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButton.Left )
			{
				SetValue( ( e.ClientPosition.Y - slider.HalfHeight ) / SliderRange );
			}
		}

		void sliderBackground_MouseMoved( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButton.Left )
			{
				SetValue( ( e.ClientPosition.Y - slider.HalfHeight ) / SliderRange );
			}
		}

		void arrowUp_MouseHeld( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButton.Left )
			{
				IncrementValue( -scrollStep );
			}
		}

		void arrowDown_MouseHeld( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButton.Left )
			{
				IncrementValue( scrollStep );
			}
		}

		public event EventHandler<EventArgs> ValueChanged
		{
			add { onValueChanged += value.MakeWeak( e => onValueChanged -= e ); }
			remove { onValueChanged -= onValueChanged.Unregister( value ); }
		}

		public float Value { get; private set; }

		private float SliderRange
		{
			get { return sliderBackground.Height - slider.Height; }
		}

		public override void DoLayout( ConstraintCategory category = ConstraintCategory.All )
		{
			arrowUp.Size = new Vector2( Width );
			arrowDown.Size = new Vector2( Width );
			slider.Size = new Vector2( Width, 10 );
			base.DoLayout( category );
		}

		public void IncrementValue( float increment )
		{
			SetValue( Value + increment );
		}

		public void SetValue( float value, bool surppressEvent = false )
		{
			float newValue = MathUtil.Clamp( value, 0, 1 );
			if( Value != newValue )
			{
				Value = newValue;
				slider.Location = new Vector2( slider.Location.X, Value * SliderRange + sliderBackground.Location.Y );

				if( !surppressEvent && onValueChanged != null )
				{
					onValueChanged.Invoke( this, null );
				}
			}
		}
	}
}