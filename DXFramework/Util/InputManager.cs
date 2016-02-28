using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace DXFramework
{
	public static class InputManager
	{
		[DllImport("user32.dll")]
		private static extern bool ClipCursor(ClipRect region);

		private static Game game;
		private static GraphicsDevice graphicsDevice;
		private static KeyboardManager keyboard;
		private static KeyboardState keyboardState;
		private static KeyboardState oldKeyboardState;
		private static MouseManager mouse;
		private static MouseState mouseState;
		private static MouseState oldMouseState;
		private static Vector2 mousePosition;

		private static bool keyboardEnabled;
		private static bool mouseEnabled;
		private static bool touchEnabled;
		private static bool forceMouseRelease;
		private static bool isClipping;

		public static void Initialize(Game game, bool keyboardEnabled = true, bool mouseEnabled = true, bool touchEnabled = false)
		{
			InputManager.game = game;
			InputManager.graphicsDevice = game.GraphicsDevice;
			keyboard = new KeyboardManager(game);
			mouse = new MouseManager(game);
			InputManager.keyboardEnabled = keyboardEnabled;
			InputManager.mouseEnabled = mouseEnabled;
			InputManager.touchEnabled = touchEnabled;
			ClipWhileDragging = false;
		}

		#region Properties
		public static bool ClipWhileDragging { get; set; }

		public static bool AnyKeyboard { get; private set; }

		public static bool AnyTouch { get; private set; }

		/// <summary>
		/// Mouse position.
		/// </summary>
		public static Vector2 MousePosition
		{
			get { return mousePosition; }
		}

		private static Vector2 oldMousePosition;

		public static Vector2 MouseDelta { get; private set; }

		public static bool MouseMoved
		{
			get { return MouseDelta != Vector2.Zero; }
		}

		public static int MouseWheelDelta
		{
			get { return mouseState.WheelDelta; }
		}

		public static MouseButton MousePressed { get; private set; }

		public static MouseButton MouseHeld { get; private set; }

		public static MouseButton MouseReleased { get; private set; }

		public static bool AnyMousePressed
		{
			get { return MousePressed != MouseButton.None; }
		}

		public static bool AnyMouseHeld
		{
			get { return MouseHeld != MouseButton.None; }
		}

		public static bool AnyMouseReleased
		{
			get { return MouseReleased != MouseButton.None; }
		}

		public static bool PointerHandled { get; set; }

		private static bool lockMouseToCenter;
		public static bool LockMouseToCenter
		{
			get { return lockMouseToCenter; }
			set
			{
				lockMouseToCenter = value;
				if (lockMouseToCenter)
				{
					mouse.SetPosition(new Vector2(0.5f));
					mousePosition.X = 0.5f * Engine.ScreenSize.X;
					mousePosition.Y = 0.5f * Engine.ScreenSize.Y;
					oldMousePosition = mousePosition;
					MouseDelta = Vector2.Zero;
				}
			}
		}
		#endregion

		public static void Update()
		{
			if (keyboardEnabled)
			{
				UpdateKeyboard();
			}
			if (mouseEnabled)
			{
				UpdateMouse();
			}
		}

		private static void UpdateKeyboard()
		{
			oldKeyboardState = keyboardState;
			keyboardState = keyboard.GetState();
		}

		private static void UpdateMouse()
		{
			PointerHandled = false;
			oldMouseState = mouseState;
			mouseState = mouse.GetState();

			oldMousePosition = mousePosition;
			mousePosition.X = mouseState.X * Engine.ScreenSize.X;
			mousePosition.Y = mouseState.Y * Engine.ScreenSize.Y;
			MouseDelta = mousePosition - oldMousePosition;

			if (LockMouseToCenter)
			{
				mouse.SetPosition(new Vector2(0.5f));
				mousePosition.X = 0.5f * Engine.ScreenSize.X;
				mousePosition.Y = 0.5f * Engine.ScreenSize.Y;
			}

			CheckDragClip();
			UpdateMouseButtonStates();

			if (forceMouseRelease && !AnyMouseHeld)
			{
				ResetMouse();
				forceMouseRelease = false;
				return;
			}
		}

		public static void SetMouse(Vector2 pos)
		{
			mouse.SetPosition(pos);
		}

		private static void CheckDragClip()
		{
			if (ClipWhileDragging)
			{
				if (isClipping && !AnyMouseHeld)
				{
					ClearMouseClip();
				}
				else if (!isClipping && AnyMouseHeld)
				{
					System.Windows.Forms.Control control = game.Window.NativeWindow as System.Windows.Forms.Control;
					System.Drawing.Size diff = control.Size - control.ClientSize;

					Rectangle region = Rectangle.Empty;
					region.X = control.Left + diff.Width - control.Margin.Left;
					region.Y = control.Top + diff.Height - control.Margin.Top;
					region.Width = control.ClientSize.Width;
					region.Height = control.ClientSize.Height;

					ClipMouse(region);
				}
			}
		}

		/// <summary>
		/// Clips the mouse to a given region.
		/// Remember to clear clip with <see cref="ClearMouseClip"/>.
		/// </summary>
		/// <param name="region">Region to clip to.</param>
		public static void ClipMouse(Rectangle region)
		{
			ClipCursor(new ClipRect(ref region));
			isClipping = true;
		}

		/// <summary>
		/// Clears any mouse clip set by <see cref="ClipMouse"/>.
		/// </summary>
		public static void ClearMouseClip()
		{
			if (isClipping)
			{
				ClipCursor(null);
				isClipping = false;
			}
		}

		/// <summary>
		/// Releases all mouse buttons in the <see cref="InputManager"/>.
		/// </summary>
		public static void ReleaseMouse()
		{
			forceMouseRelease = true;
		}

		private static void ResetMouse()
		{
			MouseHeld = MouseButton.None;
			MousePressed = MouseButton.None;
			MouseReleased = MouseButton.None;
		}

		public static bool Held(params Keys[] keys)
		{
			for (int i = keys.Length; --i >= 0;)
			{
				if (!keyboardState.IsKeyDown(keys[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool Pressed(params Keys[] keys)
		{
			for (int i = keys.Length; --i >= 0;)
			{
				if (!(keyboardState.IsKeyDown(keys[i]) && !oldKeyboardState.IsKeyDown(keys[i])))
				{
					return false;
				}
			}
			return true;
		}

		public static bool Released(params Keys[] keys)
		{
			for (int i = keys.Length; --i >= 0;)
			{
				if (!(keyboardState.IsKeyDown(keys[i]) && !oldKeyboardState.IsKeyDown(keys[i])))
				{
					return false;
				}
			}
			return true;
		}

		public static bool Held(params MouseButton[] buttons)
		{
			if (forceMouseRelease)
			{
				return false;
			}
			for (int i = buttons.Length; --i >= 0;)
			{
				if (!mouseState.GetButtonState(buttons[i]).Down)
				{
					return false;
				}
			}
			return true;
		}

		public static bool Pressed(params MouseButton[] buttons)
		{
			if (forceMouseRelease)
			{
				return false;
			}
			for (int i = buttons.Length; --i >= 0;)
			{
				if (!(mouseState.GetButtonState(buttons[i]).Pressed && !oldMouseState.GetButtonState(buttons[i]).Released))
				{
					return false;
				}
			}
			return true;
		}

		public static bool Released(params MouseButton[] buttons)
		{
			if (forceMouseRelease)
			{
				return false;
			}
			for (int i = buttons.Length; --i >= 0;)
			{
				if (!(mouseState.GetButtonState(buttons[i]).Released && !oldMouseState.GetButtonState(buttons[i]).Pressed))
				{
					return false;
				}
			}
			return true;
		}

		private static ButtonState GetButtonState(this MouseState ms, MouseButton button)
		{
			switch (button)
			{
				case MouseButton.None:
					return new ButtonState(ButtonStateFlags.None);
				case MouseButton.Left:
					return ms.LeftButton;
				case MouseButton.Right:
					return ms.RightButton;
				case MouseButton.Middle:
					return ms.MiddleButton;
				case MouseButton.XButton1:
					return ms.XButton1;
				case MouseButton.XButton2:
					return ms.XButton2;
				default:
					return new ButtonState(ButtonStateFlags.None);
			}
		}

		private static MouseButton GetButtonByFlags(this MouseState ms, ButtonStateFlags flags)
		{
			if (ms.LeftButton.Flags == flags)
			{
				return MouseButton.Left;
			}
			else if (ms.RightButton.Flags == flags)
			{
				return MouseButton.Right;
			}
			else if (ms.MiddleButton.Flags == flags)
			{
				return MouseButton.Middle;
			}
			else if (ms.XButton1.Flags == flags)
			{
				return MouseButton.XButton1;
			}
			else if (ms.XButton2.Flags == flags)
			{
				return MouseButton.XButton2;
			}
			return MouseButton.None;
		}

		private static MouseButton[] AllButtons = { MouseButton.Left, MouseButton.Middle, MouseButton.Right, MouseButton.XButton1, MouseButton.XButton2 };
		private static int allButtonCount = AllButtons.Length;

		private static void UpdateMouseButtonStates()
		{
			ResetMouse();

			for (int i = allButtonCount; --i >= 0;)
			{
				if (Held(AllButtons[i]))
				{
					MouseHeld = AllButtons[i];
					//Console.WriteLine( "MouseDown: " + MouseDown );
				}
				if (Pressed(AllButtons[i]))
				{
					MousePressed = AllButtons[i];
					//Console.WriteLine( "MousePressed: " + MousePressed );
				}
				if (Released(AllButtons[i]))
				{
					MouseReleased = AllButtons[i];
					//Console.WriteLine( "MouseReleased: " + MouseReleased );
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private class ClipRect
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public ClipRect(ref Rectangle r)
			{
				this.left = r.X;
				this.top = r.Y;
				this.right = r.Right;
				this.bottom = r.Bottom;
			}
		}
	}

	public enum MouseButton
	{
		None,
		Left,
		Right,
		Middle,
		XButton1,
		XButton2
	}

	public class MouseEventArgs : EventArgs
	{
		public MouseEventArgs(MouseButton button, Vector2 position)
		{
			this.Button = button;
			this.ClientPosition = position;
			this.Absorbed = true;
		}

		public MouseButton Button { get; private set; }

		public Vector2 ClientPosition { get; private set; }

		public bool Absorbed { get; set; }

		public override string ToString()
		{
			return Button.ToString() + ": " + ClientPosition.ToString();
		}
	}
}