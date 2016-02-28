using DXFramework.Util;
using DXPrimitiveFramework;
using SharpDX.Collections;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System.Linq;

namespace DXFramework.UI
{
	public class UIManager : ObservableCollection<UIControl>
	{
		/// <summary>
		/// True to draw UI debugging information.
		/// </summary>
		public static bool DrawDebug { get; set; }
		public static Profiler layoutProfiler;

		private bool layoutDone;
#if DEBUG
		private Profiler updateProfiler;
		private Profiler drawProfiler;
		private UIDebugPanel debugPanel;
#endif

		public UIManager()
		{
			this.ItemAdded += UIRenderer_ItemAdded;
			this.ItemRemoved += UIRenderer_ItemRemoved;

#if DEBUG
			debugPanel = new UIDebugPanel();
			//debugPanel.Size = new Vector2(460, 150);
			//debugPanel.AddLineBreak();
			debugPanel.SetDebugValue("Primitives");
			debugPanel.AddLineBreak();
			debugPanel.SetDebugValue("Game time");
			debugPanel.SetDebugValue("  Update");
			debugPanel.AddLineBreak();
			debugPanel.SetDebugValue("UI time");
			debugPanel.SetDebugValue("  Draw");
			debugPanel.SetDebugValue("  Layout");
			//debugPanel.DoLayout();
#endif
		}

		public bool EnableProfilling { get; set; }

		void UIRenderer_ItemRemoved(object sender, ObservableCollectionEventArgs<UIControl> e)
		{
			layoutDone = false;
		}

		void UIRenderer_ItemAdded(object sender, ObservableCollectionEventArgs<UIControl> e)
		{
			layoutDone = false;
			e.Item.CheckInitialize();
		}

		public void SetDebugValue(string key, object value)
		{
#if DEBUG
			if (EnableProfilling)
			{
				debugPanel.SetDebugValue(key, value);
			}
#endif
		}

		public void SetStaticLabel(string labelText)
		{
#if DEBUG
			if (EnableProfilling)
			{
				debugPanel.SetStaticLabel(labelText);
			}
#endif
		}

		public void AddLineBreak(int space = 10)
		{
#if DEBUG
			if (EnableProfilling)
			{
				debugPanel.AddLineBreak(space);
			}
#endif
		}

		public void DoLayout()
		{
#if DEBUG
			//if( EnableProfilling )
			//{
			//	layoutProfiler.Start();
			//}
#endif
			foreach (UIControl control in this)
			{
				control.DoLayout();
			}
			layoutDone = true;

#if DEBUG
			//if( EnableProfilling )
			//{
			//layoutProfiler.Stop();
			//debugPanel.SetDebugValue( "  UI Layout:", layoutProfiler.FullOutput() );
			//}
#endif
		}

		public void Update(GameTime gameTime)
		{
			if (!layoutDone)
			{
				return;
			}
#if DEBUG
			if (EnableProfilling)
			{
				updateProfiler.Start();
			}
#endif
			for (int i = Count; --i >= 0;)
			{
				UIControl control = this[i];
				if (control.Enabled && control.Visible)
				{
					control.Update(gameTime);
				}
			}

			if (UIControl.Disposed.Count > 0)
			{
				UIControl.Disposed.ForEach(c => Remove(c));
				UIControl.Disposed.Clear();
			}

#if DEBUG
			if (EnableProfilling)
			{
				updateProfiler.Stop();
				debugPanel.SetDebugValue("Game time", gameTime.TotalGameTime.TotalSeconds.ToString("0.00"));
				debugPanel.SetDebugValue("  Update", updateProfiler.FullOutput(2, 4));
			}
#endif
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!layoutDone)
			{
				DoLayout();
			}
#if DEBUG
			if (EnableProfilling)
			{
				drawProfiler.Start();
			}
#endif
			spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendStates.NonPremultiplied);
			PrimitiveBatch.Begin();

			foreach (UIControl control in this)
			{
				if (control.Enabled && control.Visible)
				{
					control.Draw(spriteBatch);
				}
			}

			PrimitiveBatch.End();
			spriteBatch.End();
#if DEBUG
			if (EnableProfilling)
			{

				drawProfiler.Stop();
				debugPanel.SetDebugValue("UI time", (drawProfiler.TotalElapsed + updateProfiler.TotalElapsed + layoutProfiler.TotalElapsed).ToString("0.00"));
				debugPanel.SetDebugValue("  Draw", drawProfiler.FullOutput(2, 4));
				debugPanel.SetDebugValue("  Layout", layoutProfiler.FullOutput());
				debugPanel.SetDebugValue("Primitives", PrimitiveBatch.DrawCount.ToString());

				spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendStates.NonPremultiplied);
				PrimitiveBatch.Begin();

				debugPanel.Draw(spriteBatch);

				PrimitiveBatch.End();
				spriteBatch.End();
			}
#endif
		}
	}
}