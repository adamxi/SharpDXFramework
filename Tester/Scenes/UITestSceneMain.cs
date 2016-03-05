using DXFramework;
using DXFramework.SceneManagement;
using DXFramework.UI;
using DXFramework.Util;
using SharpDX;
using SharpDX.IO;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.IO;

namespace XManager.Scenes
{
	/// <summary>
	/// Right after the PreMenu scene the menu is displayed.
	/// </summary>
	public class UITestSceneMain : TestScene
	{
		private Camera cam;
		private UIManager uiManager;
		private UITweenPanel menuPanel;

		public override void LoadContent()
		{
			UIManager.DrawDebug = false;
			base.LoadContent();
			cam = new Camera(GraphicsDevice);
			uiManager = new UIManager();
			uiManager.EnableProfilling = true;

			InitTextWindow();
			InitMenuPanel();
			InitWindow();
		}

		private void InitTextWindow()
		{
			UIScrollWindow textWindow = new UIScrollWindow();
			textWindow.Position = new Vector2(300, 300);
			textWindow.ScrollPanel.Restriction = ScrollRestriction.Vertical;
			textWindow.Size = new Vector2(0, 180);
			textWindow.AddConstraint(Edge.Bottom, null, Edge.Bottom, 5);
			textWindow.AddConstraint(Edge.Horizontal, null, Edge.Horizontal, 5);

			string text = NativeFile.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Lorem Ipsum.txt"));
			UILabel label = new UILabel(text, true);
			label.AddConstraint(Edge.TopLeft, textWindow.ScrollPanel, Edge.TopLeft, ConstraintCategory.Initialization);
			label.AddConstraint(Edge.Right, textWindow.ScrollPanel, Edge.Right, ConstraintCategory.Initialization);
			textWindow.AddChild(label);

			uiManager.Add(textWindow);
		}

		private void InitMenuPanel()
		{
			menuPanel = new UITweenPanel();
			menuPanel.AddConstraint(Edge.CenterXY, null, Edge.CenterXY);
			menuPanel.Alpha = 0.5f;
			menuPanel.AddChild(GetMenuEntry("New Game"));
			menuPanel.AddChild(GetMenuEntry("Continue"));
			menuPanel.AddChild(GetMenuEntry("Options"));
			menuPanel.AddChild(GetMenuEntry("Help"));
			menuPanel.AddChild(GetMenuEntry("Exit"));

			uiManager.Add(menuPanel);
		}

		private void InitWindow()
		{
			var window = new UIWindow();
			window.AddConstraint(Edge.CenterY, null, Edge.CenterY, 0f, ConstraintCategory.Initialization);
			window.AddConstraint(Edge.Right, null, Edge.Right, 50f, ConstraintCategory.Initialization);

			var contentPanel = new UIScrollWindow(UIScrollWindow.ScrollBarMode.Both);
			contentPanel.AddConstraint(Edge.Dock, window.BodyPanel, Edge.Dock);

			var img = new UIImage("graphics/map");
			img.Tag = "Map";
			img.AddConstraint(Edge.TopLeft, contentPanel.ScrollPanel, Edge.TopLeft, ConstraintCategory.Initialization);
			contentPanel.AddChild(img);

			window.BodyPanel.AddChild(contentPanel);
			uiManager.Add(window);
		}

		private UIControl previousEntry;
		private UIControl GetMenuEntry(string label)
		{
			var btn = new UIButton();
			btn.Tag = label;
			btn.AddDecoration(new UILabel(label));
			//btn.NormalizedOrigin = Vector2.Zero;

			btn.AddConstraint(Edge.CenterX, menuPanel, Edge.CenterX, ConstraintCategory.All);

			if (previousEntry != null)
			{
				btn.AddConstraint(Edge.Top, previousEntry, Edge.Bottom, -20, ConstraintCategory.All);
			}

			previousEntry = btn;
			return btn;
		}

		void menuPanel_MouseMoved(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButton.Right)
			{
				System.Console.WriteLine(InputManager.MouseDelta);
				(sender as UIControl).Position += InputManager.MouseDelta;
			}
			else
			{
				e.Absorbed = false;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			//if( InputManager.AnyMouse )
			//{
			//	SceneManager.Set( typeof( Loading ).Name );
			//}

			uiManager.Update(gameTime);
			uiManager.AddLineBreak();
			uiManager.SetDebugValue("Pointer handled", InputManager.PointerHandled);

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
					cam.InitiateCursorMovement();
				}
				else if (InputManager.Held(MouseButton.Right) || InputManager.Held(MouseButton.Left))
				{
					cam.DoCursorMovement();
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			//GraphicsDevice.Clear(Color.Brown);
			uiManager.Draw(spriteBatch);
		}
	}
}