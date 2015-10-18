using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace VoxelEngine
{
	[StructLayout( LayoutKind.Sequential )]
	public struct VertexPositionColorTextureNormal
	{
		[VertexElement( "SV_Position" )]
		public Vector3 Position;
		[VertexElement( "COLOR" )]
		public Color Color;
		[VertexElement( "TEXCOORD0" )]
		public Vector2 Texture;
		[VertexElement( "NORMAL" )]
		public Vector3 Normal;

		public VertexPositionColorTextureNormal( Vector3 position, Color color, Vector2 texture, Vector3 normal )
		{
			Position = position;
			Color = color;
			Texture = texture;
			Normal = normal;
		}
	}
}