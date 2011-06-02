using System;
using System.Collections.Generic;
using System.Text;

namespace DemFileFormat.Quake1
{
	public class vec3_t
	{
		public int x;
		public int y;
		public int z;
		public void Read(System.IO.BinaryReader source)
		{
			x = source.ReadInt16();
			y = source.ReadInt16();
			z = source.ReadInt16();
		}
	}
}
