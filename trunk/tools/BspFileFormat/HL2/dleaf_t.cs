using ReaderUtils;
using System.IO;
using BspFileFormat.BspMath;

namespace BspFileFormat.HL2
{
		public class dleaf_t
		{
			public int               contents;               // OR of all brushes (not needed?)
			public short             cluster;                // cluster this leaf is in
			public short             area_flags;                 // area this leaf is in
			public bboxshort_t box;
			public ushort    firstleafface;          // index into leaffaces
			public ushort    numleaffaces;
			public ushort    firstleafbrush;         // index into leafbrushes
			public ushort    numleafbrushes;
			public short             leafWaterDataID;        // -1 for not in water
			public CompressedLightCube ambientLighting;  // Precaculated light info for entities.
			public short padding;                // padding to 4-byte boundary
				
		};
		public class dleaf_17t : dleaf_t
		{
			public void Read(BinaryReader source)
			{
				contents = source.ReadInt32();               // OR of all brushes (not needed?)
				cluster = source.ReadInt16();               // cluster this leaf is in
				area_flags = source.ReadInt16();                 // area this leaf is in
				box.Read(source);
				firstleafface = source.ReadUInt16();          // index into leaffaces
				numleaffaces = source.ReadUInt16();
				firstleafbrush = source.ReadUInt16();         // index into leafbrushes
				numleafbrushes = source.ReadUInt16();
				leafWaterDataID = source.ReadInt16();        // -1 for not in water
				padding = source.ReadInt16();                // padding to 4-byte boundary
			}
		}
		public class dleaf_19t : dleaf_t
		{
			public void Read(BinaryReader source)
			{
				contents = source.ReadInt32();               // OR of all brushes (not needed?)
				cluster = source.ReadInt16();                // cluster this leaf is in
				area_flags = source.ReadInt16();                 // area this leaf is in
				box.Read(source);
				firstleafface = source.ReadUInt16();          // index into leaffaces
				numleaffaces = source.ReadUInt16();
				firstleafbrush = source.ReadUInt16();         // index into leafbrushes
				numleafbrushes = source.ReadUInt16();
				leafWaterDataID = source.ReadInt16();        // -1 for not in water
				ambientLighting.Read(source);  // Precaculated light info for entities.
				padding = source.ReadInt16();                // padding to 4-byte boundary
			}
		}
		public struct CompressedLightCube
		{
			public byte[] Data;

			public void Read(BinaryReader source)
			{
				Data = source.ReadBytes(24);
			}
		}
}
