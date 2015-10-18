using System;
using System.Runtime.Serialization;
using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.UI
{
	[DataContract]
	public class UIImage : UIControl
	{
		private Texture2D texture;

		public UIImage( string resource )
		{
			DebugColor = Color.Red;
			this.Resource = resource;
		}

		[DataMember]
		public string Resource { get; set; }

		protected override void Initialize()
		{
			base.Initialize();
			System.Diagnostics.Debug.Assert( texture == null, "Error: Texture already loaded." );
			this.texture = Engine.Content.Load<Texture2D>( Resource );
			this.Size = new Vector2( texture.Width, texture.Height );
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
			base.Draw( spriteBatch );

			RectangleF clip;
			RectangleF destRect = Bounds;
			Rectangle? sourceRect = null;

			if( HasClip( out clip ) )
			{
				RectangleF inter = RectangleF.Empty;
				inter.Left = Math.Max( clip.Left, destRect.Left );
				inter.Top = Math.Max( clip.Top, destRect.Top );
				inter.Right = Math.Min( clip.Right, destRect.Right );
				inter.Bottom = Math.Min( clip.Bottom, destRect.Bottom );

				int leftClip = (int)( inter.Left - DrawPosition.X );
				int rightClip = (int)( inter.Right - ( DrawPosition.X + Width ) );
				int topClip = (int)( inter.Top - DrawPosition.Y );
				int bottomClip = (int)( inter.Bottom - ( DrawPosition.Y + Height ) );
				int clippedWidth = rightClip - leftClip;
				int clippedHeight = bottomClip - topClip;

				Rectangle source = Rectangle.Empty;
				source.X = leftClip;
				source.Y = topClip;
				source.Width = (int)Width + clippedWidth;
				source.Height = (int)Height + clippedHeight;
				sourceRect = source;

				destRect.X += leftClip;
				destRect.Y += topClip;
				destRect.Width = Math.Max( destRect.Width + clippedWidth, 0 );
				destRect.Height = Math.Max( destRect.Height + clippedHeight, 0 );

				if( UIManager.DrawDebug )
				{
					PrimitiveBatch.Begin();
					PRect rect = new PRect( clip, 1 );
					rect.Color = Color.Yellow;
					PrimitiveBatch.Draw( rect );
					PrimitiveBatch.End();

					PrimitiveBatch.Begin();
					rect = new PRect( inter, 1 );
					rect.Color = Color.Magenta;
					PrimitiveBatch.Draw( rect );
					PrimitiveBatch.End();
				}

				//int rC = (int)(  inter.Right - ( DrawPosition.X + Width ) - location.X + location.X);
				//int lC = (int)( location.X + inter.Left - DrawPosition.X - location.X );
				//int bC = (int)( location.Y + inter.Bottom - ( DrawPosition.Y + Height ) - location.Y );
				//int tC = (int)( location.Y + inter.Top - DrawPosition.Y - location.Y );
			}

			spriteBatch.Draw( texture, destRect, sourceRect, Color, 0f, Vector2.Zero, SpriteEffect, LayerDepth );
		}

		public override string ToString()
		{
			return base.ToString() + " Image: " + texture.Name;
		}
	}
}