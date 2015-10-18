using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;

namespace DXFramework.PrimitiveFramework
{
	public class PRect : Primitive
	{
		protected float width;
		protected float height;

		public PRect( PRect rect )
			: base( rect )
		{
			this.width = rect.width;
			this.height = rect.height;
		}

		public PRect( RectangleF rect, bool filled ) : this( rect.X, rect.Y, rect.Width, rect.Height, filled ) { }

		public PRect( RectangleF rect, uint thickness = 1 ) : this( rect.X, rect.Y, rect.Width, rect.Height, thickness ) { }

		public PRect( Rectangle rect, bool filled ) : this( rect.X, rect.Y, rect.Width, rect.Height, filled ) { }

		public PRect( Rectangle rect, uint thickness = 1 ) : this( rect.X, rect.Y, rect.Width, rect.Height, thickness ) { }

		public PRect( float x, float y, float width, float height, bool filled ) : this( new Vector2( x, y ), width, height, filled ) { }

		public PRect( float x, float y, float width, float height, uint thickness = 1 ) : this( new Vector2( x, y ), width, height, thickness ) { }

		public PRect( Vector2 position, float width, float height, bool filled )
			: base( filled )
		{
			this.position = position;
			this.width = Math.Max( width, 1 );
			this.height = Math.Max( height, 1 );
		}

		public PRect( Vector2 position, float width, float height, uint thickness = 1 )
			: base( thickness )
		{
			if( thickness >= width * 0.5f )
			{
				width = thickness + 1;
			}
			if( thickness >= height * 0.5f )
			{
				height = thickness + 1;
			}
			this.position = position;
			this.width = Math.Max( width, 1 );
			this.height = Math.Max( height, 1 );
		}

		public float Width
		{
			get { return width; }
			set
			{
				width = value;
				primitiveCreated = false;
			}
		}

		public float Height
		{
			get { return height; }
			set
			{
				height = value;
				primitiveCreated = false;
			}
		}

		internal override List<PolygonPoint> GetPoints()
		{
			List<PolygonPoint> points = new List<PolygonPoint>() {
                new PolygonPoint(0, 0),
                new PolygonPoint(width, 0),
                new PolygonPoint(width, height),
                new PolygonPoint(0, height),
            };

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
				List<PolygonPoint> holePoints = new List<PolygonPoint>(){
                    new PolygonPoint(thickness, thickness),
                    new PolygonPoint(width - thickness, thickness),
                    new PolygonPoint(width - thickness, height - thickness),
                    new PolygonPoint(thickness, height - thickness)
                };

				Polygon hole = new Polygon( holePoints );
				poly.AddHole( hole );
			}

			return poly;
		}
	}
}