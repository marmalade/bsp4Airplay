using ReaderUtils;
using System;

namespace BspFileFormat.Q3
{
	public class vertex_t
	{
		public Vector3 vPosition;      // (x, y, z) position. 
		public Vector2 vTextureCoord;  // (u, v) texture coordinate
		public Vector2 vLightmapCoord; // (u, v) lightmap coordinate
		public Vector3 vNormal;        // (x, y, z) normal vector
		public byte[] color;           // RGBA color for the vertex [4]

		public void Read(System.IO.BinaryReader source)
		{
			vPosition.X = source.ReadSingle();
			vPosition.Y = source.ReadSingle();
			vPosition.Z = source.ReadSingle();
			vTextureCoord.X = source.ReadSingle();
			vTextureCoord.Y = source.ReadSingle();
			vLightmapCoord.X = source.ReadSingle();
			vLightmapCoord.Y = source.ReadSingle();
			if (vLightmapCoord.X < 0) vLightmapCoord.X = 0;
			if (vLightmapCoord.X > 1) vLightmapCoord.X = 1;
			if (vLightmapCoord.Y < 0) vLightmapCoord.Y = 0;
			if (vLightmapCoord.Y > 1) vLightmapCoord.Y = 1;
			vNormal.X = source.ReadSingle();
			vNormal.Y = source.ReadSingle();
			vNormal.Z = source.ReadSingle();
			if (vNormal.LengthSquared < 0.9f || vNormal.LengthSquared > 1.1f)
				throw new ApplicationException("Probably wrong format of vertex");
			color = source.ReadBytes(4);
		}
	}
}
