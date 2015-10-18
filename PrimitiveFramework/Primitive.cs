using System.Collections.Generic;
using Poly2Tri;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using MathUtil = SharpDX.MathUtil;

namespace DXPrimitiveFramework
{
    public abstract class Primitive
	{
		protected List<VertexPositionColor> vertexPositionColors;
		protected VertexPositionColor[] tranformedVPCs;
		protected VertexPositionColor lastVertexPositionColor;
		protected List<Color> vertexColors;

		protected PrimitiveType primitiveType;
		protected Color color;
		protected Vector2 position;
		protected Vector2 origin;
		protected Vector2 scale;
		protected float degrees;
		protected int alpha;
		protected bool filled;
		protected float thickness;
		private bool primitiveCreated;

		protected Matrix transform;
		protected Matrix positionMatrix;
		protected Matrix originMatrix;
		protected Matrix scaleMatrix;
		protected Matrix rotationMatrix;

		/// <summary>
		/// Primitive copy constructor. Inheriting classes implementing a copy constructor must call this.
		/// </summary>
		/// <param name="primitive">Primitive to copy.</param>
		public Primitive(Primitive primitive)
		{
			this.vertexPositionColors = new List<VertexPositionColor>();
			this.vertexColors = new List<Color>();
			this.primitiveType = primitive.primitiveType;
			this.color = primitive.color;
			this.position = primitive.position;
			this.origin = primitive.origin;
			this.scale = primitive.scale;
			this.degrees = primitive.degrees;
			this.alpha = primitive.alpha;
			this.filled = primitive.filled;
			this.thickness = primitive.thickness;
		}

		internal Primitive()
		{
			color = Color.White;
			alpha = color.A;
			scale = Vector2.One;
		}

		public Primitive(bool filled)
		{
			this.filled = filled;
			Init();
		}

		public Primitive(float thickness)
		{
			this.thickness = thickness;
			this.filled = thickness > 1;
			Init();
		}

		private void Init()
		{
			vertexPositionColors = new List<VertexPositionColor>();
			vertexColors = new List<Color>();
			primitiveType = filled ? PrimitiveType.TriangleList : PrimitiveType.LineList;
			color = Color.White;
			alpha = color.A;
			scale = Vector2.One;
		}

		#region Properties
		public bool UpdateTransform { get; set; }
		protected bool PrimitiveCreated
		{
			get
			{
				return primitiveCreated;
			}

			set
			{
				primitiveCreated = value;
				if (value == false)
				{
					"".ToCharArray();
				}
			}
		}

		/// <summary>
		/// Number of polygons in the primitive. This only applies if the primitiveType is a TriangleList.
		/// </summary>
		public int PolygonCount { get; protected set; }

		/// <summary>
		/// True if the primitive is filled.
		/// </summary>
		public bool Filled
		{
			get { return filled; }
			set
			{
				if (filled != value)
				{
					filled = value;
					primitiveType = value ? PrimitiveType.TriangleList : PrimitiveType.LineList;
					PrimitiveCreated = false;
				}
			}
		}

		/// <summary>
		/// Primitive thickness. Must be a positive value (uint).
		/// </summary>
		public float Thickness
		{
			get { return thickness; }
		}

		/// <summary>
		/// Primitive scale.
		/// </summary>
		public Vector2 Scale
		{
			get { return scale; }
			set
			{
				if (scale != value)
				{
					scale = value;
					scaleMatrix = Matrix.Scaling(new Vector3(scale, 0f));
					UpdateTransform = true;
				}
			}
		}

		/// <summary>
		/// Primitive position.
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set
			{
				if (position != value)
				{
					position = value;
					positionMatrix = Matrix.Translation(new Vector3(position, 0));
					UpdateTransform = true;
				}
			}
		}

		/// <summary>
		/// Primitive origin. Origin affects the drawn position of the primitive.
		/// </summary>
		public Vector2 Origin
		{
			get { return origin; }
			set
			{
				if (origin != value)
				{
					origin = value;
					originMatrix = Matrix.Translation(new Vector3(-origin, 0));
					UpdateTransform = true;
				}
			}
		}

		/// <summary>
		/// Primitive degrees from 0 to 360.
		/// </summary>
		public float Degrees
		{
			get { return degrees; }
			set
			{
				value %= 360;
				if (value > 360)
				{
					value -= 360;
				}
				else if (value < 0)
				{
					value += 360;
				}
				if (degrees != value)
				{
					degrees = value;
					rotationMatrix = Matrix.RotationZ(MathUtil.DegreesToRadians(degrees));
					UpdateTransform = true;
				}
			}
		}

