using SharpDX;

namespace VoxelEngine
{
	/// <summary>
	/// Voxel Triangle class
	/// </summary>
	public struct Triangle
	{
		// Define the values for storage of each corner
		public Vector3 pointOne;
		public Vector3 pointTwo;
		public Vector3 pointThree;

		public Triangle( Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree )
		{
			this.pointOne = pointOne;
			this.pointTwo = pointTwo;
			this.pointThree = pointThree;
		}
	}
}