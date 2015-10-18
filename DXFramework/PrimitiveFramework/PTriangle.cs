using System;
using System.Collections.Generic;
using DXFramework.Util;
using Poly2Tri;
using SharpDX;

namespace DXFramework.PrimitiveFramework
{
	public class PTriangle : Primitive
	{
		private Vector2 a;
		private Vector2 b;
		private Vector2 c;

		public PTriangle( PTriangle triangle )
			: base( triangle )
		{
			this.a = triangle.a;
			this.b = triangle.b;
			this.c = triangle.c;
		}

		public PTriangle( Vector2 a, float lengthAB, float angleB, bool filled )
			: base( filled )
		{
			if( angleB >= 180 )
			{
				throw new ArgumentException( "Angle cannot be greater than or equal to 180." );
			}
			this.position = a;
			this.a = a;
			this.b = new Vector2( a.X + lengthAB, a.Y );
			this.c = b + TriangleHelper2D.RadianToVector( MathUtil.DegreesToRadians( angleB ) - MathUtil.PiOverTwo ) * lengthAB;
		}

		public PTriangle( Vector2 a, float lengthAB, float angleB, uint thickness )
			: base( thickness )
		{
			if( angleB >= 180 )
			{
				throw new ArgumentException( "Angle cannot be greater than or equal to 180." );
			}
			this.position = a;
			this.a = a;
			this.b = new Vector2( a.X + lengthAB, a.Y );
			this.c = b + TriangleHelper2D.RadianToVector( MathUtil.DegreesToRadians( angleB ) - MathUtil.PiOverTwo ) * lengthAB;
		}

		public PTriangle( Vector2 a, Vector2 b, Vector2 c, bool filled )
			: base( filled )
		{
			this.position = a;
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public PTriangle( Vector2 a, Vector2 b, Vector2 c, uint thickness )
			: base( thickness )
		{
			this.position = a;
			this.a = a;
			this.b = b;
			this.c = c;
		}

		internal override List<PolygonPoint> GetPoints()
		{
			List<PolygonPoint> points = new List<PolygonPoint>(){
                new PolygonPoint(a.X, a.Y),
                new PolygonPoint(b.X, b.Y),
                new PolygonPoint(c.X, c.Y)};

			if( !Filled )
			{
				points.Add( points[ 0 ] );
			}

			// Offset all points by distance to the centroid. This places all points around (0, 0).
			Vector2 center = GetCentroid( points );
			foreach( PolygonPoint point in points )
			{
				point.X -= center.X;
				point.Y -= center.Y;
			}

			return points;
		}

		protected override Polygon GetPolygon()
		{
			Polygon poly = new Polygon( GetPoints() );

			if( thickness > 1 )
			{
				List<PolygonPoint> points = GetPoints();
				Vector2 center = GetCentroid( points );
				float[] angles = { TriangleHelper2D.AngleA( a, b, c ), TriangleHelper2D.AngleB( a, b, c ), TriangleHelper2D.AngleC( a, b, c ) };
				int count = points.Count;

				for( int i = count; --i >= 0; )
				{
					PolygonPoint point = points[ i ];

					double vecX = center.X - point.X;
					double vecY = center.Y - point.Y;
					double invLen = 1d / Math.Sqrt( ( vecX * vecX ) + ( vecY * vecY ) );
					vecX = vecX * invLen;
					vecY = vecY * invLen;

					float ratio = 1 - ( angles[ i ] / 180 );
					float angleThickness = ratio * thickness;
					point.X += vecX * angleThickness;
					point.Y += vecY * angleThickness;
				}

				Polygon hole = new Polygon( points );
				poly.AddHole( hole );
			}

			return poly;
		}

		public override bool Intersects( float x, float y )
		{
			if( Filled )
			{
				return base.Intersects( x, y );
			}
			else
			{
				Vector2 A = new Vector2();
				Vector2 B = new Vector2();
				Vector2 C = new Vector2();

				A.X = tranformedVPCs[ 0 ].Position.X;
				A.Y = tranformedVPCs[ 0 ].Position.Y;

				B.X = tranformedVPCs[ 2 ].Position.X;
				B.Y = tranformedVPCs[ 2 ].Position.Y;

				C.X = tranformedVPCs[ 4 ].Position.X;
				C.Y = tranformedVPCs[ 4 ].Position.Y;

				if( IntersectsTriangle( ref x, ref y, ref A, ref B, ref C ) )
				{
					return true;
				}
			}
			return false;
		}
	}
}