		/// <summary>
		/// Primitive color.
		/// </summary>
		public virtual Color Color
		{
			get { return color; }
			set
			{
				if (color != value)
				{
					color = value;
					color.A = (byte)alpha;
					UpdateTransform = true;
				}
			}
		}

		/// <summary>
		/// Color alpha from 0 to 255.
		/// </summary>
		public virtual int Alpha
		{
			get { return alpha; }
			set
			{
				value = MathUtil.Clamp(value, 0, 255);
				if (alpha != value)
				{
					alpha = value;
					color.A = (byte)value;
					UpdateTransform = true;
				}
			}
		}

		internal virtual int _Alpha
		{
			get { return alpha; }
			set
			{
				color.A = (byte)value;
				UpdateTransform = true;
			}
		}

		internal PrimitiveType PrimitiveType
		{
			get { return primitiveType; }
			private set
			{
				primitiveType = value;
				PrimitiveCreated = false;
			}
		}

		public VertexPositionColor[] TransformedVertexPositionColors
		{
			get { return tranformedVPCs; }
		}
		#endregion

		#region Methods
		public void InitializeForDrawing()
		{
			Create();
			UpdateTransformation();
		}

		internal virtual void Create()
		{
			if (!PrimitiveCreated)
			{
				InitializeMatrices();
				ClearVertexData();
				InitializeVertexData();
				PrimitiveCreated = true;
				UpdateTransform = true;
			}
		}

		protected void InitializeMatrices()
		{
			scaleMatrix = Matrix.Scaling(new Vector3(scale, 0f));
			positionMatrix = Matrix.Translation(new Vector3(position, 0));
			originMatrix = Matrix.Translation(new Vector3(-origin, 0));
			rotationMatrix = Matrix.RotationZ(MathUtil.DegreesToRadians(degrees));
		}

		protected void ClearVertexData()
		{
			vertexPositionColors.Clear();
			vertexColors.Clear();
			PolygonCount = 0;
		}

		internal abstract List<PolygonPoint> GetPoints();

		protected virtual Polygon GetPolygon()
		{
			return new Polygon(GetPoints());
		}

		protected void InitializeVertexData()
		{
			if (primitiveType == PrimitiveType.LineList)
			{
				List<PolygonPoint> points = GetPoints();

				foreach (PolygonPoint point in points)
				{
					// It seems silly to not just say "VertexPositionColor v = new VertexPositionColor(new Vector3(vertex, 0), color);"
					// but its actually a JIT optimization to make sure this statement always gets inlined.
					// Reference: Downloaded powerpoint (Understanding XNA Framework Performance) at http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=16477
					VertexPositionColor vpc = new VertexPositionColor();
					vpc.Position.X = (float)point.X;
					vpc.Position.Y = (float)point.Y;

					if (vertexPositionColors.Count > 1)
					{
						vertexPositionColors.Add(lastVertexPositionColor);
					}

					lastVertexPositionColor = vpc;
					vertexPositionColors.Add(vpc);
				}
			}
			else if (primitiveType == PrimitiveType.TriangleList)
			{
				Polygon polygon = GetPolygon();
				P2T.Triangulate(polygon);
				PolygonCount = polygon.Triangles.Count;

				foreach (DelaunayTriangle triangle in polygon.Triangles)
				{
					foreach (TriangulationPoint point in triangle.Points)
					{
						VertexPositionColor vpc = new VertexPositionColor();
						vpc.Position.X = (float)point.X;
						vpc.Position.Y = (float)point.Y;
						vertexPositionColors.Add(vpc);
					}
				}
			}

			tranformedVPCs = vertexPositionColors.ToArray();
		}

		internal Matrix GetTransformation()
		{
			if (UpdateTransform)
			{
				transform = originMatrix * scaleMatrix * rotationMatrix * positionMatrix;
				UpdateTransform = false;
			}
			return transform;
		}

		internal virtual void UpdateTransformation()
		{
			if (UpdateTransform)
			{
				Matrix transform = GetTransformation();

				for (int i = vertexPositionColors.Count; --i >= 0;)
				{
					VertexPositionColor vpc = vertexPositionColors[i];
					Vector3.Transform(ref vpc.Position, ref transform, out vpc.Position);
					vpc.Color = color;
					tranformedVPCs[i] = vpc;
				}

				UpdateTransform = false;
			}
		}

