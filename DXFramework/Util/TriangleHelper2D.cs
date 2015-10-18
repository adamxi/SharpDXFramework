using System;
using SharpDX;

namespace DXFramework.Util
{
	// Todo: Class is a mess. Clean it up and make it more relavant.
	public static class TriangleHelper2D
	{
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
		/// Returns a radian from a Vector2. Up (0, -1) is equal to 0.
		/// </summary>
		public static float VectorToRadian(Vector2 vector)
		{
			float radians = (float)Math.Atan2(vector.X, -vector.Y);

			if (radians < 0)
			{
				radians += MathUtil.TwoPi;
			}

			return radians;
		}

		public static Vector2 Intersection(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
		{
			float a1 = Slope(A1, A2);
			float a2 = Slope(B1, B2);

			float b1 = (A1.Y - a1 * A1.X);
			float b2 = (B1.Y - a2 * B1.X);

			Vector2 vec;
			vec.X = ((b2 - b1) / (a1 - a2));
			vec.Y = (a1 * vec.X + b1);
			//double y2   = a2 * x + b2; //just to check
			return vec;
		}

		/// <summary>
		/// Returns the intersection point of two infinitely long lines. Returns an empty vector2 if the lines are parallel.
		/// </summary>
		/// <param name="A1">Line 1 start.</param>
		/// <param name="A2">Line 1 stop.</param>
		/// <param name="B1">Line 2 start.</param>
		/// <param name="B2">Line 2 stop.</param>
		public static Vector2 LineIntersectionPoint(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
		{
			float x1 = A1.X;
			float x2 = A2.X;
			float x3 = B1.X;
			float x4 = B2.X;
			float y1 = A1.Y;
			float y2 = A2.Y;
			float y3 = B1.Y;
			float y4 = B2.Y;

			float a = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4));
			float b = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4));
			float d = ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

			if (d == 0)
			{
				return Vector2.Zero;
			}

			float ua = a / d;
			float ub = b / d;

