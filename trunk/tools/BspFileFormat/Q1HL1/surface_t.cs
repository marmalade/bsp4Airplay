using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;

namespace BspFileFormat.Q1HL1
{
	public class surface_t
	{
		public Vector3 vectorS;            // S vector, horizontal in texture space)
		public float distS;              // horizontal offset in texture space
		public Vector3 vectorT;            // T vector, vertical in texture space
		public float distT;              // vertical offset in texture space
		public uint texture_id;         // Index of Mip Texture
		//           must be in [0,numtex[
		public uint animated;           // 0 for ordinary textures, 1 for water 

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
			texture_id = source.ReadUInt32();
			animated = source.ReadUInt32();
		}
	} ;
}
