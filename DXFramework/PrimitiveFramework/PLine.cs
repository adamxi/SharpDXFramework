﻿using System.Collections.Generic;
using Poly2Tri;
using SharpDX;

namespace DXFramework.PrimitiveFramework
{
	public class PLine : Primitive
	{
		private Vector2 end;

		public PLine( PLine line )
			: base( line )
		{
			this.end = line.end;
		}

		public PLine( float startX, float startY, float endX, float endY, uint thickness = 1 )
			: base( thickness )
		{
			this.position.X = startX;
			this.position.Y = startY;
			this.end.X = endX - startX;
			this.end.Y = endY - startY;
		}

		public PLine( Vector2 start, Vector2 end, uint thickness = 1 )
			: base( thickness )
		{
			this.position = start;
			this.end = end - start;
		}

		protected Vector2 End
		{
			get { return end; }
			set
			{
				end = value;
				primitiveCreated = false;
			}
		}

		internal override List<PolygonPoint> GetPoints()
		{
			List<PolygonPoint> points = new List<PolygonPoint>(){
                new PolygonPoint(0, 0),
                new PolygonPoint(end.X, end.Y)
            };

			return points;
		}

		protected override Polygon GetPolygon()
		{
			Vector2 slope;
			slope.X = -( end.Y - 0 );
			slope.Y = end.X - 0;
			slope.Normalize();
			slope *= Thickness * 0.5f;

			Vector2 topLeft = Vector2.Zero - slope;
			Vector2 topRight = end - slope;
			Vector2 bottomRight = end + slope;
			Vector2 bottomLeft = Vector2.Zero + slope;

			List<PolygonPoint> points = new List<PolygonPoint>(){
                new PolygonPoint(topLeft.X, topLeft.Y),
                new PolygonPoint(topRight.X, topRight.Y),
                new PolygonPoint(bottomRight.X, bottomRight.Y),
                new PolygonPoint(bottomLeft.X, bottomLeft.Y)
            };

			return new Polygon( points );
		}
	}
}