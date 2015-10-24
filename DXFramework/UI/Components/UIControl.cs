using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.UI
{
	[DataContract]
	public abstract class UIControl
	{
		internal Vector2 location;
		protected Color color;
		protected float layerDepth;
		private UIControl parent;
		private List<UIControl> decorations;
		private SpriteEffects spriteEffect;
		internal Vector2 size;
		internal Vector2 scale;
		private bool suspendLayout;
		protected bool layoutDone;
		protected bool initialized;
		protected RectangleF sourceRect;
		protected RectangleF boundingRect;
		protected Matrix transform;

		public UIControl()
		{
			Enabled = true;
			Visible = true;
			scale = Vector2.One;
			Alpha = 1f;
			Color = Color.White;
			layerDepth = 1f;
			DebugColor = Color.Black;
			NormalizedOrigin = new Vector2(0.5f);
			suspendLayout = true;
			initialized = false;
		}

		#region Properties
		public Matrix Transform
		{
			get { return transform; }
		}

		public bool UpdateTransformation { get; set; }

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
			set
			{
				suspendLayout = value;
				if (HasDecorations)
				{
					decorations.ForEach(d => d.SuspendLayout = value);
				}
			}
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

		internal Vector2 drawPosition;

		public Vector2 DrawPosition
		{
			get { return drawPosition; }
			set
			{
				if (drawPosition != value)
				{
					drawPosition = value;
					UpdateTransformation = true;
				}
			}
		}

		public virtual Vector2 Location
		{
			get { return location; }
			set
			{
				if (location != value)
				{
					location = value;
					UpdateTransformation = true;
					this.UpdateDrawLocation();
				}
			}
		}

		/// <summary>
		/// Normalized origin [0..1].
		/// </summary>
		public virtual Vector2 NormalizedOrigin { get; set; }

		public virtual Vector2 Origin
		{
			get { return NormalizedOrigin * size; }
		}

		public virtual Vector2 ScaledOrigin
		{
			get { return NormalizedOrigin * ScaledSize; }
		}

		public virtual Vector2 Size
		{
			get { return size; }
			set
			{
				if (size != value)
				{
					size = value;
					UpdateTransformation = true;
					sourceRect = new RectangleF(0, 0, value.X, value.Y);
					DoLayout(ConstraintCategory.Update);
				}
			}
		}

		public Vector2 ScaledSize
		{
			get { return scale * size; }
		}

		public Vector2 Scale
		{
			get { return scale; }
			set
			{
				if (scale != value)
				{
					scale = value;
					UpdateTransformation = true;
					DoLayout(ConstraintCategory.Update);
				}
			}
		}

		public void IncrementScale(Vector2 delta, bool scaleChrildren = false)
		{
			SetScale(scale + delta, scaleChrildren);
		}

		public virtual void SetScale(Vector2 scale, bool scaleChrildren = false)
		{
			Vector2 delta = scale - this.scale;

			if (!delta.IsZero)
			{
				Scale = scale;
				if (scaleChrildren && HasDecorations)
				{
					decorations.ForEach(d => d.SetScale(d.scale + delta, scaleChrildren));
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
				if (location.X != value)
				{
					location.X = value;
					this.UpdateDrawLocation();
				}
			}
		}

		public float Top
		{
			get { return location.Y; }
			set
			{
				if (location.Y != value)
				{
					location.Y = value;
					this.UpdateDrawLocation();
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

		public Vector2 LocalCenter
		{
			get { return location + size * 0.5f; }
		}

		public virtual RectangleF Bounds
		{
			get
			{
				UpdateTransform();
				return boundingRect;
			}
		}

		/// <summary>
		/// Normalized Alpha [0..1]
		/// </summary>
		[DataMember]
		public float Alpha
		{
			get { return color.A * (1f / 255f); }
			set { color.A = (byte)(value * 255); }
		}

		[DataMember]
		public Color Color
		{
			get { return color; }
			set
			{
				if (color != value)
				{
					value.A = color.A;
					color = value;
				}
			}
		}

		protected Color DebugColor { get; set; }

		public SpriteEffects SpriteEffect
		{
			get { return spriteEffect; }
			set { spriteEffect = value; }
		}

		public virtual float LayerDepth
		{
			get { return layerDepth; }
			set
			{
				if (layerDepth != value)
				{
					layerDepth = value;
					if (HasDecorations)
					{
						decorations.ForEach(d => d.LayerDepth = value);
					}
				}
			}
		}

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
			add { onInputPressed += value.MakeWeak(e => onInputPressed -= e); }
			remove { onInputPressed -= onInputPressed.Unregister(value); }
		}

		public event EventHandler<MouseEventArgs> InputDown
		{
			add { onInputHeld += value.MakeWeak(e => onInputHeld -= e); }
			remove { onInputHeld -= onInputHeld.Unregister(value); }
		}

		public event EventHandler<MouseEventArgs> InputReleased
		{
			add { onInputReleased += value.MakeWeak(e => onInputReleased -= e); }
			remove { onInputReleased -= onInputReleased.Unregister(value); }
		}

		public event EventHandler<MouseEventArgs> InputMoved
		{
			add { onInputMoved += value.MakeWeak(e => onInputMoved -= e); }
			remove { onInputMoved -= onInputMoved.Unregister(value); }
		}

		public event EventHandler<MouseEventArgs> InputReleasedAnywhere
		{
			add { onInputReleasedAnywhere += value.MakeWeak(e => onInputReleasedAnywhere -= e); }
			remove { onInputReleasedAnywhere -= onInputReleasedAnywhere.Unregister(value); }
		}

		public event EventHandler<MouseEventArgs> InputEnter
		{
			add { onInputEnter += value.MakeWeak(e => onInputEnter -= e); }
			remove { onInputEnter -= onInputEnter.Unregister(value); }
		}

		public event EventHandler<MouseEventArgs> InputLeave
		{
			add { onInputLeave += value.MakeWeak(e => onInputLeave -= e); }
			remove { onInputLeave -= onInputLeave.Unregister(value); }
		}

		public event EventHandler<MouseEventArgs> InputHover
		{
			add { onInputHover += value.MakeWeak(e => onInputHover -= e); }
			remove { onInputHover -= onInputHover.Unregister(value); }
		}

		private bool CheckEvent(EventHandler<MouseEventArgs> inputEvent, MouseButton button)
		{
			if (inputEvent != null)
			{
				MouseEventArgs e = new MouseEventArgs(button, PointToClient(InputManager.MousePosition));
				inputEvent.Invoke(this, e);
				return e.Absorbed;
			}
			return false;
		}

		private static UIControl pointedControl;
		private static UIControl heldControl;

		protected virtual bool HandlePointerEvents()
		{
			if (heldControl != null && heldControl != this)
			{
				return false;
			}

			bool pointerHandled = false;

			if (pointedControl == this)
			{
				pointerHandled = true;

				if (IntersectsPointer())
				{
					if (CheckEvent(onInputHover, MouseButton.None))
					{
					}
				}
				else
				{
					//Console.WriteLine("Left 1: " + ToString());
					CheckEvent(onInputLeave, MouseButton.None);
					pointedControl = null;
				}
			}
			else
			{
				if (IntersectsPointer())
				{
					if (pointedControl != null && pointedControl != this)
					{
						//Console.WriteLine("Left 2: " + pointedControl.ToString());
						pointedControl.CheckEvent(pointedControl.onInputLeave, MouseButton.None);
						pointedControl = null;
					}

					//Console.WriteLine("Entered: " + ToString());
					CheckEvent(onInputEnter, MouseButton.None);
					UIControl.pointedControl = this;
					pointerHandled = true;
				}
			}

			if (PointerDown)
			{
				if (InputManager.AnyMousePressed && CheckEvent(onInputPressed, InputManager.MousePressed))
				{
					//Console.WriteLine("Pressed: " + ToString());
					pointerHandled = true;
					heldControl = this;
				}

				if (InputManager.AnyMouseDown)
				{
					bool inputMoveInitiated = false;
					if (InputManager.MouseMoved)
					{
						inputMoveInitiated = CheckEvent(onInputMoved, InputManager.MouseDown);
						//Console.WriteLine( ToString() + ": Moved " + mouseMoveInitiated );
					}
					if (CheckEvent(onInputHeld, InputManager.MouseDown))
					{
						//Console.WriteLine("Pointer held: " + ToString());
						pointerHandled = true;
						heldControl = this;
					}
					pointerHandled = AbsorbPointer || inputMoveInitiated;
				}

				if (InputManager.AnyMouseReleased && IntersectsPointer() && CheckEvent(onInputReleased, InputManager.MouseReleased))
				{
					//Console.WriteLine("Release: " + ToString());
					pointerHandled = true;
					heldControl = null;
				}

				if (!InputManager.AnyMouseDown)
				{
					//Console.WriteLine("Pointer Up: " + ToString());
					PointerDown = false;
					if (CheckEvent(onInputReleasedAnywhere, InputManager.MouseReleased))
					{
						pointerHandled = true;
					}

					if (heldControl != null)
					{
						heldControl = null;
						pointerHandled = true;
					}
				}
			}

			return pointerHandled;
		}

		protected virtual void CheckPointerDown()
		{
			if (!PointerDown && InputManager.AnyMousePressed && IntersectsPointer())
			{
				PointerDown = true;
				//Console.WriteLine( "Pointer down: " + ToString() );
			}
		}
		#endregion

		#region Decoration methods
		public virtual T AddDecoration<T>(T decoration) where T : UIControl
		{
			if (decoration == null)
			{
				return default(T);
			}

			if (decorations == null)
			{
				decorations = new List<UIControl>();
			}
			if (!decorations.Contains(decoration))
			{
				decoration.AssignParent(this);
				decorations.Add(decoration);
				HasDecorations = true;
			}
			return decoration;
		}

		public virtual bool RemoveDecoration(UIControl decoration)
		{
			if (HasDecorations && decoration != null && decorations.Remove(decoration))
			{
				decoration.AssignParent(null);
				HasDecorations = decorations.Count > 0;
				return true;
			}
			return false;
		}

		public virtual UIControl GetDecoration(int index)
		{
			if (!HasDecorations || index < 0 || index >= decorations.Count)
			{
				return null;
			}
			return decorations[index];
		}

		public virtual void ClearDecorations()
		{
			if (HasDecorations)
			{
				foreach (UIControl decoration in decorations)
				{
					RemoveDecoration(decoration);
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
			if (!initialized)
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

		internal void AssignParent(UIControl control)
		{
			CheckInitialize();
			parent = control;
			HasParent = control != null;
		}

		public virtual void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			this.EnforceConstraints(category);
			if (HasDecorations)
			{
				decorations.ForEach(d => d.DoLayout(category));
			}
			layoutDone = true;
		}

		public bool HasClip(out RectangleF clip)
		{
			if (HasParent && parent.HasClip(out clip))
			{
				if (ClipContent)
				{
					RectangleF r = Bounds;
					RectangleF intersection = RectangleF.Empty;
					intersection.Left = Math.Max(clip.Left, r.Left);
					intersection.Top = Math.Max(clip.Top, r.Top);
					intersection.Right = Math.Min(clip.Right, r.Right);
					intersection.Bottom = Math.Min(clip.Bottom, r.Bottom);
					clip = intersection;
				}
				return true;
			}

			if (ClipContent)
			{
				clip = Bounds;
				return true;
			}

			clip = RectangleF.Empty;
			return false;
		}

		protected bool IntersectsPointer()
		{
			Vector2 position = InputManager.MousePosition;
			RectangleF b = Bounds;
			if (position.X < b.Left || position.X > b.Right ||
				position.Y < b.Top || position.Y > b.Bottom)
			{
				return false;
			}
			return true;
		}

		public Vector2 PointToClient(Vector2 position)
		{
			return position - DrawPosition;
		}

		private void UpdateTransform()
		{
			if (UpdateTransformation)
			{
				sourceRect = new RectangleF(0, 0, size.X, size.Y); // Todo: SourceRect should be properly set from the size property.
																   //boundingRect = new RectangleF( DrawPosition.X, DrawPosition.Y, size.X, size.Y );

				transform =
					Matrix.Translation(new Vector3(-Origin, 0f)) *
					Matrix.Scaling(new Vector3(Scale, 0f)) *
					//Matrix.RotationZ( Radians ) *
					Matrix.Translation(new Vector3(DrawPosition + Origin, 0f));

				CalculateBoundingRectangle(ref sourceRect, ref transform, out boundingRect);
				UpdateTransformation = false;
			}
		}

		public virtual void Update(GameTime gameTime)
		{
			CheckPointerDown();
			if (!InputManager.PointerHandled)
			{
				InputManager.PointerHandled = HandlePointerEvents();
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			UpdateTransform();
			if (DrawBounds && Size != Vector2.Zero)
			{
				spriteBatch.Draw(Engine.Texture1x1, Bounds, null, Color, 0f, Vector2.Zero, SpriteEffect, LayerDepth);
			}

			if (HasDecorations)
			{
				decorations.ForEach(d => d.Draw(spriteBatch));
			}

			if (UIManager.DrawDebug)
			{
				PrimitiveBatch.Begin();
				PRect rect = new PRect(Bounds, 3);
				rect.Color = DebugColor;
				rect.Draw();
				PrimitiveBatch.End();
			}
		}

		public override string ToString()
		{
			return (Tag != null ? Tag.ToString() + " | " : "") + $"{GetType().Name} | {Bounds.Size.ToString()}";
		}
		#endregion

		/// <summary>
		/// Calculates an axis aligned rectangle which fully contains an arbitrarily transformed axis aligned rectangle.
		/// </summary>
		/// <param name="rectangle">Original bounding rectangle.</param>
		/// <param name="transform">World transform of the rectangle.</param>
		/// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
		public static void CalculateBoundingRectangle(ref RectangleF rectangle, ref Matrix transform, out RectangleF boundingRect)
		{
			// Get all four corners in local space
			Vector2 leftTop;
			leftTop.X = rectangle.Left;
			leftTop.Y = rectangle.Top;

			Vector2 rightTop;
			rightTop.X = rectangle.Right;
			rightTop.Y = rectangle.Top;

			Vector2 leftBottom;
			leftBottom.X = rectangle.Left;
			leftBottom.Y = rectangle.Bottom;

			Vector2 rightBottom;
			rightBottom.X = rectangle.Right;
			rightBottom.Y = rectangle.Bottom;

			// Transform all four corners into work space
			Vector2Helper.Transform(ref leftTop, ref transform, out leftTop);
			Vector2Helper.Transform(ref rightTop, ref transform, out rightTop);
			Vector2Helper.Transform(ref leftBottom, ref transform, out leftBottom);
			Vector2Helper.Transform(ref rightBottom, ref transform, out rightBottom);

			// Find the minimum and maximum extents of the rectangle in world space
			Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom));
			Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

			// Return that as a rectangle
			boundingRect = new RectangleF();
			boundingRect.X = min.X;
			boundingRect.Y = min.Y;
			boundingRect.Width = (max.X - min.X);
			boundingRect.Height = (max.Y - min.Y);
		}
	}
}