using System.Collections.Generic;
using Poly2Tri;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace DXPrimitiveFramework
{
	public class PLine : Primitive
	{
		private Vector2 end;
		private VertexPositionColor vpcStart;
		private VertexPositionColor vpcEnd;

		public PLine(PLine line) : base(line)
		{
			this.end = line.end;
		}

		public PLine(float startX, float startY, float endX, float endY, uint thickness = 1) : base(thickness)
		{
			this.position.X = startX;
			this.position.Y = startY;
			this.end.X = endX - startX;
			this.end.Y = endY - startY;
			Slope = new Vector2(endX - endY) - new Vector2(startX, startY);
			Slope.Normalize();

			vpcStart.Position.X = startX;
			vpcStart.Position.Y = startY;
			vpcEnd.Position.X = endX;
			vpcEnd.Position.Y = endY;
		}

		public PLine(Vector2 start, Vector2 end, uint thickness = 1) : base(thickness)
		{
			this.position = start;
			this.end = end - start;
			Slope = end - start;
			Slope.Normalize();

			vpcStart.Position.X = start.X;
			vpcStart.Position.Y = start.Y;
			vpcEnd.Position.X = end.X;
			vpcEnd.Position.Y = end.Y;
		}

		public Vector2 Slope { get; private set; }

		public Primitive StartShape { get; set; }

		public Primitive EndShape { get; set; }

		public Vector2 _Position
		{
			get { return position; }
			set { this.position = value; }
		}

		public Vector2 _End
		{
			get { return end; }
			set { this.end = value; }
		}

		public void SetEnd(Vector2 pos)
		{
			End = pos - position;
		}

		protected Vector2 End
		{
			get { return end; }
			set
			{
				end = value;
				PrimitiveCreated = false;
			}
		}

		internal override List<PolygonPoint> GetPoints(float thickness = 0)
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
			slope.X = -(end.Y - 0);
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

			return new Polygon(points);
		}

		public override void Draw()
		{
			base.Draw();
			return;

			InitializeForDrawing();
			//if (thickness <= 1)
			//{
			//	PrimitiveBatch.Batch.DrawLine(vpcStart, vpcEnd);
			//}
			//else
			{
				//PrimitiveBatch.Draw(this);
				PrimitiveBatch.Batch.Draw(PrimitiveType.LineStripWithAdjacency, TransformedVertexPositionColors);
			}
		}
	}
}