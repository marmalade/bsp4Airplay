using System;
using System.Collections.Generic;
using System.Text;
using ReaderUtils;
using System.IO;

namespace BspFileFormat.HL2
{
	public struct dentry_t
	{
		public uint offset;
		public uint size;
		public uint version;
		public uint magic;

		internal void Read(System.IO.BinaryReader source, bool isVersion21)
		{
			if (isVersion21)
			{
				version = source.ReadUInt32();
				offset = source.ReadUInt32();
				size = source.ReadUInt32();
				magic = source.ReadUInt32();
			}
			else
			{
				offset = source.ReadUInt32();
				size = source.ReadUInt32();
				version = source.ReadUInt32();
				magic = source.ReadUInt32();
			}
		}
	}
}
