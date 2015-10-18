using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.UI
{
	[DataContract]
	public abstract class UIControl
	{
		protected const float INV_255 = 1f / 255f;
		internal Vector2 location;
		protected Color color;
		private UIControl parent;
		private List<UIControl> decorations;
		private SpriteEffects spriteEffect;
		internal Vector2 size;
		private bool suspendLayout;
		protected bool layoutDone;
		protected bool initialized;

		public UIControl()
		{
			Enabled = true;
			Visible = true;
			Scale = Vector2.One;
			Alpha = 1f;
			Color = Color.White;
			Margin = new Vector2( 0 );
			Anchor = AnchorStyle.TopLeft;
			suspendLayout = true;
			initialized = false;
		}

		#region Properties
		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public bool Visible { get; set; }

		public bool HasDecorations { get; private set; }

		public bool HasParent { get; private set; }

		[DataMember]
		public bool DrawBounds { get; set; }

		[DataMember]
		public bool ClipContent { get; set; }

		[DataMember]
		public virtual bool SuspendLayout
		{
			get { return suspendLayout; }
			set { suspendLayout = value; }
		}

		/// <summary>
		/// If true, the control will absorb pointer inputs.
		/// Will absorb inputs regardless of if the control has any subscribing pointer events.
		/// </summary>
		[DataMember]
		public bool AbsorbPointer { get; set; }

		[DataMember]
		public bool PointerPreview { get; set; }

		/// <summary>
		/// True, if the pointer has gone down inside the control region.
		/// </summary>
		protected bool PointerDown { get; private set; }

		internal UIControl Parent
		{
			get { return parent; }
		}

		public virtual Vector2 DrawPosition { get; internal set; }

		public virtual Vector2 Location
		{
			get { return location; }
			set
			{
				if( location != value )
				{
					location = value;
					UILayoutEngine.UpdateLocation( this );
				}
			}
		}

		/// <summary>
		/// Normalized [0..1].
		/// </summary>
		public virtual Vector2 Origin { get; set; }

		public virtual Vector2 AbsOrigin
		{
			get { return size * Origin; }
		}

		internal Vector2 oldSize;

		public virtual Vector2 Size
		{
			get { return size; }
			set
			{
				if( size != value )
				{
					oldSize = size;
					size = value;
					UILayoutEngine.UpdateSize( this, size - oldSize );
				}
			}
		}

		public virtual float Width
		{
			get { return size.X; }
			set { size.X = value; }
		}

		public virtual float Height
		{
			get { return size.Y; }
			set { size.Y = value; }
		}

		public float HalfWidth
		{
			get { return size.X * 0.5f; }
		}

		public float HalfHeight
		{
			get { return size.Y * 0.5f; }
		}

		public float X
		{
			get { return Left; }
			set { Left = value; }
		}

		public float Y
		{
			get { return Top; }
			set { Top = value; }
		}

		public float Left
		{
			get { return location.X; }
			set
			{
				if( location.X != value )
				{
					location.X = value;
					UILayoutEngine.UpdateLocation( this );
				}
			}
		}

		public float Top
		{
			get { return location.Y; }
			set
			{
				if( location.Y != value )
				{
					location.Y = value;
					UILayoutEngine.UpdateLocation( this );
				}
			}
		}

		public float Right
		{
			get { return location.X + Size.X; }
		}

		public float Bottom
		{
			get { return location.Y + Size.Y; }
		}

		public float CenterX
		{
			get { return Left + HalfWidth; }
		}

		public float CenterY
		{
			get { return Top + HalfHeight; }
		}

		public Vector2 DrawCenter
		{
			get { return DrawPosition + size * 0.5f; }
		}

		public Vector2 Center
		{
			get { return location + size * 0.5f; }
		}

		public virtual RectangleF BoundsF
		{
			get { return new RectangleF( DrawPosition.X, DrawPosition.Y, Size.X, Size.Y ); }
			set
			{
				location = HasParent ? DrawPosition - value.Location : value.Location;
				Size = new Vector2( value.Width, value.Height );
			}
		}

		public virtual Rectangle Bounds
		{
			get { return new Rectangle( (int)DrawPosition.X, (int)DrawPosition.Y, (int)Size.X, (int)Size.Y ); }
			set
			{
				location = HasParent ? DrawPosition - value.Location : value.Location;
				Size = new Vector2( value.Width, value.Height );
			}
		}

		public Vector2 Scale { get; set; }

		/// <summary>
		/// Normalized Alpha [0..1]
		/// </summary>
		[DataMember]
		public float Alpha
		{
			get { return color.A * INV_255; }
			set { color.A = (byte)( value * 255 ); }
		}

		[DataMember]
		public Color Color
		{
			get { return color; }
			set
			{
				if( color != value )
				{
					value.A = color.A;
					color = value;
				}
			}
		}

		public float Radians { get; set; }

		public Vector2 Margin { get; set; }

		public SpriteEffects SpriteEffect
		{
			get { return spriteEffect; }
			set { spriteEffect = value; }
		}

		public virtual AnchorStyle Anchor { get; set; }

		internal UIConstrainer Constrainer { get; set; }

		public object Tag { get; set; }

		internal List<UIControl> Decorations
		{
			get { return decorations; }
			set { decorations = value; }
		}
		#endregion

		#region Event handling
		private EventHandler<MouseEventArgs> onInputPressed;
		private EventHandler<MouseEventArgs> onInputHeld;
		private EventHandler<MouseEventArgs> onInputReleased;
		private EventHandler<MouseEventArgs> onInputMoved;
		private EventHandler<MouseEventArgs> onInputReleasedAnywhere;
		private EventHandler<MouseEventArgs> onInputEnter;
		private EventHandler<MouseEventArgs> onInputLeave;
		private EventHandler<MouseEventArgs> onInputHover;

		public event EventHandler<MouseEventArgs> InputPressed
		{
			add { onInputPressed += value.MakeWeak( e => onInputPressed -= e ); }
			remove { onInputPressed -= onInputPressed.Unregister( value ); }
		}

		public event EventHandler<MouseEventArgs> InputHeld
		{
			add { onInputHeld += value.MakeWeak( e => onInputHeld -= e ); }
			remove { onInputHeld -= onInputHeld.Unregister( value ); }
		}

		public event EventHandler<MouseEventArgs> InputReleased
		{
			add { onInputReleased += value.MakeWeak( e => onInputReleased -= e ); }
			remove { onInputReleased -= onInputReleased.Unregister( value ); }
		}

		public event EventHandler<MouseEventArgs> InputMoved
		{
			add { onInputMoved += value.MakeWeak( e => onInputMoved -= e ); }
			remove { onInputMoved -= onInputMoved.Unregister( value ); }
		}

		public event EventHandler<MouseEventArgs> InputReleasedAnywhere
		{
			add { onInputReleasedAnywhere += value.MakeWeak( e => onInputReleasedAnywhere -= e ); }
			remove { onInputReleasedAnywhere -= onInputReleasedAnywhere.Unregister( value ); }
		}

		public event EventHandler<MouseEventArgs> InputEnter
		{
			add { onInputEnter += value.MakeWeak( e => onInputEnter -= e ); }
			remove { onInputEnter -= onInputEnter.Unregister( value ); }
		}

		public event EventHandler<MouseEventArgs> InputLeave
		{
			add { onInputLeave += value.MakeWeak( e => onInputLeave -= e ); }
			remove { onInputLeave -= onInputLeave.Unregister( value ); }
		}

		public event EventHandler<MouseEventArgs> InputHover
		{
			add { onInputHover += value.MakeWeak( e => onInputHover -= e ); }
			remove { onInputHover -= onInputHover.Unregister( value ); }
		}

		//protected virtual bool CheckOnInputPressed()
		//{
		//	if( onInputPressed != null )
		//	{
		//		MouseEventArgs e = CreateMouseArgs( InputManager.MousePressed );
		//		onInputPressed.Invoke( this, e );
		//		return e.Absorbed;
		//	}
		//	return false;
		//}

		//protected virtual bool CheckOnInputHeld()
		//{
		//	if( onInputHeld != null )
		//	{
		//		MouseEventArgs e = CreateMouseArgs( InputManager.MouseDown );
		//		onInputHeld.Invoke( this, e );
		//		return e.Absorbed;
		//	}
		//	return false;
		//}

		//protected virtual bool CheckOnInputReleased()
		//{
		//	if( onInputReleased != null )
		//	{
		//		MouseEventArgs e = CreateMouseArgs( InputManager.MouseReleased );
		//		onInputReleased.Invoke( this, e );
		//		return e.Absorbed;
		//	}
		//	return false;
		//}

		//protected virtual bool CheckOnInputMoved()
		//{
		//	if( onInputMoved != null )
		//	{
		//		MouseEventArgs e = CreateMouseArgs( InputManager.MouseDown );
		//		onInputMoved.Invoke( this, e );
		//		return e.Absorbed;
		//	}
		//	return false;
		//}

		//protected virtual bool CheckOnInputReleasedAnywhere()
		//{
		//	if( onInputReleasedAnywhere != null )
		//	{
		//		MouseEventArgs e = CreateMouseArgs( InputManager.MouseReleased );
		//		onInputReleasedAnywhere.Invoke( this, e );
		//		return e.Absorbed;
		//	}
		//	return false;
		//}

		private bool CheckEvent( EventHandler<MouseEventArgs> inputEvent, MouseButton button )
		{
			if( inputEvent != null )
			{
				MouseEventArgs e = new MouseEventArgs( button, PointToClient( InputManager.MousePosition ) );
				inputEvent.Invoke( this, e );
				return e.Absorbed;
			}
			return false;
		}

		protected virtual void CheckPointerDown()
		{
			if( InputManager.AnyMousePressed && IntersectsPointer() )
			{
				PointerDown = true;
				//Console.WriteLine( "Pointer down: " + ToString() );
			}
		}

		//private MouseEventArgs CreateMouseArgs( MouseButton button )
		//{
		//	return new MouseEventArgs( button, PointToClient( InputManager.MousePosition ) );
		//}
		protected virtual bool HandlePointerEvents()
		{
			if( PointerDown )
			{
				if( InputManager.AnyMousePressed && CheckEvent( onInputPressed, InputManager.MousePressed ) )
				{
					//Console.WriteLine( ToString() + ": Pressed" );
					return true;
				}
				if( InputManager.AnyMouseDown )
				{
					bool inputMoveInitiated = false;
					if( InputManager.MouseMoved )
					{
						inputMoveInitiated = CheckEvent( onInputMoved, InputManager.MouseDown );
						//Console.WriteLine( ToString() + ": Moved " + mouseMoveInitiated );
					}
					if( CheckEvent( onInputHeld, InputManager.MouseDown ) )
					{
						//Console.WriteLine( "Pointer held: " + ToString() );
						return true;
					}
					return AbsorbPointer || inputMoveInitiated;
				}
				if( InputManager.AnyMouseReleased && IntersectsPointer() && CheckEvent( onInputReleased, InputManager.MouseReleased ) )
				{
					//Console.WriteLine( ToString() + ": Release" );
					return true;
				}
				//if( !InputManager.AnyMouseDown )
				{
					//Console.WriteLine( "Pointer Up: " + ToString() );
					PointerDown = false;
					//inputMoveInitiated = false;
					if( CheckEvent( onInputReleasedAnywhere, InputManager.MouseReleased ) )
					{
						return true;
					}
				}
			}
			return false;
		}

		//private bool inputMoveInitiated;
		//protected virtual bool HandlePointerEvents()
		//{
		//	if( PointerDown )
		//	{
		//		if( InputManager.AnyMousePressed && CheckOnInputPressed() )
		//		{
		//			//Console.WriteLine( ToString() + ": Pressed" );
		//			return true;
		//		}
		//		if( InputManager.AnyMouseDown )
		//		{
		//			if( InputManager.MouseMoved )
		//			{
		//				inputMoveInitiated = CheckOnInputMoved();
		//				//Console.WriteLine( ToString() + ": Moved " + mouseMoveInitiated );
		//			}
		//			if( CheckOnInputHeld() )
		//			{
		//				//Console.WriteLine( "Pointer held: " + ToString() );
		//				return true;
		//			}
		//			return AbsorbPointer || inputMoveInitiated;
		//		}
		//		if( InputManager.AnyMouseReleased && IntersectsPointer() && CheckOnInputReleased() )
		//		{
		//			//Console.WriteLine( ToString() + ": Release" );
		//			return true;
		//		}
		//		//if( !InputManager.AnyMouseDown )
		//		{
		//			//Console.WriteLine( "Pointer Up: " + ToString() );
		//			PointerDown = false;
		//			inputMoveInitiated = false;
		//			if( CheckOnInputReleasedAnywhere() )
		//			{
		//				return true;
		//			}
		//		}
		//	}
		//	return false;
		//}
		#endregion

		#region Decoration methods
		public virtual T AddDecoration<T>( T decoration ) where T : UIControl
		{
			if( decorations == null )
			{
				decorations = new List<UIControl>();
			}
			if( !decorations.Contains( decoration ) )
			{
				decoration.AssignParent( this );
				decorations.Add( decoration );
				HasDecorations = true;
			}
			return decoration;
		}

		public virtual bool RemoveDecoration( UIControl decoration )
		{
			if( HasDecorations && decoration != null && decorations.Remove( decoration ) )
			{
				decoration.AssignParent( null );
				HasDecorations = decorations.Count > 0;
				return true;
			}
			return false;
		}

		public virtual void ClearDecorations()
		{
			if( HasDecorations )
			{
				foreach( UIControl decoration in decorations )
				{
					RemoveDecoration( decoration );
				}
			}
		}

		//public virtual void ReorderDecoration( UIControl decoration, int zDepth )
		//{
		//	if( HasDecorations && decoration != null && decorations.Contains( decoration ) )
		//	{
		//		int removeIndex = decorations.IndexOf( decoration );
		//		decorations.Insert( zDepth, decoration );
		//		decorations.RemoveAt( removeIndex );
		//	}
		//}

		//public virtual void MoveDecorationToFront( UIControl decoration )
		//{
		//	if( HasDecorations && decoration != null && decorations.Remove( decoration ) )
		//	{
		//		decorations.Add( decoration );
		//	}
		//}

		//public virtual void MoveDecorationToBack( UIControl decoration )
		//{
		//	if( HasDecorations && decoration != null && decorations.Remove( decoration ) )
		//	{
		//		decorations.Insert( 0, decoration );
		//	}
		//}

		//public virtual void MoveDecorationBefore( UIControl decoration, UIControl relative )
		//{
		//	if( HasDecorations && decoration != null && relative != null && decorations.Contains( decoration ) && decorations.Contains( relative ) )
		//	{
		//		decorations.Remove( decoration );
		//		decorations.Insert( decorations.IndexOf( relative ), decoration );
		//	}
		//}

		//public virtual void MoveDecorationAfter( UIControl decoration, UIControl relative )
		//{
		//	if( HasDecorations && decoration != null && relative != null && decorations.Contains( decoration ) && decorations.Contains( relative ) )
		//	{
		//		decorations.Remove( decoration );
		//		decorations.Insert( decorations.IndexOf( relative ) + 1, decoration );
		//	}
		//}
		#endregion

		#region Methods
		internal void CheckInitialize()
		{
			if( !initialized )
			{
				Initialize();
			}
		}

		/// <summary>
		/// Override and initialize any control related variables here.
		/// Called internally by the UI system prior to usage.
		/// </summary>
		protected virtual void Initialize()
		{
			initialized = true;
		}

		internal void AssignParent( UIControl control )
		{
			CheckInitialize();
			parent = control;
			HasParent = control != null;
		}

		public virtual void DoLayout()
		{
			suspendLayout = false;
			UILayoutEngine.EnforceConstraints( this );
			if( HasDecorations )
			{
				decorations.ForEach( d => d.DoLayout() );
			}
			layoutDone = true;
		}

		public bool HasClip( out Rectangle clip )
		{
			if( HasParent && parent.HasClip( out clip ) )
			{
				if( ClipContent )
				{
					Rectangle r = Bounds;
					Rectangle intersection = Rectangle.Empty;
					intersection.Left = Math.Max( clip.Left, r.Left );
					intersection.Top = Math.Max( clip.Top, r.Top );
					intersection.Right = Math.Min( clip.Right, r.Right );
					intersection.Bottom = Math.Min( clip.Bottom, r.Bottom );
					clip = intersection;
				}
				return true;
			}

			if( ClipContent )
			{
				clip = Bounds;
				return true;
			}

			clip = Rectangle.Empty;
			return false;
		}

		private bool IntersectsPointer()
		{
			Vector2 position = InputManager.MousePosition;
			if( position.X < DrawPosition.X )
			{
				return false;
			}
			if( position.X > DrawPosition.X + size.X )
			{
				return false;
			}
			if( position.Y < DrawPosition.Y )
			{
				return false;
			}
			if( position.Y > DrawPosition.Y + size.Y )
			{
				return false;
			}
			return true;
		}

		public Vector2 PointToClient( Vector2 position )
		{
			return position - DrawPosition;
		}

		public virtual void Update( GameTime gameTime )
		{
			CheckPointerDown();
			if( !InputManager.PointerHandled )
			{
				InputManager.PointerHandled = HandlePointerEvents();
			}
		}

		public virtual void Draw( SpriteBatch spriteBatch )
		{
			if( DrawBounds && Size != Vector2.Zero )
			{
				spriteBatch.Draw( Engine.Texture1x1, Bounds, null, Color, Radians, Origin, SpriteEffect, 1f );
			}

			if( HasDecorations )
			{
				decorations.ForEach( d => d.Draw( spriteBatch ) );
			}
		}

		public override string ToString()
		{
			return this.GetType().Name + " Bounds: " + Bounds.Size.ToString();
		}
		#endregion
	}
}