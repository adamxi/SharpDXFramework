using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;
using MathUtil = SharpDX.MathUtil;

namespace DXPrimitiveFramework
{
	public class PTriangle : Primitive
	{
		private Vector2 a;
		private Vector2 b;
		private Vector2 c;

		public PTriangle(PTriangle triangle) : base(triangle)
		{
			this.a = triangle.a;
			this.b = triangle.b;
			this.c = triangle.c;
		}

		public PTriangle(float lengthAB, float height, bool filled) : base(filled)
		{
			this.a = Vector2.Zero;
			this.b = new Vector2(lengthAB, 0);
			this.c = new Vector2(lengthAB * 0.5f, -height);
		}

		public PTriangle(float lengthAB, float height, uint thickness) : base(thickness)
		{
			this.a = Vector2.Zero;
			this.b = new Vector2(lengthAB, 0);
			this.c = new Vector2(lengthAB * 0.5f, -height);
		}

		public PTriangle(Vector2 a, float lengthAB, float angleB, bool filled) : base(filled)
		{
			if (angleB >= 180)
			{
				throw new ArgumentException("Angle cannot be greater than or equal to 180.");
			}
			this.position = a;
			this.a = a;
			this.b = new Vector2(a.X + lengthAB, a.Y);
			this.c = b + RadianToVector(MathUtil.DegreesToRadians(angleB) - MathUtil.PiOverTwo) * lengthAB;
		}

		public PTriangle(Vector2 a, float lengthAB, float angleB, uint thickness) : base(thickness)
		{
			if (angleB >= 180)
			{
				throw new ArgumentException("Angle cannot be greater than or equal to 180.");
			}
			this.position = a;
			this.a = a;
			this.b = new Vector2(a.X + lengthAB, a.Y);
			this.c = b + RadianToVector(MathUtil.DegreesToRadians(angleB) - MathUtil.PiOverTwo) * lengthAB;
		}

		public PTriangle(Vector2 a, Vector2 b, Vector2 c, bool filled) : base(filled)
		{
			this.position = a;
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public PTriangle(Vector2 a, Vector2 b, Vector2 c, uint thickness) : base(thickness)
		{
			this.position = a;
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public Vector2 C
		{
			get { return c; }
		}

		internal override List<PolygonPoint> GetPoints(float thickness = 0)
		{
			List<PolygonPoint> points = new List<PolygonPoint>(){
				new PolygonPoint(a.X, a.Y),
				new PolygonPoint(b.X, b.Y),
				new PolygonPoint(c.X, c.Y)};

			if (!Filled)
			{
				points.Add(points[0]);
			}

			// Offset all points by distance to the centroid. This places all points around (0, 0).
			Vector2 center = GetCentroid(points);
			foreach (PolygonPoint point in points)
			{
				point.X -= center.X;
				point.Y -= center.Y;
			}

			return points;
		}

		protected override Polygon GetPolygon()
		{
			Polygon poly = new Polygon(GetPoints());

			if (thickness > 1)
			{
				List<PolygonPoint> points = GetPoints();
				Vector2 center = GetCentroid(points);
				float[] angles = { AngleA(a, b, c), AngleB(a, b, c), AngleC(a, b, c) };
				int count = points.Count;

				for (int i = count; --i >= 0;)
				{
					PolygonPoint point = points[i];

					double vecX = center.X - point.X;
					double vecY = center.Y - point.Y;
					double invLen = 1d / Math.Sqrt((vecX * vecX) + (vecY * vecY));
					vecX = vecX * invLen;
					vecY = vecY * invLen;

					float ratio = 1 - (angles[i] / 180);
					float angleThickness = ratio * thickness;
					point.X += vecX * angleThickness;
					point.Y += vecY * angleThickness;
				}

				Polygon hole = new Polygon(points);
				poly.AddHole(hole);
			}

			return poly;
		}

		public override bool Intersects(float x, float y)
		{
			if (Filled)
			{
				return base.Intersects(x, y);
			}
			else
			{
				Vector2 A = new Vector2();
				Vector2 B = new Vector2();
				Vector2 C = new Vector2();

				A.X = tranformedVPCs[0].Position.X;
				A.Y = tranformedVPCs[0].Position.Y;

				B.X = tranformedVPCs[2].Position.X;
				B.Y = tranformedVPCs[2].Position.Y;

				C.X = tranformedVPCs[4].Position.X;
				C.Y = tranformedVPCs[4].Position.Y;

				if (IntersectsTriangle(ref x, ref y, ref A, ref B, ref C))
				{
					return true;
				}
			}
			return false;
		}

		#region Triangle helper functions
		/// <summary>
		/// Returns a normalized Vector2 from a radian. 0 is equal to up (0, -1).
		/// </summary>
		public static Vector2 RadianToVector(float radian)
		{
			Vector2 v = new Vector2();
			v.X = (float)Math.Sin(radian);
			v.Y = -(float)Math.Cos(radian);
			return v;
		}

		/// <summary>
		/// Returns the degrees of angle A in a triangle using the law of cosines.
		/// </summary>
		/// <param name="A">Position of A.</param>
		/// <param name="B">Position of B.</param>
		/// <param name="C">Position of C.</param>
		public static float Angle(Vector2 A, Vector2 B, Vector2 C)
		{
			float lenA = Vector2.Distance(B, C);
			float lenB = Vector2.Distance(A, C);
			float lenC = Vector2.Distance(A, B);
			float tmpAngle = ((lenB * lenB + lenC * lenC - lenA * lenA) / (2 * lenB * lenC));
			float radianAngle = (float)Math.Acos(tmpAngle);

			return MathUtil.RadiansToDegrees(radianAngle);
		}

		/// <summary>
		/// Returns the degrees of angle A in a triangle using the law of cosines.
		/// </summary>
		/// <param name="A">Position of A.</param>
		/// <param name="B">Position of B.</param>
		/// <param name="C">Position of C.</param>
		public static float AngleA(Vector2 A, Vector2 B, Vector2 C)
		{
			return Angle(A, B, C);
		}

		/// <summary>
		/// Returns the degrees of angle B in a triangle using the law of cosines.
		/// </summary>
		/// <param name="A">Position of A.</param>
		/// <param name="B">Position of B.</param>
		/// <param name="C">Position of C.</param>
		public static float AngleB(Vector2 A, Vector2 B, Vector2 C)
		{
			return Angle(B, C, A);
		}

		/// <summary>
		/// Returns the degrees of angle C in a triangle using the law of cosines.
		/// </summary>
		/// <param name="A">Position of A.</param>
		/// <param name="B">Position of B.</param>
		/// <param name="C">Position of C.</param>
		public static float AngleC(Vector2 A, Vector2 B, Vector2 C)
		{
			return Angle(C, A, B);
		}
		#endregion
	}
}