using System.Collections.Generic;
using SharpDX;

namespace VoxelEngine.Primitives
{
	/// <summary>
	/// Geometric primitive class for drawing cubes.
	/// </summary>
	public class VoxelChunk: GeometricPrimitive
	{
		public List<Vector3> borderCubes;

		public VoxelChunk()
		{
			borderCubes = new List<Vector3>();
		}

		internal void RemoveBorderCubes( BoundingBox box )
		{
			//box.Max = box.Max - Vector3.One;//???
			borderCubes.RemoveAll( v => box.Contains( v ) == ContainmentType.Contains );
		}
	}
}