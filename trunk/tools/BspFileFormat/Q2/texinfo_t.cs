using ReaderUtils;
using System.Text;

namespace BspFileFormat.Q2
{
	public class texinfo_t
	{

		public Vector3 vectorS;
		public float distS;

		public Vector3 vectorT;
		public float distT;

		public uint flags;
		public uint value;

		public string name;

		public uint next_texinfo;

		public void Read(System.IO.BinaryReader source)
		{
			vectorS.X = source.ReadSingle();
			vectorS.Y = source.ReadSingle();
			vectorS.Z = source.ReadSingle();
			distS = source.ReadSingle();
			vectorT.X = source.ReadSingle();
			vectorT.Y = source.ReadSingle();
			vectorT.Z = source.ReadSingle();
			distT = source.ReadSingle();
			flags = source.ReadUInt32();
			value = source.ReadUInt32();
			name = Encoding.ASCII.GetString(source.ReadBytes(32)).Trim(new char[] { ' ', '\0' });
			next_texinfo = source.ReadUInt32();
		}
	};
}
