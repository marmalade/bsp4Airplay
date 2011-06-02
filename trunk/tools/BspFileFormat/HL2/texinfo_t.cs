using ReaderUtils;
using System.IO;

namespace BspFileFormat.HL2
{
	public class texinfo_t
	{
		public Vector3 vectorS;            // S vector, horizontal in texture space)
		public float distS;              // horizontal offset in texture space
		public Vector3 vectorT;            // T vector, vertical in texture space
		public float distT;              // vertical offset in texture space

		public Vector3 lm_vectorS;            // S vector, horizontal in texture space)
		public float lm_distS;              // horizontal offset in texture space
		public Vector3 lm_vectorT;            // T vector, vertical in texture space
		public float lm_distT;              // vertical offset in texture space

		public int flags;                  // miptex flags + overrides
		public int texdata;                // Pointer to texture name, size, etc.
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

			lm_vectorS.X = source.ReadSingle();
			lm_vectorS.Y = source.ReadSingle();
			lm_vectorS.Z = source.ReadSingle();
			lm_distS = source.ReadSingle();

			lm_vectorT.X = source.ReadSingle();
			lm_vectorT.Y = source.ReadSingle();
			lm_vectorT.Z = source.ReadSingle();
			lm_distT = source.ReadSingle();

			flags = source.ReadInt32();
			texdata = source.ReadInt32();
		}
	}
}
