using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;

namespace DXFramework.PrimitiveFramework
{
	public class PCircle : Primitive
	{
		private const int DEFAULT_SIDES = 32;
		protected float radius;
		protected int sides;

		public PCircle( PCircle circle )
			: base( circle )
		{
			this.radius = circle.radius;
			this.sides = circle.sides;
		}

		public PCircle( float x, float y, float radius, bool filled ) : this( x, y, radius, DEFAULT_SIDES, filled ) { }

		public PCircle( float x, float y, float radius, uint thickness ) : this( x, y, radius, DEFAULT_SIDES, thickness ) { }

		public PCircle( float x, float y, float radius, int sides, bool filled ) : this( new Vector2( x, y ), radius, sides, filled ) { }

		public PCircle( float x, float y, float radius, int sides, uint thickness ) : this( new Vector2( x, y ), radius, sides, thickness ) { }

		public PCircle( Vector2 position, float radius, bool filled ) : this( position, radius, DEFAULT_SIDES, filled ) { }

		public PCircle( Vector2 position, float radius, uint thickness ) : this( position, radius, DEFAULT_SIDES, thickness ) { }

		public PCircle( Vector2 position, float radius, int sides, bool filled )
			: base( filled )
		{
			this.position = position;
			this.radius = radius;
			this.sides = sides;
		}

		public PCircle( Vector2 position, float radius, int sides, uint thickness )
			: base( thickness )
		{
			if( thickness >= radius )
			{
				throw new ArgumentException( "Circle thickness cannot be greater than radius - 1 (Current max thickness: " + ( radius - 1 ) + ")." );
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
				primitiveCreated = false;
			}
		}

		public int Sides
		{
			get { return sides; }
			set
			{
				sides = value;
				primitiveCreated = false;
			}
		}

		internal override List<PolygonPoint> GetPoints()
		{
			List<PolygonPoint> points = new List<PolygonPoint>();

			double x;
			double y;
			float radAngle;
			float degreeStep = 360f / sides;

			for( int i = 0; i < sides; i++ )
			{
				radAngle = MathUtil.DegreesToRadians( degreeStep * i );
				x = Math.Cos( radAngle ) * radius;
				y = Math.Sin( radAngle ) * radius;

				points.Add( new PolygonPoint( x, y ) );
			}

			if( !Filled )
			{
				points.Add( points[ 0 ] );
			}

			return points;
		}

		protected override Polygon GetPolygon()
		{
			Polygon poly = new Polygon( GetPoints() );

			if( thickness > 0 )
			{
				List<PolygonPoint> holePoints = new List<PolygonPoint>();

				float x;
				float y;
				float radAngle;
				float degreeStep = 360f / sides;
				float r = radius - thickness;

				for( int i = 0; i < sides; i++ )
				{
					radAngle = MathUtil.DegreesToRadians( degreeStep * i );
					x = (float)( Math.Cos( radAngle ) * r );
					y = (float)( Math.Sin( radAngle ) * r );

					holePoints.Add( new PolygonPoint( x, y ) );
				}

				Polygon hole = new Polygon( holePoints );
				poly.AddHole( hole );
			}

			return poly;
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