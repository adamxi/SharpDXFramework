using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WinformXNA.Properties;

namespace WinformXNA {
	public class XNAPanel: GraphicsDeviceControl {
		protected Dictionary<string, object> debugObjects;
		protected Dictionary<string, float> debugValues;
		protected List<string> debugStrings;

		public XNAPanel() {
		}

		public FPSCounter FPSCounter { get; private set; }
		public SpriteFont DebugFont { get; private set; }
		public Vector2 DebugPos { get; set; }
		public Color DebugColor { get; set; }

		protected override void Initialize() {
			debugObjects = new Dictionary<string, object>();
			debugValues = new Dictionary<string, float>();
			debugStrings = new List<string>();

			ContentManager content = new ResourceContentManager(services, Resource.ResourceManager);
			DebugFont = content.Load<SpriteFont>("DebugFont");
			DebugPos = new Vector2(15, 15);
			DebugColor = Color.Yellow;

			FPSCounter = new FPSCounter(DebugPos, DebugFont, DebugColor);
		}

		protected override void DebugDraw() {
			spriteBatch.Begin();
			FPSCounter.Update(spriteBatch);

			Vector2 pos = DebugPos;
			Vector2 shadePos = pos + Vector2.One;

			if(debugObjects.Count > 0) {
				List<string> keys = debugObjects.Keys.ToList();
				foreach(string key in keys) {
					string drawString = key + ": " + debugObjects[key].ToString();
					pos.Y += 15;
					shadePos.Y += 15;

					spriteBatch.DrawString(DebugFont, drawString, shadePos, Color.Black);
					spriteBatch.DrawString(DebugFont, drawString, pos, DebugColor);
				}
			}

			if(debugValues.Count > 0) {
				List<string> keys = debugValues.Keys.ToList();
				foreach(string key in keys) {
					string drawString = key + ": " + debugValues[key].ToString();
					pos.Y += 15;
					shadePos.Y += 15;

					spriteBatch.DrawString(DebugFont, drawString, shadePos, Color.Black);
					spriteBatch.DrawString(DebugFont, drawString, pos, DebugColor);
				}
			}

			if(debugStrings.Count > 0) {
				foreach(string text in debugStrings) {
					pos.Y += 15;
					shadePos.Y += 15;

					spriteBatch.DrawString(DebugFont, text, shadePos, Color.Black);
					spriteBatch.DrawString(DebugFont, text, pos, DebugColor);
				}
			}

			spriteBatch.End();
		}

		public void AddDebugString(string text) {
			debugStrings.Add(text);
		}

		public void AddDebugValue(string name) {
			debugValues.Add(name, 0);
		}

		public void AddDebugObject(string name) {
			debugObjects.Add(name, string.Empty);
		}

		public void SetDebugValue(string name, float value) {
			if(debugValues.ContainsKey(name)) {
				debugValues[name] = value;
			} else {
				debugValues.Add(name, value);
			}
		}

		public void SetDebugObject(string name, object value) {
			if(debugObjects.ContainsKey(name)) {
				debugObjects[name] = value;
			} else {
				debugObjects.Add(name, value);
			}
		}

		public void IncrementDebugStat(string name, int increment) {
			if(debugValues.ContainsKey(name)) {
				debugValues[name] += increment;
			}
		}
	}
}