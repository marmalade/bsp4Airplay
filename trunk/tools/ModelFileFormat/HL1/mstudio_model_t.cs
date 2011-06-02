using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat.HL1
{
	public class mstudio_model_t
	{
		public string name;
		public int type;
		public float boundingradius;
		public int nummesh;
		public int meshindex;
		public int numverts;         // number of unique vertices
		public int vertinfoindex;    // vertex bone info
		public int vertindex;        // vertex vec3_t
		public int numnorms;         // number of unique surface normals
		public int norminfoindex;    // normal bone info
		public int normindex;        // normal vec3_t
		public int numgroups;        // deformation groups
		public int groupindex;
		public byte[] Weights;
		public ReaderUtils.Vector3[] Normals;
		public ReaderUtils.Vector3[] Vertices;
		public List<mstudio_mesh_t> Meshes;
		public void Read(BinaryReader source)
		{
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[] { '\0' });
			type = source.ReadInt32();
			boundingradius = source.ReadSingle();
			nummesh = source.ReadInt32();
			meshindex = source.ReadInt32();
			numverts = source.ReadInt32();
			vertinfoindex = source.ReadInt32();
			vertindex = source.ReadInt32();
			numnorms = source.ReadInt32();
			norminfoindex = source.ReadInt32();
			normindex = source.ReadInt32();
			numgroups = source.ReadInt32();
			groupindex = source.ReadInt32();
		}
	}
}
