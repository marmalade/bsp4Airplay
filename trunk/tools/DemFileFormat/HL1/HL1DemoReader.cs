using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DemFileFormat.HL1
{
	/// <summary>
	/// 
	/// http://lmpc.cvs.sourceforge.net/viewvc/lmpc/lmpc/spec/dem-hl.spec?revision=1.1&view=markup
	/// http://lmpc.cvs.sourceforge.net/viewvc/lmpc/lmpc/spec/dem-hl.pl?revision=1.17&view=markup
	/// </summary>
	public class HL1DemoReader : IDemoReader
	{
		header_t header;
		direntry_t[] direntries;
		long fileStartAt = 0;
		public void ReadDemo(BinaryReader source, DemoDocument dest)
		{
			fileStartAt = source.BaseStream.Position;
			ReadHeader(source);

			ReadEntries(source);

			foreach (var de in direntries)
			{
				source.BaseStream.Seek(fileStartAt + de.offset, SeekOrigin.Begin);
				ReadEvents(source,de);
			}
		}

		private void ReadEvents(BinaryReader source, direntry_t de)
		{
			var type = source.ReadByte();
			var time = source.ReadSingle();
			var frame = source.ReadUInt32();

			switch (type)
			{
				case 0:
				case 1:
					ReadUKData(source);
					break;
				case 2:
					//nop;
					break;
				case 3:
					ReadTextMessage(source);
					break;
				case 4:
					source.ReadBytes(32);
					break;
				case 5:
					//last one
					return;
				case 6:
					source.ReadBytes(4+4+4+72);
					break;
				case 7:
					source.ReadBytes(8);
					break;
				case 8:
					ReadSound(source);
					break;
				case 9:
					ReadChunk(source);
					break;
				default:
					//Unknown
					return;
			}
		}

		private void ReadChunk(BinaryReader source)
		{
			var size = source.ReadInt32();
			source.ReadBytes(size);
		}

		private void ReadSound(BinaryReader source)
		{
			var uk_i1 = source.ReadUInt32();
			var sound_name_length = source.ReadInt32();
			var name = Encoding.ASCII.GetString(source.ReadBytes(sound_name_length)).Trim(new char[] { '\0' });
			source.ReadSingle();
			source.ReadSingle();
			source.ReadUInt32();
			source.ReadUInt32();
		}

		private void ReadTextMessage(BinaryReader source)
		{
			var msg = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[] { '\0' });
		}

		private void ReadUKData(BinaryReader source)
		{
			byte[] data;
			if (header.network_version == 42)
				data = source.ReadBytes(560);
			if (header.network_version >= 46)
				data = source.ReadBytes(464);
			var length = source.ReadInt32();
			ParseServerMessage(source.ReadBytes(length));
		}

		private void ParseServerMessage(byte[] message)
		{
			using (var s = new MemoryStream(message))
			{
				using (var source = new BinaryReader(s))
				{
					while (s.Position < message.Length)
					{

						var type = source.ReadByte();
						switch (type)
						{
							case 0x08:
								ServerMessage8(source);
								break;
							case 0x0b:
								ServerInfo(source);
								break;
							case 54:
								SendExtraInfo(source);
								break;
							case 14:
								deltadescription(source);
								break;
							default:
								return;
						}
					}
				}
			}

		}

		private void deltadescription(BinaryReader source)
		{
			var structure = ReadStrZ(source);
			var numEntries = source.ReadUInt16();
			throw new NotSupportedException();
		}
		private void SendExtraInfo(BinaryReader source)
		{
			var text = ReadStrZ(source);
			var uk_byte = source.ReadByte();
		}
		private void ServerInfo(BinaryReader source)
		{
			int serverversion = source.ReadInt32();
			int uk_i1 = source.ReadInt32();
			int uk_i2 = source.ReadInt32();
			source.ReadBytes(16);
			var maxclients = source.ReadByte();
			var uk_b4 = source.ReadByte();
			var uk_b5 = source.ReadByte();
			var gameDir = ReadStrZ(source);
			if (header.network_version >= 46)
			{
				var remotehost = ReadStrZ(source);
			}
			var map1 = ReadStrZ(source);
			var map2 = ReadStrZ(source);
			var extraflag = source.ReadByte();
			if (extraflag > 0)
			{
				var extralength = source.ReadByte();
				var extra = source.ReadBytes(extralength);
				var extra2 = source.ReadBytes(16);
			}
		}

		private void ServerMessage8(BinaryReader source)
		{
			var msg = ReadStrZ(source);
		}

		private static string ReadStrZ(BinaryReader source)
		{
			var d = new List<byte>(16);
			for (; ; )
			{
				var b = source.ReadByte();
				if (0 == b)
					break;
				d.Add(b);
			}
			return Encoding.ASCII.GetString(d.ToArray());
		}



		private void ReadEntries(BinaryReader source)
		{
			source.BaseStream.Seek(fileStartAt + header.dir_offset, SeekOrigin.Begin);
			var numDirEntries = source.ReadUInt32();
			direntries = new direntry_t[numDirEntries];
			for (uint i = 0; i < numDirEntries; ++i)
			{
				direntries[i] = new direntry_t();
				direntries[i].Read(source);
			}
		}

		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
	}
}
