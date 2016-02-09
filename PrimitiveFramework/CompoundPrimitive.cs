using System;
using System.Collections.Generic;
using Poly2Tri;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace DXPrimitiveFramework
{
	public class CompoundPrimitive : Primitive
	{
		private List<Primitive> primitives;

		public CompoundPrimitive()
		{
			primitives = new List<Primitive>();
		}

		public CompoundPrimitive(CompoundPrimitive compoundPrimitive) : base(compoundPrimitive)
		{
			primitives = new List<Primitive>(compoundPrimitive.primitives.Count);
			foreach (Primitive primitive in compoundPrimitive.primitives)
			{
				Primitive p = Activator.CreateInstance(primitive.GetType(), primitive) as Primitive;
				primitives.Add(p);
			}
		}

		#region Properties
		public Primitive this[int index]
		{
			get { return primitives[index]; }
			set { primitives[index] = value; }
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
				if (color != value)
				{
					primitives.ForEach(o => o.Color = value);
					color = value;
					color.A = (byte)alpha;
					UpdateTransform = true;
				}
			}
		}

		/// <summary>
		/// Color alpha from 0 to 255. Child primitive alpha's are set relative to the parent.
		/// </summary>
		public override int Alpha
		{
			get { return alpha; }
			set
			{
				value = MathUtil.Clamp(value, 0, 255);
				if (alpha != value)
				{
					foreach (Primitive primitive in primitives)
					{
						primitive._Alpha = (int)(primitive._Alpha * value * (1f / 255f));
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
				foreach (Primitive primitive in primitives)
				{
					primitive._Alpha = (int)(primitive._Alpha * value * (1f / 255f)); ;
				}
				color.A = (byte)value;
				UpdateTransform = true;
			}
		}

		public override Vector2 TransformedScale
		{
			get { return base.TransformedScale; }
			internal set
			{
				var diff = value - base.TransformedScale;
				base.TransformedScale = value;
				primitives.ForEach(p => p.TransformedScale += diff);
			}
		}

		public override float TransformedDegrees
		{
			get { return base.TransformedDegrees; }
			internal set
			{
				var diff = value - base.TransformedDegrees;
				base.TransformedDegrees = value;
				primitives.ForEach(p => p.TransformedDegrees += diff);
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Adds a primitive to the compound.
		/// </summary>
		public void Add(Primitive primitive)
		{
			primitives.Insert(0, primitive);
		}

		/// <summary>
		/// Removes a primitive from the compound.
		/// </summary>
		public void Remove(Primitive primitive)
		{
			primitives.Remove(primitive);
		}

		/// <summary>
		/// Removes all primitives from the compound.
		/// </summary>
		public void Clear()
		{
			primitives.Clear();
		}

		internal override void Create()
		{
			if (!PrimitiveCreated)
			{
				primitives.ForEach(o => o.Create());
				InitializeMatrices();
				PrimitiveCreated = true;
				UpdateTransform = true;
			}
		}

		internal override void UpdateTransformation()
		{
			if (UpdateTransform)
			{
				Matrix transform = GetTransformation();
				List<VertexPositionColor> vpc = new List<VertexPositionColor>();

				foreach (Primitive primitive in primitives)
				{
					Matrix t = primitive.GetTransformation() * transform;
					primitive.UpdateTransformation(ref t);
					vpc.AddRange(primitive.TransformedVertexPositionColors);
				}

				tranformedVPCs = vpc.ToArray();
				UpdateTransform = false;
			}
		}

		internal override void UpdateTransformation(ref Matrix transform)
		{
			List<VertexPositionColor> vpc = new List<VertexPositionColor>();

			foreach (Primitive primitive in primitives)
			{
				primitive.UpdateTransformation(ref transform);
				vpc.AddRange(primitive.TransformedVertexPositionColors);
			}

			tranformedVPCs = vpc.ToArray();
		}

		internal override List<PolygonPoint> GetPoints(float thickness = 0)
		{
			throw new NotImplementedException();
		}

		public override bool Intersects(float x, float y)
		{
			foreach (Primitive primitive in primitives)
			{
				if (primitive.Intersects(x, y))
				{
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}