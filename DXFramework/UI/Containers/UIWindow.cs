using SharpDX;
using SharpDX.Toolkit;

namespace DXFramework.UI
{
	public class UIWindow : UIPanel
	{
		private const string RESOURCE_MAXIMIZE = "graphics/btn_maximize";
		private const string RESOURCE_WINDOWED = "graphics/btn_windowed";

		private UILabel titleLabel;
		private bool resizing;
		private bool moving;
		private Vector2 inputStart;
		private Vector2 resizeStartSize;
		private UIButton btn_close;
		private UIButton btn_maximize;
		private UIButton btn_minimize;
		private Vector2 oldSize;
		private Vector2 oldMinSize;
		private Vector2 oldPos;

		public UIWindow()
		{
			AutoSize = false;
			Alpha = 0f;
			MinimumSize = new Vector2(200);
			Size = new Vector2(350, 250);
			TopPanel = new UIPanel();
			BodyPanel = new UIPanel();
			BottomPanel = new UIPanel();

			// Top panel
			TopPanel.AddConstraint(Edge.Top, this, Edge.Top);
			TopPanel.AddConstraint(Edge.Horizontal, this, Edge.Horizontal);
			TopPanel.Color = Color.White;
			TopPanel.Alpha = 1f;
			TopPanel.Tag = nameof(TopPanel);
			TopPanel.InputMoved += TopPanel_InputMoved;
			TopPanel.InputReleasedAnywhere += TopPanel_InputReleasedAnywhere;

			titleLabel = new UILabel(nameof(UIWindow));
			titleLabel.AddConstraint(Edge.Top, TopPanel, Edge.Top);
			titleLabel.AddConstraint(Edge.Left, TopPanel, Edge.Left, 5);
			titleLabel.Tag = "Titel label";
			TopPanel.AddChild(titleLabel);

			btn_close = new UIButton();
			btn_close.InputEnter += btn_close_InputEnter;
			btn_close.InputLeave += btn_close_InputLeave;
			btn_close.AddDecoration(new UIImage("graphics/btn_close") { Color = Color.Black });
			btn_close.AddConstraint(Edge.TopRight, TopPanel, Edge.TopRight);
			btn_close.HighlightZoom = false;
			btn_close.InputReleased += Btn_close_InputReleased;
			TopPanel.AddChild(btn_close);

			btn_maximize = new UIButton();
			btn_maximize.Color = Color.Transparent;
			btn_maximize.PointedColor = new Color(190, 190, 190);
			btn_maximize.PressedColor = new Color(120, 120, 120);
			btn_maximize.PointedAlpha = 0.5f;
			btn_maximize.PressedAlpha = 0.5f;
			btn_maximize.AddDecoration(new UIImage(RESOURCE_MAXIMIZE) { Color = Color.Black });
			btn_maximize.AddConstraint(Edge.Top, btn_close, Edge.Top);
			btn_maximize.AddConstraint(Edge.Right, btn_close, Edge.Left, -1);
			btn_maximize.HighlightZoom = false;
			btn_maximize.InputReleased += Btn_maximize_InputReleased;
			TopPanel.AddChild(btn_maximize);

			btn_minimize = new UIButton();
			btn_minimize.Color = Color.Transparent;
			btn_minimize.PointedColor = new Color(190, 190, 190);
			btn_minimize.PressedColor = new Color(120, 120, 120);
			btn_minimize.PointedAlpha = 0.5f;
			btn_minimize.PressedAlpha = 0.5f;
			btn_minimize.AddDecoration(new UIImage("graphics/btn_minimize") { Color = Color.Black });
			btn_minimize.AddConstraint(Edge.Top, btn_maximize, Edge.Top);
			btn_minimize.AddConstraint(Edge.Right, btn_maximize, Edge.Left, -1);
			btn_minimize.HighlightZoom = false;
			btn_minimize.InputReleased += Btn_minimize_InputReleased;
			TopPanel.AddChild(btn_minimize);

			// Body panel
			BodyPanel.AddConstraint(Edge.Top, TopPanel, Edge.Bottom);
			BodyPanel.AddConstraint(Edge.Horizontal, this, Edge.Horizontal);
			BodyPanel.AddConstraint(Edge.Bottom, BottomPanel, Edge.Top);
			//BodyPanel.Color = Color.White;
			//BodyPanel.Alpha = 1f;
			BodyPanel.Tag = nameof(BodyPanel);

			// Bottom panel
			BottomPanel.AddConstraint(Edge.Bottom, this, Edge.Bottom);
			BottomPanel.AddConstraint(Edge.Horizontal, this, Edge.Horizontal);
			BottomPanel.Color = Color.White;
			BottomPanel.Alpha = 1f;
			BottomPanel.Tag = nameof(BottomPanel);
			BottomPanel.Size = new Vector2(0, 20);

			// Resize grip
			ResizeGrip = new UIImage("graphics/resizeGrip");
			ResizeGrip.Color = Color.Black;
			ResizeGrip.AddConstraint(Edge.BottomRight, BottomPanel, Edge.BottomRight);
			ResizeGrip.Tag = nameof(ResizeGrip);
			ResizeGrip.InputPressed += ResizeGrip_InputPressed;
			ResizeGrip.InputReleasedAnywhere += ResizeGrip_InputReleasedAnywhere;
			BottomPanel.AddChild(ResizeGrip);

			AddChild(TopPanel);
			AddChild(BodyPanel);
			AddChild(BottomPanel);

			Resizable = true;
		}

