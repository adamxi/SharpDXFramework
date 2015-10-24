using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;

namespace DXPrimitiveFramework
{
	public class PRect : Primitive
	{
		protected float width;
		protected float height;

		public PRect(PRect rect)
			: base(rect)
		{
			this.width = rect.width;
			this.height = rect.height;
		}

		public PRect(RectangleF rect, bool filled) : this(rect.X, rect.Y, rect.Width, rect.Height, filled) { }

		public PRect(RectangleF rect, float thickness = 1f) : this(rect.X, rect.Y, rect.Width, rect.Height, thickness) { }

		public PRect(Rectangle rect, bool filled) : this(rect.X, rect.Y, rect.Width, rect.Height, filled) { }

		public PRect(Rectangle rect, float thickness = 1f) : this(rect.X, rect.Y, rect.Width, rect.Height, thickness) { }

		public PRect(float x, float y, float width, float height, bool filled) : this(new Vector2(x, y), width, height, filled) { }

		public PRect(float x, float y, float width, float height, float thickness = 1f) : this(new Vector2(x, y), width, height, thickness) { }

		public PRect(Vector2 position, float width, float height, bool filled)
			: base(filled)
		{
			this.position = position;
			this.width = Math.Max(width, 1);
			this.height = Math.Max(height, 1);
		}

		public PRect(Vector2 position, float width, float height, float thickness = 1f)
			: base(thickness)
		{
			if (thickness >= width * 0.5f)
			{
				width = thickness + 1;
			}
			if (thickness >= height * 0.5f)
			{
				height = thickness + 1;
			}
			this.position = position;
			this.width = Math.Max(width, 1);
			this.height = Math.Max(height, 1);
		}

		public float Width
		{
			get { return width; }
			set
			{
				width = value;
				PrimitiveCreated = false;
			}
		}

		public float Height
		{
			get { return height; }
			set
			{
				height = value;
				PrimitiveCreated = false;
			}
		}

		internal override List<PolygonPoint> GetPoints(float thickness = 0)
		{
			List<PolygonPoint> points = new List<PolygonPoint>() {
				new PolygonPoint(thickness, thickness),
				new PolygonPoint(width - thickness, thickness),
				new PolygonPoint(width - thickness, height - thickness),
				new PolygonPoint(thickness, height - thickness),
			};

			if (!Filled)
			{
				points.Add(points[0]);
			}

			return points;
		}
	}
}