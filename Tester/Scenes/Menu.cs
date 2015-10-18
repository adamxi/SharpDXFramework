using DXFramework.SceneManagement;
using DXFramework.UI;
using SharpDX;
using SharpDX.Toolkit;

namespace XManager.Scenes
{
	public class Menu : Scene
	{
		private UIManager uiManager;

		public Menu()
		{
			//UIManager.DrawDebug = true;
			uiManager = new UIManager();
			uiManager.EnableProfilling = true;

			UITweenPanel menuPanel = new UITweenPanel();
			menuPanel.Alpha = 0f;
			menuPanel.AddConstraint( Edge.CenterXY, null, Edge.CenterXY );
			menuPanel.AddChild( GetMenuEntry( menuPanel, "New Game" ) );
			menuPanel.AddChild( GetMenuEntry( menuPanel, "Continue" ) );
			menuPanel.AddChild( GetMenuEntry( menuPanel, "Options" ) );
			menuPanel.AddChild( GetMenuEntry( menuPanel, "Help" ) );
			menuPanel.AddChild( GetMenuEntry( menuPanel, "Exit" ) );

			uiManager.Add( menuPanel );
		}

		private UIControl previousEntry;
		private UIControl GetMenuEntry( UIControl parent, string label )
		{
			var control = new UIButton();
			control.InputReleased += control_InputPressed;
			//var uiImg = new UIImage( "graphics/arrow_down" );
			//uiImg.AddConstraint( Edge.CenterXY, control, Edge.CenterXY );
			//control.AddDecoration( uiImg );

			var uiLabel = new UILabel( label );
			uiLabel.AddConstraint( Edge.CenterXY, control, Edge.CenterXY );
			control.AddDecoration( uiLabel );

			control.AddConstraint( Edge.CenterX, parent, Edge.CenterX );

			if( previousEntry == null )
			{
				control.AddConstraint( Edge.CenterY, parent, Edge.CenterY, 0, ConstraintCategory.Initialization );
			}
			else
			{
				control.AddConstraint( Edge.Top, previousEntry, Edge.Bottom, -20, ConstraintCategory.Initialization );
			}

			previousEntry = control;
			return control;
		}

		void control_InputPressed( object sender, DXFramework.MouseEventArgs e )
		{
			UIControl c = sender as UIControl;
			System.Console.WriteLine( "Clicked: " + c.GetDecoration( 0 ).ToString() );
		}

		public override void Update( GameTime gameTime )
		{
			uiManager.Update( gameTime );
		}

		public override void Draw( GameTime gameTime )
		{
			graphicsDevice.Clear( Color.OliveDrab );

			uiManager.Draw( spriteBatch );
		}
	}
}