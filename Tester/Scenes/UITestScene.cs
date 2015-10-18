using DXFramework;
using DXFramework.SceneManagement;
using DXFramework.UI;
using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace XManager.Scenes
{
	/// <summary>
	/// Right after the PreMenu scene the menu is displayed.
	/// </summary>
	public class UITestScene : Scene
	{
		private Camera cam;
		private UIManager uiManager;
		private UITweenPanel menuPanel;

		public override void LoadContent()
		{
			cam = new Camera(GraphicsDevice);
			uiManager = new UIManager();
			uiManager.EnableProfilling = true;

			InitTextWindow();
			InitMenuPanel();
			InitScrollWindow();
			InitButton();

#if DEBUG
			UIManager.DrawDebug = true;
#else
			UIManager.DrawDebug = false;
#endif
		}

		private void InitButton()
		{
			UIButton btn = new UIButton();
			btn.AddDecoration(new UIImage("graphics/arrow_down"));
			btn.Tag = "Arrow";
			btn.AddConstraint(Edge.Right, null, Edge.Right, 100f, ConstraintCategory.Initialization);
			btn.AddConstraint(Edge.CenterY, null, Edge.CenterY, ConstraintCategory.Initialization);
			btn.InputMoved += btn_MouseMoved;

			uiManager.Add(btn);
		}

		private void InitTextWindow()
		{
			UIScrollWindow textWindow = new UIScrollWindow();
			textWindow.Location = new Vector2(300, 300);
			textWindow.ScrollPanel.Restriction = ScrollRestriction.Vertical;
			textWindow.Size = new Vector2(180);
			textWindow.AddConstraint(Edge.Horizontal | Edge.Bottom, null, Edge.Horizontal | Edge.Bottom, 5, ConstraintCategory.Initialization);

			string str = "A multiline UILabel in a UIScrollPanel. ";
			for (int i = 0; i < 6; i++)
			{
				str += str;
			}

			UILabel label = new UILabel(str, true);
			label.AddConstraint(Edge.TopLeft, textWindow.ScrollPanel, Edge.TopLeft, ConstraintCategory.Initialization);
			label.AddConstraint(Edge.Right, textWindow.ScrollPanel, Edge.Right, ConstraintCategory.Initialization);

			textWindow.AddChild(label);

			uiManager.Add(textWindow);
		}

		private void InitScrollWindow()
		{
			UIScrollWindow sw = new UIScrollWindow();
			sw.Location = new Vector2(100, 200);
			sw.Size = new Vector2(100);

			UIImage img = new UIImage("graphics/arrow_down");
			img.AddConstraint(Edge.CenterXY, sw, Edge.TopLeft, ConstraintCategory.Initialization);
			sw.AddChild(img);

			uiManager.Add(sw);
		}

		private void InitMenuPanel()
		{
			menuPanel = new UITweenPanel();
			menuPanel.AddConstraint(Edge.CenterXY, null, Edge.CenterXY);
			menuPanel.Alpha = 0f;
			menuPanel.AddChild(GetMenuEntry("New Game"));
			menuPanel.AddChild(GetMenuEntry("Continue"));
			menuPanel.AddChild(GetMenuEntry("Options"));
			menuPanel.AddChild(GetMenuEntry("Help"));
			menuPanel.AddChild(GetMenuEntry("Exit"));

			uiManager.Add(menuPanel);
		}

		private UIControl previousEntry;
		private UIControl GetMenuEntry(string label)
		{
			var btn = new UIButton();
			btn.Tag = label;
			btn.AddDecoration(new UILabel(label));

			if (previousEntry == null)
			{
				btn.AddConstraint(Edge.CenterXY, menuPanel, Edge.CenterXY, ConstraintCategory.Initialization);
			}
			else
			{
				btn.AddConstraint(Edge.CenterX, previousEntry, Edge.CenterX, ConstraintCategory.Initialization);
				btn.AddConstraint(Edge.Top, previousEntry, Edge.Bottom, -20, ConstraintCategory.Initialization);
			}

			previousEntry = btn;
			return btn;
		}

		void btn_MouseMoved(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Right)
			{
				(sender as UIControl).Location += InputManager.MouseDelta;
			}
		}

		void menuPanel_MouseMoved(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Right)
			{
				System.Console.WriteLine(InputManager.MouseDelta);
				(sender as UIControl).Location += InputManager.MouseDelta;
			}
			else
			{
				e.Absorbed = false;
			}
		}

		public override void Update(GameTime gameTime)
		{
			//if( InputManager.AnyMouse )
			//{
			//	SceneManager.Set( typeof( Loading ).Name );
			//}

			uiManager.Update(gameTime);

			if (InputManager.Pressed(Keys.Left))
			{
				menuPanel.Toggle(Edge.Left);
			}
			else if (InputManager.Pressed(Keys.Right))
			{
				menuPanel.Toggle(Edge.Right);
			}
			if (InputManager.Pressed(Keys.Up))
			{
				menuPanel.Toggle(Edge.Top);
			}
			if (InputManager.Pressed(Keys.Down))
			{
				menuPanel.Toggle(Edge.Bottom);
			}

			if (InputManager.Pressed(Keys.F12))
			{
				UIManager.DrawDebug = !UIManager.DrawDebug;
			}

			if (!InputManager.PointerHandled)
			{
				if (InputManager.Pressed(MouseButton.Right) || InputManager.Pressed(MouseButton.Left))
				{
					//System.Console.WriteLine("Click");
					cam.InitializeMovement();
				}
				else if (InputManager.Held(MouseButton.Right) || InputManager.Held(MouseButton.Left))
				{
					cam.DoMovement();
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Brown);
			uiManager.Draw(spriteBatch);
		}
	}
}