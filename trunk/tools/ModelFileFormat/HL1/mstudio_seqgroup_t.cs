using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;

namespace ModelFileFormat.HL1
{
	public class mstudio_seqgroup_t
	{
		public string label;
		public string name;
		int cache;
		public int data;
		public void Read(BinaryReader source)
		{
			label = Encoding.ASCII.GetString(source.ReadBytes(32)).Trim(new char[] { '\0' });
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[] { '\0' });
			cache = source.ReadInt32();
			data = source.ReadInt32();
		}
	}
}
