using SharpDX;

namespace VoxelEngine
{
	/// <summary>
	/// The GridCell class, stores a position and density for each vertex
	/// </summary>
	public struct GridCell
	{
		// Define the position and value (density) arrays
		public Vector3[] p;
		public float[] val;

		/// <summary>
		/// Create a GridCell
		/// </summary>
		/// <param name="positions">An array of positions containing each corner of the GridCell</param>
		/// <param name="values">And array of values containing the density value at each corner of the GridCell</param>
		public GridCell( Vector3[] positions, float[] values )
		{
			p = positions;
			val = values;
		}
	}
}