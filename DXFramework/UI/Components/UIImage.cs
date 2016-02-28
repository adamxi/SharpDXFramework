using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;

namespace DXFramework.UI
{
	public class UIImage : UIControl
	{
		private Texture2D texture;

		public UIImage(string resource)
		{
			DebugColor = Color.Red;
			Resource = resource;
		}

		public string Resource { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			if (!Resource.Equals(texture?.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				texture = Engine.Content.Load<Texture2D>(Resource);
				Size = new Vector2(texture.Width, texture.Height);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			RectangleF clip;
			RectangleF destRect = Bounds;
			Rectangle? sourceRect = null;

			if (HasClip(out clip))
			{
				RectangleF inter = RectangleF.Empty;
				inter.Left = Math.Max(clip.Left, destRect.Left);
				inter.Top = Math.Max(clip.Top, destRect.Top);
				inter.Right = Math.Min(clip.Right, destRect.Right);
				inter.Bottom = Math.Min(clip.Bottom, destRect.Bottom);

				float leftClip = inter.Left - DrawPosition.X;
				float rightClip = inter.Right - (DrawPosition.X + Width);
				float topClip = inter.Top - DrawPosition.Y;
				float bottomClip = inter.Bottom - (DrawPosition.Y + Height);
				float clippedWidth = rightClip - leftClip;
				float clippedHeight = bottomClip - topClip;

				Rectangle source = Rectangle.Empty;
				source.X = (int)leftClip;
				source.Y = (int)topClip;
				source.Width = (int)(Width + clippedWidth);
				source.Height = (int)(Height + clippedHeight);
				sourceRect = source;

				destRect.X += leftClip;
				destRect.Y += topClip;
				destRect.Width = Math.Max(destRect.Width + clippedWidth, 0);
				destRect.Height = Math.Max(destRect.Height + clippedHeight, 0);

				if (UIManager.DrawDebug)
				{
					PRect rect = new PRect(clip, 1);
					rect.Color = Color.Yellow;
					rect.Draw();

					rect = new PRect(inter, 1);
					rect.Color = Color.Magenta;
					rect.Draw();
				}

				//int rC = (int)(  inter.Right - ( DrawPosition.X + Width ) - location.X + location.X);
				//int lC = (int)( location.X + inter.Left - DrawPosition.X - location.X );
				//int bC = (int)( location.Y + inter.Bottom - ( DrawPosition.Y + Height ) - location.Y );
				//int tC = (int)( location.Y + inter.Top - DrawPosition.Y - location.Y );
			}

			spriteBatch.Draw(texture, destRect, sourceRect, Color, 0f, Vector2.Zero, SpriteEffect, LayerDepth);
		}

		public override string ToString()
		{
			return base.ToString() + " Image: " + texture.Name;
		}
	}
}