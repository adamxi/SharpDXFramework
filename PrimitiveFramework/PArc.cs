using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;
using MathUtil = SharpDX.MathUtil;

namespace DXPrimitiveFramework
{
	public class PArc : Primitive
	{
		private const int DEFAULT_SIDES = 32;
		protected float radius;
		protected float arcDegrees;
		protected int sides;

		public PArc(PArc arc) : base(arc)
		{
			this.radius = arc.radius;
			this.arcDegrees = arc.degrees;
			this.sides = arc.sides;
		}

		public PArc(float x, float y, float radius, float degrees, bool filled) : this(x, y, radius, degrees, DEFAULT_SIDES, filled) { }

		public PArc(float x, float y, float radius, float degrees, uint thickness) : this(x, y, radius, degrees, DEFAULT_SIDES, thickness) { }

		public PArc(float x, float y, float radius, float degrees, int sides, bool filled) : this(new Vector2(x, y), radius, degrees, sides, filled) { }

		public PArc(float x, float y, float radius, float degrees, int sides, uint thickness) : this(new Vector2(x, y), radius, degrees, sides, thickness) { }

		public PArc(Vector2 position, float radius, float degrees, bool filled) : this(position, radius, degrees, DEFAULT_SIDES, filled) { }

		public PArc(Vector2 position, float radius, float degrees, uint thickness) : this(position, radius, degrees, DEFAULT_SIDES, thickness) { }

		public PArc(Vector2 position, float radius, float degrees, int sides, bool filled) : base(filled)
		{
			this.position = position;
			this.radius = radius;
			this.sides = sides;
			this.arcDegrees = degrees;
		}

		public PArc(Vector2 position, float radius, float degrees, int sides, uint thickness) : base(thickness)
		{
			if (thickness >= radius)
			{
				throw new ArgumentException("Arc thickness cannot be greater than radius - 1 (Current max thickness: " + (radius - 1) + ").");
			}
			this.position = position;
			this.radius = radius;
			this.sides = sides;
			this.arcDegrees = degrees;
		}

		public float Radius
		{
			get { return radius; }
			set
			{
				radius = value;
				PrimitiveCreated = false;
			}
		}

		public int Sides
		{
			get { return sides; }
			set
			{
				sides = value;
				PrimitiveCreated = false;
			}
		}

		internal override List<PolygonPoint> GetPoints(float thickness = 0)
		{
			List<PolygonPoint> points = new List<PolygonPoint>();

			int arcSides = (int)Math.Round((arcDegrees / 360f) * sides, MidpointRounding.AwayFromZero);
			int halfArcSides = arcSides / 2;
			float degreeStep = arcDegrees / arcSides;
			float r = radius - thickness;

			//points.Add(new PolygonPoint(0 + thickness, 0 + thickness));
			points.Add(GetPoint(degreeStep * -halfArcSides, thickness));

			for (int i = -halfArcSides; i <= halfArcSides; i++)
			{
				//float radAngle = MathUtil.DegreesToRadians(degreeStep * i) - MathUtil.PiOverTwo;
				//double x = Math.Cos(radAngle) * r;
				//double y = Math.Sin(radAngle) * r;

				//points.Add(new PolygonPoint(x, y));

				points.Add(GetPoint(degreeStep * i, r));
            }

			if (!Filled)
			{
				points.Add(points[0]);
			}

			return points;
		}

		private PolygonPoint GetPoint(float degrees, float radius)
		{
			float radAngle = MathUtil.DegreesToRadians(degrees) - MathUtil.PiOverTwo;
			double x = Math.Cos(radAngle) * radius;
			double y = Math.Sin(radAngle) * radius;

			return new PolygonPoint(x, y);
		}


		//public override bool Intersects(float x, float y) {
		//    float distX = x - position.X;
		//    float distY = y - position.Y;
		//    float radius = this.radius * scale.X;
		//    float distSquared = (distX * distX) + (distY * distY);
		//    float radiusSquared = radius * radius;

		//    if(thickness > 1) {
		//        float innerRadiusSquared = (radius - thickness) * (radius - thickness);

		//        if(distSquared <= radiusSquared && distSquared > innerRadiusSquared) {
		//            return true;
		//        }
		//    } else if(distSquared <= radiusSquared) {
		//        return true;
		//    }

		//    return false;
		//}
	}
}