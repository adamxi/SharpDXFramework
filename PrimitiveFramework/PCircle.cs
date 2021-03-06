﻿using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;
using MathUtil = SharpDX.MathUtil;

namespace DXPrimitiveFramework
{
	public class PCircle : Primitive
	{
		private const int DEFAULT_SIDES = 32;
		protected float radius;
		protected int sides;

		public PCircle(PCircle circle) : base(circle)
		{
			this.radius = circle.radius;
			this.sides = circle.sides;
		}

		public PCircle(float x, float y, float radius, bool filled) : this(x, y, radius, DEFAULT_SIDES, filled) { }

		public PCircle(float x, float y, float radius, float thickness) : this(x, y, radius, DEFAULT_SIDES, thickness) { }

		public PCircle(float x, float y, float radius, int sides, bool filled) : this(new Vector2(x, y), radius, sides, filled) { }

		public PCircle(float x, float y, float radius, int sides, float thickness) : this(new Vector2(x, y), radius, sides, thickness) { }

		public PCircle(Vector2 position, float radius, bool filled) : this(position, radius, DEFAULT_SIDES, filled) { }

		public PCircle(Vector2 position, float radius, float thickness) : this(position, radius, DEFAULT_SIDES, thickness) { }

		public PCircle(Vector2 position, float radius, int sides, bool filled) : base(filled)
		{
			this.position = position;
			this.radius = radius;
			this.sides = sides;
		}

		public PCircle(Vector2 position, float radius, int sides, float thickness) : base(thickness)
		{
			if (thickness >= radius)
			{
				throw new ArgumentException("Circle thickness cannot be greater than radius - 1 (Current max thickness: " + (radius - 1) + ").");
			}
			this.position = position;
			this.radius = radius;
			this.sides = sides;
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

			float degreeStep = 360f / sides;
			float r = radius - thickness;

			for (int i = 0; i < sides; i++)
			{
				float radAngle = MathUtil.DegreesToRadians(degreeStep * i);
				double x = Math.Cos(radAngle) * r;
				double y = Math.Sin(radAngle) * r;

				points.Add(new PolygonPoint(x, y));
			}

			if (!Filled)
			{
				points.Add(points[0]);
			}

			return points;
		}

		public override bool Intersects(float x, float y)
		{
			float distX = x - TransformedPosition.X;
			float distY = y - TransformedPosition.Y;
			float radius = this.radius * TransformedScale.X;
			float distSquared = (distX * distX) + (distY * distY);
			float radiusSquared = radius * radius;

			if (thickness > 1)
			{
				float t = thickness * TransformedScale.X;
				float innerRadiusSquared = (radius - t) * (radius - t);

				if (distSquared <= radiusSquared && distSquared > innerRadiusSquared)
				{
					return true;
				}
			}
			else if (distSquared <= radiusSquared)
			{
				return true;
			}

			return false;
		}
	}
}