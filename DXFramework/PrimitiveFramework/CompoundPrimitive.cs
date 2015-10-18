using System;
using System.Collections.Generic;
using System.ComponentModel;
using Poly2Tri;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.PrimitiveFramework
{
	public class CompoundPrimitive : Primitive
	{
		private const float inv255 = 1f / 255f;
		private List<Primitive> primitives;

		public CompoundPrimitive()
		{
			primitives = new List<Primitive>();
		}

		public CompoundPrimitive( CompoundPrimitive compoundPrimitive )
			: base( compoundPrimitive )
		{
			primitives = new List<Primitive>( compoundPrimitive.primitives.Count );
			foreach( Primitive primitive in compoundPrimitive.primitives )
			{
				Primitive p = Activator.CreateInstance( primitive.GetType(), primitive ) as Primitive;
				primitives.Add( p );
			}
		}

		#region Properties
		public Primitive this[ int index ]
		{
			get { return primitives[ index ]; }
			set { primitives[ index ] = value; }
		}

		/// <summary>
		/// List of primitives contained in this compound.
		/// </summary>
		public List<Primitive> Primitives
		{
			get { return primitives; }
		}

		/// <summary>
		/// Primitive color.
		/// </summary>
		public override Color Color
		{
			get { return color; }
			set
			{
				if( color != value )
				{
					primitives.ForEach( o => o.Color = value );
					color = value;
					color.A = (byte)alpha;
					UpdateTransform = true;
				}
			}
		}

		/// <summary>
		/// Color alpha from 0 to 255. Child primitive alpha's are set relative to the parrent.
		/// </summary>
		public override int Alpha
		{
			get { return alpha; }
			set
			{
				value = (int)MathUtil.Clamp( value, 0, 255 );
				if( alpha != value )
				{
					foreach( Primitive primitive in primitives )
					{
						primitive._Alpha = (int)( primitive._Alpha * value * inv255 );
					}
					alpha = value;
					color.A = (byte)value;
					UpdateTransform = true;
				}
			}
		}

		internal override int _Alpha
		{
			get { return alpha; }
			set
			{
				foreach( Primitive primitive in primitives )
				{
					primitive._Alpha = (int)( primitive._Alpha * value * inv255 ); ;
				}
				color.A = (byte)value;
				UpdateTransform = true;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Adds a primitive to the compound.
		/// </summary>
		public void Add( Primitive primitive )
		{
			primitives.Add( primitive );
		}

		/// <summary>
		/// Removes a primitive from the compound.
		/// </summary>
		public void Remove( Primitive primitive )
		{
			primitives.Remove( primitive );
		}

		/// <summary>
		/// Removes all primitives from the compound.
		/// </summary>
		public void Clear()
		{
			primitives.Clear();
		}

		/// <summary>
		/// GetCentroid() is not supported on compound primitives.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public override Vector2 GetCentroid()
		{
			throw new NotSupportedException( "GetCentroid() is not supported on compound primitives." );
		}

		internal override void Create()
		{
			if( !primitiveCreated )
			{
				primitives.ForEach( o => o.Create() );
				InitializeMatrices();
				primitiveCreated = true;
				UpdateTransform = true;
			}
		}

		internal override void UpdateTransformation()
		{
			if( UpdateTransform )
			{
				Matrix transform = GetTransformation();
				List<VertexPositionColor> vpc = new List<VertexPositionColor>();

				foreach( Primitive primitive in primitives )
				{
					primitive.UpdateTransformation( ref transform );
					vpc.AddRange( primitive.TransformedVertexPositionColors );
				}

				tranformedVPCs = vpc.ToArray();
				UpdateTransform = false;
			}
		}

		internal override void UpdateTransformation( ref Matrix parentTransform )
		{
			Matrix transform = GetTransformation() * parentTransform;
			List<VertexPositionColor> vpc = new List<VertexPositionColor>();

			foreach( Primitive primitive in primitives )
			{
				primitive.UpdateTransformation( ref transform );
				vpc.AddRange( primitive.TransformedVertexPositionColors );
			}

			tranformedVPCs = vpc.ToArray();
		}

		internal override List<PolygonPoint> GetPoints()
		{
			throw new NotImplementedException();
		}

		public override bool Intersects( float x, float y )
		{
			foreach( Primitive primitive in primitives )
			{
				if( primitive.Intersects( x, y ) )
				{
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}