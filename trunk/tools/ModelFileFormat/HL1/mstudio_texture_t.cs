using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat.HL1
{
	public class mstudio_texture_t
	{
		public string name;
		public int flags;
		public int width;
		public int height;
		public int index;
		public void Read(BinaryReader source)
		{
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[] { '\0' });
			flags = source.ReadInt32();
			width = source.ReadInt32();
			height = source.ReadInt32();
			index = source.ReadInt32();
		}
	}
}
