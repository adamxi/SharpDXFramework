using DXFramework.SceneManagement;
using DXFramework.SceneManagement.Transitions;
using DXFramework.UI;
using DXFramework.Util;
using SharpDX;
using SharpDX.Toolkit;

namespace XManager.Scenes
{
	public class TestScene : Scene
	{
		protected Camera cam;
		protected UIManager testSceneUIManager;

		public override void LoadContent()
		{
			var menuBar = new UIPanel();
			menuBar.AddConstraint(Edge.Top, null, Edge.Top, 10);
			menuBar.AddConstraint(Edge.Left, null, Edge.Left, 50);
			menuBar.Alpha = 0f;

			var homeItem = new UIButton();
			homeItem.AddDecoration(new UILabel("Home Menu"));
			homeItem.AddConstraint(Edge.Left, menuBar, Edge.Left);
			homeItem.AddConstraint(Edge.CenterY, menuBar, Edge.CenterY);
			homeItem.InputReleased += HomeItem_InputReleased;
			menuBar.AddChild(homeItem);

			var debugCheckbox = new UILabelCheckBox("Draw UI Borders:");
			debugCheckbox.AddConstraint(Edge.Left, homeItem, Edge.Right, -20);
			debugCheckbox.AddConstraint(Edge.CenterY, menuBar, Edge.CenterY);
			debugCheckbox.CheckBox.InputReleased += DebugCheckbox_InputReleased;
			debugCheckbox.CheckBox.Checked = UIManager.DrawDebug;
			menuBar.AddChild(debugCheckbox);

			testSceneUIManager = new UIManager();
			testSceneUIManager.Add(menuBar);
		}

		private void DebugCheckbox_InputReleased(object sender, DXFramework.MouseEventArgs e)
		{
			UIManager.DrawDebug = (sender as UICheckBox).Checked;
		}

		private void HomeItem_InputReleased(object sender, DXFramework.MouseEventArgs e)
		{
			sceneManager.Set<MainTestScene>(new TransitionFade());
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Brown);
			testSceneUIManager.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			testSceneUIManager.Update(gameTime);
		}
	}
}