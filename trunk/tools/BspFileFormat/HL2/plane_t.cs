using ReaderUtils;
using System.IO;

namespace BspFileFormat.HL2
{
	public class plane_t
	{
		public Vector3 normal;     // normal vector
		public float dist;       // distance from origin
		public int type;       // plane axis identifier

		public void Read(BinaryReader source)
		{
			normal.X = source.ReadSingle();
			normal.Y = source.ReadSingle();
			normal.Z = source.ReadSingle();
			dist = source.ReadSingle();
			type = source.ReadInt32();
		}
	};
}
