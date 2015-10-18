using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;
using MathUtil = SharpDX.MathUtil;

namespace DXPrimitiveFramework
{
	public class PArc : Primitive
	{
		private const int DEFAULT_SIDES = 8;
		protected float radius;
		protected int sides;

		public PArc( PArc arc )
			: base( arc )
		{
			this.radius = arc.radius;
			this.degrees = arc.degrees;
			this.sides = arc.sides;
		}

		public PArc( float x, float y, float radius, float degrees, bool filled )
			: this( x, y, radius, degrees, DEFAULT_SIDES, filled )
		{
		}

		public PArc( float x, float y, float radius, float degrees, uint thickness )
			: this( x, y, radius, degrees, DEFAULT_SIDES, thickness )
		{
		}

		public PArc( float x, float y, float radius, float degrees, int sides, bool filled )
			: this( new Vector2( x, y ), radius, degrees, sides, filled )
		{
		}

		public PArc( float x, float y, float radius, float degrees, int sides, uint thickness )
			: this( new Vector2( x, y ), radius, degrees, sides, thickness )
		{
		}

		public PArc( Vector2 position, float radius, float degrees, bool filled )
			: this( position, radius, degrees, DEFAULT_SIDES, filled )
		{
		}

		public PArc( Vector2 position, float radius, float degrees, uint thickness )
			: this( position, radius, degrees, DEFAULT_SIDES, thickness )
		{
		}

		public PArc( Vector2 position, float radius, float degrees, int sides, bool filled )
			: base( filled )
		{
			this.position = position;
			this.radius = radius;
			this.sides = sides;
			this.degrees = degrees;
		}

		public PArc( Vector2 position, float radius, float degrees, int sides, uint thickness )
			: base( thickness )
		{
			if( thickness >= radius )
			{
				throw new ArgumentException( "Arc thickness cannot be greater than radius - 1 (Current max thickness: " + ( radius - 1 ) + ")." );
			}
			this.position = position;
			this.radius = radius;
			this.sides = sides;
			this.degrees = degrees;
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

		internal override List<PolygonPoint> GetPoints()
		{
			List<PolygonPoint> points = new List<PolygonPoint>();

			//float degreeStep = degrees / sides;
			//int steps = sides + 1;
			//float reminderStep = degreeStep - (int)degreeStep;

			//if(reminderStep != 0) {
			//    steps--;
			//}

			for( int i = 0; i < degrees; i++ )
			{
				float radAngle = MathUtil.DegreesToRadians( i );
				float x = (float)Math.Cos( radAngle ) * radius;
				float y = (float)Math.Sin( radAngle ) * radius;

				points.Add( new PolygonPoint( x, y ) );
			}

			//if(reminderStep != 0) {
			//    float radAngle = MathHelper.ToRadians(degreeStep * sides + reminderStep);
			//    float x = (float)Math.Cos(radAngle) * radius;
			//    float y = (float)Math.Sin(radAngle) * radius;

			//    points.Add(new PolygonPoint(x, y));
			//}

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
				//int steps = sides + 1;

				for( int i = 0; i < degrees; i++ )
				{
					radAngle = MathUtil.DegreesToRadians( i );
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