using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;

namespace DXFramework.UI
{
	public class UILabel : UIControl
	{
		private static string[] splitChar = new string[] { " " };

		private SpriteFont font;
		private float lineHeight;
		private string text;
		private string[] lines;
		private bool multiLine;

		public UILabel() : this(null, false) { }

		public UILabel(bool multiLine) : this(null, multiLine) { }

		public UILabel(string text, bool multiLine = false)
		{
			this.text = text;
			this.multiLine = multiLine;
			AutoSize = true;
			Color = Color.Green;
			Font = Engine.DefaultFont;
			SetText(text);
		}

		public override void Initialize()
		{
			base.Initialize();
			SetText(text);
		}

		public override void DoLayout(ConstraintCategory category = ConstraintCategory.All)
		{
			base.DoLayout(category);
			if (multiLine)
			{
				SplitText(base.Size.X);
			}
		}

		public SpriteFont Font
		{
			get { return font; }
			set
			{
				font = value;
				lineHeight = font.MeasureString(" ").Y + LineSpacing;
			}
		}

		public bool AutoSize { get; set; }

		public int LineSpacing { get; set; }

		public bool MultiLine
		{
			get { return multiLine; }
			set
			{
				multiLine = value;
				if (multiLine)
				{
					SplitText(size.X);
				}
			}
		}

		public override Vector2 Size
		{
			get { return base.Size; }
			set
			{
				if (base.Size != value)
				{
					if (multiLine)
					{
						SplitText(value.X);
					}
					else
					{
						base.Size = value;
					}
				}
			}
		}

		public void ClearText()
		{
			SetText(null);
		}

		/// <summary>
		/// Sets the label text. Returns true if the size has changed.
		/// </summary>
		public bool SetText(string text)
		{
			this.text = text;

			var oldSize = size;
			if (text == null)
			{
				if (AutoSize)
				{
					base.Size = Vector2.Zero;
				}
			}
			else
			{
				if (multiLine)
				{
					SplitText(size.X);
				}
				else if (AutoSize)
				{
					base.Size = font.MeasureString(text);
				}
			}

			return oldSize != size;
		}

		private void SplitText(float width)
		{
			if (width > 0)
			{
				lines = SplitIntoLines(width);
				base.Size = new Vector2(width, lineHeight * lines.Length);
			}
		}

		private string WarpText(int width, int margin)
		{
			string text = this.text;
			int len = text.Length;

			width = width - (margin * 2);
			for (int i = len + 1; --i >= 0;)
			{
				text = text.Substring(0, i);
				Vector2 dim = font.MeasureString(text);

				if (dim.X < width)
				{
					break;
				}
			}

			return text;
		}

		private string[] SplitIntoLines(float maxWidth, int margin = 0)
		{
			if (text == null || text.Length == 0)
			{
				size = Vector2.Zero;
				return new string[0];
			}

			string[] words = text.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
			List<string> lines = new List<string>();
			float lineWidth = 0;
			string line = string.Empty;
			margin *= 2;

			foreach (string word in words)
			{
				var subWord = word;
				int index = subWord.IndexOf("\r\n");            // Check word for "return to carriage + newline".
				while (index > -1)
				{
					var w = subWord.Substring(0, index);		// Get the first subword up until the linebreak.
					subWord = subWord.Substring(index + 2);		// Remove the linebreak from the subword.

					if (w.Length > 0)
					{
						AddWordToLine(w, ref line, ref lineWidth, margin, maxWidth, lines);
					}

					AddNewLine(lines, ref lineWidth, ref line);
					index = subWord.IndexOf("\r\n");
				}

				AddWordToLine(subWord, ref line, ref lineWidth, margin, maxWidth, lines);
			}
			if (line != string.Empty)
			{
				lines.Add(line);
			}
			return lines.ToArray();
		}

		private void AddWordToLine(string word, ref string line, ref float lineWidth, int margin, float maxWidth, List<string> lines)
		{
			float wordWidth = font.MeasureString(word + " ").X;

			if (lineWidth + wordWidth + margin > maxWidth)
			{
				AddNewLine(lines, ref lineWidth, ref line);
			}

			line = string.Concat(line, word + " ");
			lineWidth += wordWidth;
		}

		private void AddNewLine(List<string> lines, ref float lineWidth, ref string line)
		{
			lines.Add(line);
			lineWidth = 0;
			line = string.Empty;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (text == null)
			{
				return;
			}

			base.Draw(spriteBatch);

			if (multiLine)
			{
				if (lines == null)
				{
					return;
				}
				RectangleF clip;
				if (HasClip(out clip))
				{
					//RectangleF r = Bounds;
					//int left = (int)(DrawPosition.X - clip.Left);
					int top = (int)(DrawPosition.Y - clip.Top);
					//int width = (int)(clip.Right - DrawPosition.X);
					//int height = (int)(clip.Bottom - DrawPosition.Y);
					//System.Console.WriteLine( left + " " + top + " " + width + " " + height );
					//int totalHeight = (int)(lines.Length * lineHeight);
					int startLine = 0;
					int endLine = lines.Length;

					Vector2 pos = DrawPosition;
					if (top < 0)
					{
						startLine = (int)(-top / lineHeight + 1);
						pos.Y += startLine * lineHeight;
					}

					for (int i = startLine; i < endLine; i++)
					{
						if (pos.Y > clip.Bottom - lineHeight + 2)
						{
							break;
						}
						spriteBatch.DrawString(font, lines[i], pos, Color, 0f, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth);
						pos.Y += lineHeight;
					}
				}
				else
				{
					Vector2 pos = DrawPosition;
					foreach (string line in lines)
					{
						spriteBatch.DrawString(font, line, pos, Color, 0f, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth);
						pos.Y += lineHeight;
					}
				}
			}
			else
			{
				spriteBatch.DrawString(font, text, DrawPosition + Origin, Color, 0f, Origin, Scale, SpriteEffects.None, LayerDepth);
			}
		}

		public override string ToString()
		{
			return base.ToString() + " | Text: " + text;
		}
	}
}