using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DemFileFormat.Quake1
{
	/// <summary>
	/// Quake 1 .DEM file reader
	/// 
	/// The unofficial DEM format description
	/// http://www.gamers.org/dEngine/quake/Qdem/dem-1.0.2.html
	/// </summary>
	public class Quake1DemoReader : IDemoReader
	{
		int cameraBindedTo=-1;
		string cdTrack;
		float time=0;
		public void ReadDemo(System.IO.BinaryReader source, DemoDocument dest)
		{
			ReadCDTrack(source);
			ParseBlocks(source);
		}

		private void ParseBlocks(System.IO.BinaryReader source)
		{
			while (source.BaseStream.Length > source.BaseStream.Position)
			{
				uint size = source.ReadUInt32();
				vec3_t angles = new vec3_t();
				angles.x = (int)source.ReadSingle();
				angles.y = (int)source.ReadSingle();
				angles.z = (int)source.ReadSingle();
				var pos = source.BaseStream.Position;
				if (pos + (long)size < 0 || pos + (long)size > source.BaseStream.Length)
					throw new ApplicationException();
				ParseBlockDatas(source, size);
				source.BaseStream.Seek(size + pos, SeekOrigin.Begin);
			}
		}

		private void ParseBlockDatas(BinaryReader source, uint size)
		{
			var pos = source.BaseStream.Position;
			int prevType = -1;
			while (source.BaseStream.Position < pos + (long)size)
			{
				var type = source.ReadByte();
				switch (type)
				{
					case 0:
						throw new ApplicationException();
						break;
					case 0x0B:
						ServerInfo(source);
						break;
					case 0x20:
						CDTrackInfo(source);
						break;
					case 0x05:
						SetView(source);
						break;
					case 0x19:
						signonum(source);
						break;
					case 0x07:
						ReadTime(source);
						break;
					case 0x14:
						spawnstatic(source);
						break;
					case 0x16:
						spawnbaseline(source);
						break;
					case 0x0d:
						UpdateName(source);
						break;
					case 0x0E:
						UpdateFrags(source);
						break;
					case 0x0C:
						LightStyle(source);
						break;
					case 0x11:
						UpdateColors(source);
						break;
					case 0x0A:
						SetCameraAngle(source);
						break;
					case 0x0F:
						ClientData(source);
						break;
						
					case 0x1D:
						spawnstaticsound(source);
						break;
					case 0x1E:
						//intermission
						break;
					case 0x01:
						//nop
						break;
					case 0x02:
						//disconnect
						break;
					case 0x03:
						updatestat(source);
						break;
					default:
						return;
				}
				prevType = type;
			}
		}

		private void ClientData(BinaryReader source)
		{
			var mask = source.ReadUInt16();
			var view_ofs_z = (0!=(mask & 0x0001))?source.ReadSByte():22;
			var ang_ofs_1 = (0 != (mask & 0x0002)) ? (float)source.ReadSByte() : 0.0;
			var angles_0 = (0 != (mask & 0x0004)) ? source.ReadSByte() : 0;
			var vel_0 = (0 != (mask & 0x0020)) ? (source.ReadSByte()) : 0;
			var angles_1 = (0 != (mask & 0x0008)) ? (source.ReadSByte()) : 0;
			var vel_1 = (0 != (mask & 0x0040)) ? source.ReadSByte() : 0;
			var angles_2 = (0 != (mask & 0x0010)) ? source.ReadSByte() : 0;
			var vel_2 = (0 != (mask & 0x0080)) ? source.ReadSByte() : 0;
			var items = (0 != (mask & 0x0200)) ? source.ReadInt32() : 0x4001;
			var uk_bit_b10 = (0 != (mask & 0x0400)) ? 1 : 0; // bit 10 
			var uk_bit_b11 = (0 != (mask & 0x0800)) ? 1 : 0; // bit 11 
			var weaponframe = (0 != (mask & 0x1000)) ? source.ReadByte() : 0;
			var armorvalue = (0 != (mask & 0x2000)) ? source.ReadByte() : 0;
			var weaponmodel = (0 != (mask & 0x4000)) ? source.ReadByte() : 0;
			var health = source.ReadInt16();
			var currentammo = source.ReadByte();
			var ammo_shells = source.ReadByte();
			var ammo_nails = source.ReadByte();
			var ammo_rockets = source.ReadByte();
			var ammo_cells = source.ReadByte();
			var weapon = source.ReadByte();
					
		}

		private void SetCameraAngle(BinaryReader source)
		{
			var x = source.ReadByte();
			var y = source.ReadByte();
			var z = source.ReadByte();
		}

		private void LightStyle(BinaryReader source)
		{
			var style = source.ReadByte();
			var name = ReadString(source, '\0');
		}

		private void UpdateColors(BinaryReader source)
		{
			var player = source.ReadByte();
			var colors = source.ReadByte();
		}

		private void UpdateFrags(BinaryReader source)
		{
			var player = source.ReadByte();
			var frags = source.ReadInt16();
		}

		private void UpdateName(BinaryReader source)
		{
			var player = source.ReadByte();
			var name = ReadString(source, '\0');
		}

		private void ReadTime(BinaryReader source)
		{
			time = source.ReadSingle();
		}

		private void spawnbaseline(BinaryReader source)
		{
			var entity = source.ReadUInt16();
			var default_modelindex = source.ReadByte();
			var default_frame = source.ReadByte();
			var default_colormap = source.ReadByte();
			var default_skin = source.ReadByte();

			var default_originX = source.ReadInt16();
			var default_anglesX = source.ReadSByte();
			var default_originY = source.ReadInt16();
			var default_anglesY = source.ReadSByte();
			var default_originZ = source.ReadInt16();
			var default_anglesZ = source.ReadSByte();
		}

		private void spawnstatic(BinaryReader source)
		{
			var default_modelindex = source.ReadByte();
			var default_frame = source.ReadByte();
			var default_colormap = source.ReadByte();
			var default_skin = source.ReadByte();

			var default_originX = source.ReadInt16();
			var default_anglesX = source.ReadSByte();
			var default_originY = source.ReadInt16();
			var default_anglesY = source.ReadSByte();
			var default_originZ = source.ReadInt16();
			var default_anglesZ = source.ReadSByte();
		}

		private void updatestat(BinaryReader source)
		{
			var i = source.ReadByte();
			var v = source.ReadInt32();
		}

		private void spawnstaticsound(BinaryReader source)
		{
			vec3_t v = new vec3_t();
			v.Read(source);
			var soundnum = source.ReadByte();
			var vol = source.ReadByte();
			var attenuation  = source.ReadByte();
		}

		private void signonum(BinaryReader source)
		{
			int signonum = source.ReadByte();
		}

		private void CDTrackInfo(BinaryReader source)
		{
			int fromtrack = source.ReadByte();
			int totrack = source.ReadByte();
		}
		private void SetView(BinaryReader source)
		{
			cameraBindedTo = source.ReadUInt16();
			
		}
		private void ServerInfo(BinaryReader source)
		{
			int serverversion = source.ReadInt32();
			int maxclients = source.ReadByte();
			int multi = source.ReadByte();
			string level = ReadString(source, '\0');
			List<string> models = new List<string>();
			for (; ; )
			{
				string mdl = ReadString(source, '\0');
				if (string.IsNullOrEmpty(mdl))
					break;
				models.Add(mdl);
			}

			List<string> sounds = new List<string>();
			for (; ; )
			{
				string snd = ReadString(source, '\0');
				if (string.IsNullOrEmpty(snd))
					break;
				sounds.Add(snd);
			}
		}
		private string ReadString(System.IO.BinaryReader source, char term)
		{
			var nameBytes = new List<byte>();
			byte b;
			for (; ; )
			{
				b = source.ReadByte();
				if (b == term)
					break;
				nameBytes.Add(b);
			}
			return Encoding.ASCII.GetString(nameBytes.ToArray());
		}

		private void ReadCDTrack(System.IO.BinaryReader source)
		{
			cdTrack = ReadString(source,(char)0x0A);
		}
	}
}
