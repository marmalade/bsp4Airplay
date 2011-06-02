using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat.HL1
{
	public class mesh_vertex_t
	{
		public int v;
		public int n;
		public int s;
		public int t;
		public void Read(BinaryReader source)
		{
			v = source.ReadInt16();
			n = source.ReadInt16();
			s = source.ReadInt16();
			t = source.ReadInt16();
		}
	}
}
