using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DemFileFormat.HL1
{
		public class header_t
			{
				public ulong magic;
				public uint demo_version;
				public uint network_version;
				public string map_name; //0x104
				public string game_dll; //0x108
				public uint dir_offset;

				public void Read(BinaryReader source)
				{
					magic = source.ReadUInt64();
					demo_version = source.ReadUInt32();
					network_version = source.ReadUInt32();
					map_name = Encoding.ASCII.GetString(source.ReadBytes(0x104)).Trim(new char[]{'\0'});
					game_dll = Encoding.ASCII.GetString(source.ReadBytes(0x108)).Trim(new char[]{'\0'});
					dir_offset = source.ReadUInt32();
				}
			}
}