		internal virtual void UpdateTransformation(ref Matrix parentTransform)
		{
			Matrix transform = GetTransformation() * parentTransform;

			for (int i = vertexPositionColors.Count; --i >= 0;)
			{
				VertexPositionColor vpc = vertexPositionColors[i];
				Vector3.Transform(ref vpc.Position, ref transform, out vpc.Position);
				vpc.Color = color;
				tranformedVPCs[i] = vpc;
			}
		}

		/// <summary>
		/// Returns the primitive centroid.
		/// </summary>
		public virtual Vector2 GetCentroid()
		{
			return GetCentroid(GetPoints());
		}

		/// <summary>
		/// Returns the primitive centroid.
		/// </summary>
		public virtual Vector2 GetCentroid(List<PolygonPoint> points)
		{
			Vector2 c = Vector2.Zero;
			float area = 0f;
			int count = points.Count;

			for (int i = 0; i < count; ++i)
			{
				PolygonPoint p1 = points[i];
				PolygonPoint p2 = i + 1 < count ? points[i + 1] : points[0];

				float x1 = (float)p1.X;
				float y1 = (float)p1.Y;
				float x2 = (float)p2.X;
				float y2 = (float)p2.Y;

				float D = x1 * y2 - y1 * x2;
				float triangleArea = 0.5f * D;
				area += triangleArea;

				Vector2 v = Vector2.Zero;
				v.X = x1 + x2;
				v.Y = y1 + y2;

				c += triangleArea * (1f / 3f) * v;
			}

			c *= 1f / area;
			return c;
		}

		/// <summary>
		/// Draws the primitive.
		/// Same as calling PrimitiveBatch.Draw().
		/// </summary>
		public void Draw()
		{
			PrimitiveBatch.Draw(this);
		}
		#endregion

		#region Intersection checking
		/// <summary>
		/// Checks if a position is contained within the primitive.
		/// </summary>
		/// <param name="position">Position to test against.</param>
		public virtual bool Intersects(Vector2 position)
		{
			return Intersects(position.X, position.Y);
		}

		/// <summary>
		/// Checks if a position is contained within the primitive.
		/// </summary>
		/// <param name="x">x coordinat.</param>
		/// <param name="y">y coordinat.</param>
		public virtual bool Intersects(float x, float y)
		{
			if (primitiveType == PrimitiveType.TriangleList)
			{
				Vector2 A = Vector2.Zero;
				Vector2 B = Vector2.Zero;
				Vector2 C = Vector2.Zero;

				int len = tranformedVPCs.Length;
				for (int i = len - 1; i >= 0; i -= 3)
				{
					A.X = tranformedVPCs[i].Position.X;
					A.Y = tranformedVPCs[i].Position.Y;

					B.X = tranformedVPCs[i - 1].Position.X;
					B.Y = tranformedVPCs[i - 1].Position.Y;

					C.X = tranformedVPCs[i - 2].Position.X;
					C.Y = tranformedVPCs[i - 2].Position.Y;

					if (IntersectsTriangle(ref x, ref y, ref A, ref B, ref C))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected bool IntersectsTriangle(ref float posX, ref float posY, ref Vector2 A, ref Vector2 B, ref Vector2 C)
		{
			Vector2 v0 = Vector2.Zero;
			v0.X = C.X - A.X;
			v0.Y = C.Y - A.Y;

			Vector2 v1 = Vector2.Zero;
			v1.X = B.X - A.X;
			v1.Y = B.Y - A.Y;

			Vector2 v2 = Vector2.Zero;
			v2.X = posX - A.X;
			v2.Y = posY - A.Y;

			float dot00;
			float dot01;
			float dot02;
			float dot11;
			float dot12;

			Vector2.Dot(ref v0, ref v0, out dot00);
			Vector2.Dot(ref v0, ref v1, out dot01);
			Vector2.Dot(ref v0, ref v2, out dot02);
			Vector2.Dot(ref v1, ref v1, out dot11);
			Vector2.Dot(ref v1, ref v2, out dot12);

			float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
			float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

			//// Check if point is in triangle
			//return (u > 0) && (v > 0) && (u + v < 1);

			// Check if point is in or on edge of the triangle
			return (u >= 0) && (v >= 0) && (u + v <= 1);
		}
		#endregion
	}
}