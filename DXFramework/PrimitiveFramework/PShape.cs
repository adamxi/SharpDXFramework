using System.Collections.Generic;
using Poly2Tri;
using SharpDX;

namespace DXFramework.PrimitiveFramework
{
	public class PShape : Primitive
	{
		private List<PolygonPoint> points;

		public PShape( PShape shape )
			: base( shape )
		{
			this.points = shape.points;
		}

		public PShape( bool filled )
			: base( filled )
		{
			points = new List<PolygonPoint>();
		}

		public void AddVertices( List<Vector2> vertices )
		{
			foreach( Vector2 vertex in vertices )
			{
				AddVertex( vertex );
			}
		}

		public void AddVertex( Vector2 vertex )
		{
			AddVertex( vertex, Color );
		}

		public void AddVertex( Vector2 vertex, Color color )
		{
			AddVertex( vertex.X, vertex.Y, color );
		}

		public void AddVertex( float x, float y )
		{
			AddVertex( x, y, Color );
		}

		public void AddVertex( float x, float y, Color color )
		{
			if( position == Vector2.Zero )
			{
				position = new Vector2( x, y );
			}

			points.Add( new PolygonPoint( x, y ) );
			vertexColors.Add( color );
		}

		internal override List<PolygonPoint> GetPoints()
		{
			List<PolygonPoint> points = this.points;

			if( !Filled && points.Count > 2 )
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
	}
}