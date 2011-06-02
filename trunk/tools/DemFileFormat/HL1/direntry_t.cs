using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DemFileFormat.HL1
{
		public class direntry_t
			{
				public uint number;
				string title;
				public uint flags;
				public int play;
				public float time;
				public uint frames;
				public uint offset;
				public uint length;

				public void Read(BinaryReader source)
				{
					number = source.ReadUInt32();
					title = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[]{'\0'});
					flags = source.ReadUInt32();
					play = source.ReadInt32();
					time = source.ReadSingle();
					frames = source.ReadUInt32();
					offset = source.ReadUInt32();
					length = source.ReadUInt32();
				}
			}
}