			return new Vector2(ua, ub);
		}

		public static Vector2 VectorIntersectionPoint(Vector2 positionA, Vector2 vecA, Vector2 positionB, Vector2 vecB)
		{
			return LineIntersectionPoint(positionA, Extend(positionA, vecA, 1), positionB, Extend(positionB, vecB, 1));
		}

		/// <summary>
		/// Returns true if a line intersects with another line based on position and vector.
		/// </summary>
		/// <param name="A1">Line 1 start.</param>
		/// <param name="A2">Line 1 stop.</param>
		/// <param name="objPos">Object position.</param>
		/// <param name="objVector">Object velocity vector.</param>
		/// <param name="offset">Ekstra vector extension.</param>
		/// <param name="intersectionPoint">Vector to store intersection point in.</param>
		public static bool WillIntersect(Vector2 A1, Vector2 A2, Vector2 objPos, Vector2 objVector, float offset, ref Vector2 intersectionPoint)
		{
			Vector2 off = Vector2.Normalize(objVector);
			Vector2 next = new Vector2(objPos.X + objVector.X + off.X * offset, objPos.Y + objVector.Y + off.Y * offset);
			return Intersects(A1, A2, objPos, next, ref intersectionPoint);
		}

		/// <summary>
		/// Returns true if a line intersects with another line based on position and vector.
		/// </summary>
		/// <param name="A1">Line 1 start.</param>
		/// <param name="A2">Line 1 stop.</param>
		/// <param name="objPos">Object position.</param>
		/// <param name="objVector">Normalized object vector.</param>
		public static bool WillIntersect(Vector2 A1, Vector2 A2, Vector2 objPos, Vector2 objVector)
		{
			Vector2 next = new Vector2(objPos.X + objVector.X, objPos.Y + objVector.Y);
			return Intersects(A1, A2, objPos, next);
		}

		/// <summary>
		/// Returns true if two finite lines intersects.
		/// </summary>
		/// <param name="A1">Line 1 start.</param>
		/// <param name="A2">Line 1 stop.</param>
		/// <param name="B1">Line 2 start.</param>
		/// <param name="B2">Line 2 stop.</param>
		public static bool Intersects(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
		{
			Vector2 p = Vector2.Zero;
			return Intersects(A1, A2, B1, B2, ref p);
		}

		//public static bool Intersects(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, float radius, ref Vector2 intersectionPoint){
		//    Vector2 vec = Vector(B1, B2);
		//    B2 = Extend(B2, vec, radius);
		//    return Intersects(A1, A2, B1, B2, ref intersectionPoint);
		//}

		/// <summary>
		/// Returns true if two finite lines intersects.
		/// </summary>
		/// <param name="A1">Line 1 start.</param>
		/// <param name="A2">Line 1 stop.</param>
		/// <param name="B1">Line 2 start.</param>
		/// <param name="B2">Line 2 stop.</param>
		/// <param name="intersectionPoint">Vector to store intersection point in.</param>
		public static bool Intersects(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, ref Vector2 intersectionPoint)
		{
			float x1 = A1.X;
			float x2 = A2.X;
			float x3 = B1.X;
			float x4 = B2.X;
			float y1 = A1.Y;
			float y2 = A2.Y;
			float y3 = B1.Y;
			float y4 = B2.Y;

			//http://www.ziggyware.com/readarticle.php?article_id=78
			float d = ((x2 - x1) * (y4 - y3) - (y2 - y1) * (x4 - x3));

			if (d == 0)
			{
				return false;
			}

			float numR = ((y1 - y3) * (x4 - x3) - (x1 - x3) * (y4 - y3));
			float numS = ((y1 - y3) * (x2 - x1) - (x1 - x3) * (y2 - y1));
			float r = numR / d;
			float s = numS / d;

			intersectionPoint.X = x1 + (r * (x2 - x1));
			intersectionPoint.Y = y1 + (r * (y2 - y1));

			if (r < 0 || r > 1 || s < 0 || s > 1)
			{
				return false;
			}
			return true;
		}

		//public static Vector2[] IntersectionPoints(Line line, Circle circle){
		//    Vector2[] intersections;
		//    double mu;

		//    // Quadratic equations
		//    double a            = (line.End.X - line.Start.X) * (line.End.X - line.Start.X) + (line.End.Y - line.Start.Y) * (line.End.Y - line.Start.Y);
		//    double b            = 2 * ((line.End.X - line.Start.X) * (line.Start.X - circle.X) + (line.End.Y - line.Start.Y) * (line.Start.Y - circle.Y));
		//    double c            = (circle.X * circle.X) + (circle.Y * circle.Y) + (line.Start.X * line.Start.X) + (line.Start.Y * line.Start.Y) - 2 * (circle.X * line.Start.X + circle.Y * line.Start.Y) - (circle.Radius * circle.Radius);
		//    double discriminant = b * b - 4 * a * c;

		//    if(discriminant == 0){
		//        // one intersection
		//        intersections = new Vector2[1];
		//        mu = -b / (2 * a);
		//        intersections[0] = new Vector2((float)(line.Start.X + mu * (line.End.X - line.Start.X)), (float)(line.Start.Y + mu * (line.End.Y - line.Start.Y)));
		//        return intersections;
		//    } else if(discriminant > 0.0){
		//        // two intersections
		//        intersections = new Vector2[2];

		//        // first intersection
		//        mu = (-b + Math.Sqrt((b * b) - 4 * a * c)) / (2 * a);
		//        intersections[0] = new Vector2((float)(line.Start.X + mu * (line.End.X - line.Start.X)), (float)(line.Start.Y + mu * (line.End.Y - line.Start.Y)));

		//        // second intersection
		//        mu = (-b - Math.Sqrt((b * b) - 4 * a * c)) / (2 * a);
		//        intersections[1] = new Vector2((float)(line.Start.X + mu * (line.End.X - line.Start.X)), (float)(line.Start.Y + mu * (line.End.Y - line.Start.Y)));

		//        return intersections;
		//    } else{
		//        // no intersection - return a blank vector array
		//        intersections = new Vector2[0];
		//        return intersections;
		//    }
		//}

		////That's for an infinite line though, so to check its in the segment use this (though its probably not the "perfect" test)
		//public static bool PointWithinSegment(Line line, Vector2 point){
		//    if(line.Start.X > line.End.X){
		//        if(point.X > line.Start.X || point.X < line.End.X) return false;
		//    } else{
		//        if(point.X > line.End.X || point.X < line.Start.X) return false;
		//    }

		//    if(line.Start.Y > line.End.Y){
		//        if(point.Y > line.Start.Y || point.Y < line.End.Y) return false;
		//    } else{
		//        if(point.Y > line.End.Y || point.Y < line.Start.Y) return false;
		//    }
		//    return true;
		//}

		public static float Slope(Vector2 A, Vector2 B) => (B.Y - A.Y) / (B.X - A.X);

		public static float Slope(Vector2 vector) => vector.Y / vector.X;

		/// <summary>
		/// Extends a vector by an amount.
		/// </summary>
		/// <param name="vector">Vector.</param>
		/// <param name="amount">Amount to extend.</param>
		public static Vector2 Extend(Vector2 vector, float amount) => Extend(Vector2.Zero, vector, amount);

		/// <summary>
		/// Extends a vector by an amount.
		/// </summary>
		/// <param name="position">Position from which to extend from.</param>
		/// <param name="vector">Vector.</param>
		/// <param name="amount">Amount to extend.</param>
		public static Vector2 Extend(Vector2 position, Vector2 vector, float amount)
		{
			Vector2 vec;
			vec.X = vector.X * amount + position.X;
			vec.Y = vector.Y * amount + position.Y;
			return vec;
		}

		public static Vector2 Extend(Vector2 position, Vector2 vector, Vector2 amount)
		{
			Vector2 vec;
			vec.X = (vector.X * amount.X) + position.X;
			vec.Y = (vector.Y * amount.Y) + position.Y;
			return vec;
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
		public static float AngleA(Vector2 A, Vector2 B, Vector2 C) => Angle(A, B, C);

		/// <summary>
		/// Returns the degrees of angle B in a triangle using the law of cosines.
		/// </summary>
		/// <param name="A">Position of A.</param>
		/// <param name="B">Position of B.</param>
		/// <param name="C">Position of C.</param>
		public static float AngleB(Vector2 A, Vector2 B, Vector2 C) => Angle(B, C, A);

		/// <summary>
		/// Returns the degrees of angle C in a triangle using the law of cosines.
		/// </summary>
		/// <param name="A">Position of A.</param>
		/// <param name="B">Position of B.</param>
		/// <param name="C">Position of C.</param>
		public static float AngleC(Vector2 A, Vector2 B, Vector2 C) => Angle(C, A, B);

		/// <summary>
		/// Returns the angle in degrees relative to a point.
		/// </summary>
		/// <param name="basePoint">The point from which to get the relative angle.</param>
		/// <param name="relativePoint">The point to compare with the basePoint.</param>
		public static float AngleInDegrees(Vector2 basePoint, Vector2 relativePoint)
		{
			float degree = AngleInRadians(basePoint, relativePoint);
			return MathUtil.RadiansToDegrees(degree);
		}

		/// <summary>
		/// Returns the angle in radians relative to a point.
		/// </summary>
		/// <param name="basePoint">The point from which to get the relative angle.</param>
		/// <param name="relativePoint">The point to compare with the basePoint.</param>
		public static float AngleInRadians(Vector2 basePoint, Vector2 relativePoint)
		{
			float y = (basePoint.Y - relativePoint.Y);
			float x = (basePoint.X - relativePoint.X);

			return (float)Math.Atan2(y, x);
		}

		/// <summary>
		/// Returns the acreage in a triangle using the degree from A.
		/// </summary>
		/// <param name="A">Point A.</param>
		/// <param name="B">Point B.</param>
		/// <param name="C">Point C.</param>
		/// <param name="degreeA">Degree of angle A.</param>
		public static float AcreageWithA(Vector2 A, Vector2 B, Vector2 C, float degreeA)
		{
			float lenA = Vector2.Distance(B, C);
			float lenB = Vector2.Distance(A, C);
			float lenC = Vector2.Distance(A, B);
			float degree = (float)Math.Sin(MathUtil.DegreesToRadians(degreeA));
			float T = (0.5f * lenB * lenC * degree);

			return T;
		}

		/// <summary>
		/// Returns the acreage in a triangle using the degree from B.
		/// </summary>
		/// <param name="A">Point A.</param>
		/// <param name="B">Point B.</param>
		/// <param name="C">Point C.</param>
		/// <param name="degreeA">Degree of angle B.</param>
		public static float AcreageWithB(Vector2 A, Vector2 B, Vector2 C, float degreeB)
		{
			float lenA = Vector2.Distance(B, C);
			float lenB = Vector2.Distance(A, C);
			float lenC = Vector2.Distance(A, B);
			float degree = (float)Math.Sin(MathUtil.DegreesToRadians(degreeB));
			float T = (0.5f * lenA * lenC * degree);

			return T;
		}

		/// <summary>
		/// Returns the acreage in a triangle using the degree from C.
		/// </summary>
		/// <param name="A">Point A.</param>
		/// <param name="B">Point B.</param>
		/// <param name="C">Point C.</param>
		/// <param name="degreeA">Degree of angle C.</param>
		public static float AcreageWithC(Vector2 A, Vector2 B, Vector2 C, float degreeC)
		{
			float lenA = Vector2.Distance(B, C);
			float lenB = Vector2.Distance(A, C);
			float lenC = Vector2.Distance(A, B);
			float degree = (float)Math.Sin(MathUtil.DegreesToRadians(degreeC));
			float T = (0.5f * lenA * lenB * degree);

			return T;
		}

		public static float HeightOfA(Vector2 A, Vector2 B, Vector2 C)
		{
			float lenA = Vector2.Distance(B, C);
			float lenB = Vector2.Distance(A, C);
			float lenC = Vector2.Distance(A, B);

			float angleB = AngleB(A, B, C);
			float angleC = 90;
			float angleA = (180 - angleB - angleC);
			double height = (lenC / Math.Sin(MathUtil.DegreesToRadians(angleC)) * Math.Sin(MathUtil.DegreesToRadians(angleB)));

			return (float)height;
		}
	}
}