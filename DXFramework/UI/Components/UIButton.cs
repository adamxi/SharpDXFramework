using DXFramework.Tweening;
using SharpDX;
using SharpDX.Toolkit;

namespace DXFramework.UI
{
	public class UIButton : UIControl
	{
		private Tweener tweener;
		private Vector2 orgScale;

		public UIButton()
		{
			DebugColor = Color.Blue;
			Color = Color.Black;
			AbsorbPointer = true;
			DrawBounds = true;
			Alpha = 0f;

			InputEnter += UIButton_InputEnter;
			InputLeave += UIButton_InputLeave;
			InputDown += UIButton_InputDown;
			InputReleased += UIButton_InputReleased;
		}

		public override T AddDecoration<T>(T decoration)
		{
			if (decoration == null)
			{
				return default(T);
			}
			T deco = base.AddDecoration<T>(decoration);
			Size = Vector2.Max(Size, deco.Size);
			return deco;
		}

		void UIButton_InputDown(object sender, MouseEventArgs e)
		{
			if (IntersectsPointer())
			{
				Color = Color.Red;
			}
		}

		void UIButton_InputReleased(object sender, MouseEventArgs e)
		{
			Color = Color.Black;
		}

		void UIButton_InputEnter(object sender, MouseEventArgs e)
		{
			this.Alpha = 0.3f;
			if (tweener == null || tweener.Done)
			{
				orgScale = Scale;
			}
			tweener = new Tweener(Scale, orgScale * 1.20f, 0.15f, new Linear.EaseNone());
		}

		void UIButton_InputLeave(object sender, MouseEventArgs e)
		{
			this.Alpha = 0f;
			Color = Color.Black;
			tweener = new Tweener(Scale, orgScale, 0.1f, new Linear.EaseNone());
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (tweener != null)
			{
				tweener.Update(gameTime);
				SetScale(tweener.Value, true);
			}
		}
	}
}