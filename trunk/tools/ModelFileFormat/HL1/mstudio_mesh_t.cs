using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat.HL1
{
	public class mstudio_mesh_t
	{
		public int numtris;
		public int triindex;
		public int skinref;
		public int numnorms;       // per mesh normals
		public int index;      // normal vec3_t

		public List<ModelFace> Faces = new List<ModelFace>();


		public void Read(BinaryReader source)
		{
			numtris = source.ReadInt32();
			triindex = source.ReadInt32();
			skinref = source.ReadInt32();
			numnorms = source.ReadInt32();
			index = source.ReadInt32();
		}
	}
}
