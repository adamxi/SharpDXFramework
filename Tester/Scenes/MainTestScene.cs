using DXFramework.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using DXFramework.UI;
using DXFramework.Util;
using SharpDX;
using DXFramework.SceneManagement.Transitions;

namespace XManager.Scenes
{
	public class MainTestScene : TestScene
	{
		private Camera cam;
		private UIManager uiManager;

		public override void LoadContent()
		{
			base.LoadContent();

			cam = new Camera(GraphicsDevice);
			uiManager = new UIManager();
			//uiManager.EnableProfilling = true;

			var menuPanel = new UIPanel();
			menuPanel.AddConstraint(Edge.CenterXY, null, Edge.CenterXY);
			menuPanel.Alpha = 0f;

			var testScenesButton = GetMenuEntry(menuPanel, "UI Test Scenes");
			testScenesButton.InputReleased += testScenes;
			menuPanel.AddChild(testScenesButton);

			var transitionScenesButton = GetMenuEntry(menuPanel, "Transition Test Scenes");
			transitionScenesButton.InputReleased += TransitionScenesButton_InputReleased;
			menuPanel.AddChild(transitionScenesButton);

			var exitButton = GetMenuEntry(menuPanel, "Exit");
			exitButton.InputReleased += ExitButton_InputReleased;
			menuPanel.AddChild(exitButton);

			uiManager.Add(menuPanel);
		}

		private void ExitButton_InputReleased(object sender, DXFramework.MouseEventArgs e)
		{
			this.game.Exit();
		}

		private void TransitionScenesButton_InputReleased(object sender, DXFramework.MouseEventArgs e)
		{
			sceneManager.Set<TransitionTestSceneMain>(new TransitionFade());
		}

		private void testScenes(object sender, DXFramework.MouseEventArgs e)
		{
			sceneManager.Set<UITestSceneMain>(new TransitionFade());
		}

		private UIControl previousEntry;
		private UIControl GetMenuEntry(UIControl parent, string label)
		{
			var btn = new UIButton();
			btn.Tag = label;
			btn.AddDecoration(new UILabel(label));

			btn.AddConstraint(Edge.CenterX, parent, Edge.CenterX, ConstraintCategory.All);

			if (previousEntry != null)
			{
				btn.AddConstraint(Edge.Top, previousEntry, Edge.Bottom, -20, ConstraintCategory.All);
			}

			previousEntry = btn;
			return btn;
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			uiManager.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			uiManager.Update(gameTime);
		}
	}
}