		public UIPanel TopPanel { get; private set; }

		public UIPanel BodyPanel { get; private set; }

		public UIPanel BottomPanel { get; private set; }

		public UIImage ResizeGrip { get; private set; }

		public bool Resizable
		{
			get { return ResizeGrip.Visible; }
			set { ResizeGrip.Visible = value; }
		}

		public WindowState State { get; private set; }

		private void Btn_minimize_InputReleased(object sender, MouseEventArgs e)
		{
			switch (State)
			{
				case WindowState.Windowed:
					SetState(WindowState.Minimized);
					break;

				case WindowState.Minimized:
					SetState(WindowState.Windowed);
					break;

				case WindowState.Maximized:
					SetState(WindowState.Minimized);
					break;
			}
		}

		private void Btn_maximize_InputReleased(object sender, MouseEventArgs e)
		{
			switch (State)
			{
				case WindowState.Windowed:
					SetState(WindowState.Maximized);
					break;

				case WindowState.Minimized:
					SetState(WindowState.Maximized);
					break;

				case WindowState.Maximized:
					SetState(WindowState.Windowed);
					break;
			}
		}

		private void Btn_close_InputReleased(object sender, MouseEventArgs e)
		{
			Dispose();
		}

		private void btn_close_InputEnter(object sender, MouseEventArgs e)
		{
			(sender as UIButton).Decorations[0].Color = Color.White;
		}

		private void btn_close_InputLeave(object sender, MouseEventArgs e)
		{
			(sender as UIButton).Decorations[0].Color = Color.Black;
		}

		private void ResizeGrip_InputPressed(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				inputStart = InputManager.MousePosition;
				resizeStartSize = size;
				resizing = true;
			}
		}

		private void ResizeGrip_InputReleasedAnywhere(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				resizing = false;
			}
		}

		private void TopPanel_InputMoved(object sender, MouseEventArgs e)
		{
			if (!moving && e.Button == MouseButton.Left)
			{
				inputStart = e.ClientPosition;
				moving = true;
			}
		}

		private void TopPanel_InputReleasedAnywhere(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				moving = false;
			}
		}

		public void SetTitle(string title)
		{
			titleLabel.SetText(title);
		}

		public void SetState(WindowState state)
		{
			switch (state)
			{
				case WindowState.Windowed:
					{
						BodyPanel.Visible = true;
						BottomPanel.Visible = true;
						Resizable = true;
						SetBtnMaximizeImage(RESOURCE_MAXIMIZE);

						var tmpPos = Position;
						Position = oldPos;
						oldPos = tmpPos;

						var tmpMin = MinimumSize;
						MinimumSize = oldMinSize;
						oldMinSize = tmpMin;

						var tmpSize = Size;
						Size = oldSize;
						oldSize = tmpSize;

						DoLayout(ConstraintCategory.Update);
					}
					break;

				case WindowState.Minimized:
					{
						BodyPanel.Visible = false;
						BottomPanel.Visible = false;
						Resizable = false;
						SetBtnMaximizeImage(RESOURCE_MAXIMIZE);

						switch (State)
						{
							case WindowState.Windowed:
								oldSize = Size;
								oldPos = Position;
								oldMinSize = MinimumSize;
								break;

							case WindowState.Minimized:
								break;

							case WindowState.Maximized:
								break;
						}

						MinimumSize = Vector2.Zero;
						Size = new Vector2(200, TopPanel.Size.Y);
						this.EnforceConstraint(new UIConstraint(Edge.Bottom, null, Edge.Bottom, 5));
						DoLayoutOnChildren(ConstraintCategory.Update);
					}
					break;

				case WindowState.Maximized:
					{
						BodyPanel.Visible = true;
						BottomPanel.Visible = true;
						Resizable = false;
						SetBtnMaximizeImage(RESOURCE_WINDOWED);

						switch (State)
						{
							case WindowState.Windowed:
								oldPos = Position;
								oldSize = Size;
								oldMinSize = MinimumSize;
								break;

							case WindowState.Minimized:
								break;

							case WindowState.Maximized:
								break;
						}

						Position = Vector2.Zero;
						Size = Engine.ScreenSize;
						DoLayout(ConstraintCategory.Update); // TODO: It should not be necessary to call DoLayout after a size change.
					}
					break;
			}
			State = state;
		}

		private void SetBtnMaximizeImage(string resource)
		{
			var img = btn_maximize.Decorations[0] as UIImage;
			img.Resource = resource;
			img.Initialize();
		}

		public override void Update(GameTime gameTime)
		{
			if (moving && InputManager.MouseMoved)
			{
				if (State == WindowState.Maximized)
				{
					SetState(WindowState.Windowed);
					inputStart = TopPanel.Size * 0.5f;
				}
				Position = InputManager.MousePosition - inputStart;
			}
			if (resizing && InputManager.MouseMoved)
			{
				Vector2 delta = inputStart - InputManager.MousePosition;
				Size = resizeStartSize - delta;
				DoLayout(ConstraintCategory.Update);
			}
			base.Update(gameTime);
		}

		public enum WindowState
		{
			Windowed,
			Minimized,
			Maximized
		}
	}
}