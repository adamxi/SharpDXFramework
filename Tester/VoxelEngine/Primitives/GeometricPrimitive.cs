using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace VoxelEngine.Primitives
{
	/// <summary>
	/// Base class for simple geometric primitive models. This provides a vertex
	/// buffer, an index buffer, plus methods for drawing the model. Classes for
	/// specific types of primitive (CubePrimitive, SpherePrimitive, etc.) are
	/// derived from this common base, and use the AddVertex and AddIndex methods
	/// to specify their geometry.
	/// </summary>
	public abstract class GeometricPrimitive
	{
		public GeometricPrimitive()
		{
			Scale = Vector3.One;
			Vertices = new List<VertexPositionColorTextureNormal>();
			Indices = new List<short>();
			NeedRecalculation = true;
		}

		private Vector3 position;

		public Vector3 Position
		{
			get { return position; }
			set
			{
				if( position != value )
				{
					position = value;
					NeedRecalculation = true;
				}
			}
		}

		public Vector3 Scale { get; set; }

		public List<VertexPositionColorTextureNormal> Vertices { get; set; }

		public List<short> Indices { get; set; }

		public VertexPositionColorTextureNormal[] VerticeArray { get; set; }

		public short[] IndiceArray { get; set; }

		public bool NeedRecalculation { get; set; }

		public int VertexCount
		{
			get { return Vertices.Count; }
		}

		/// <summary>
		/// Queries the index of the current vertex. This starts at
		/// zero, and increments every time AddVertex is called.
		/// </summary>
		protected int CurrentVertex
		{
			get { return VertexCount; }
		}

		public void Update()
		{
			if( NeedRecalculation )
			{
				NeedRecalculation = false;

				Matrix m = Matrix.Translation( position );

				VerticeArray = new VertexPositionColorTextureNormal[ VertexCount ];

				for( int i = VertexCount; --i >= 0; )
				{
					VertexPositionColorTextureNormal vpc = Vertices[ i ];
					Vector4 v4;
					Vector3.Transform( ref vpc.Position, ref m, out v4 );
					vpc.Position = new Vector3( v4.X, v4.Y, v4.Z );
					VerticeArray[ i ] = vpc;
				}

				//VerticeArray = Vertices.ToArray();
				IndiceArray = Indices.ToArray();
			}
		}

		/// <summary>
		/// Adds a new vertex to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		internal void AddVertex( Vector3 position, Vector3 normal )
		{
			Vertices.Add( new VertexPositionColorTextureNormal( position, Color.White, Vector2.Zero, normal ) );
		}

		/// <summary>
		/// Adds a new index to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		internal void AddIndex( int index )
		{
			Indices.Add( (short)index );
		}

		internal void Clear()
		{
			Indices.Clear();
			Vertices.Clear();
		}

		/// <summary>
		/// Remove vertexes specified by space from vertex and index lists
		/// </summary>
		/// <param name="box"></param>
		internal void Remove( BoundingBox box )
		{
			int first = FindTriangle( box );
			while( -1 != first )
			{
				var firstInd = (int)Indices[ first ];
				Vertices.RemoveRange( (int)Indices[ first ], 3 );

				Indices.RemoveRange( first, 3 );

				for( int i = 0; i < Indices.Count; i++ )
				{
					if( Indices[ i ] >= firstInd )
						Indices[ i ] -= 3;
				}
				first = FindTriangle( box );
			}
		}

		/// <summary>
		/// find tringle. Slow!
		/// </summary>
		/// <param name="box"></param>
		/// <returns>array index of firts point Index</returns>
		private int FindTriangle( BoundingBox box )
		{
			for( int i = 0; i < Indices.Count - 3; i += 3 )
			{
				if( box.Contains( Vertices[ (int)Indices[ i ] ].Position ) == ContainmentType.Contains
					&& box.Contains( Vertices[ (int)Indices[ i + 1 ] ].Position ) == ContainmentType.Contains
					&& box.Contains( Vertices[ (int)Indices[ i + 2 ] ].Position ) == ContainmentType.Contains )
				{
					return i;
				}
			}
			return -1;
		}
	}
}