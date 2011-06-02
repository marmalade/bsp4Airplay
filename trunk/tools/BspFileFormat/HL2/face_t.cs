using ReaderUtils;
using System.IO;

namespace BspFileFormat.HL2
{
	public class face_t
	{
		public ushort planenum;		// the plane number
		public byte side;			// faces opposite to the node's plane direction
		public byte onNode; 			// 1 of on node, 0 if in leaf
		public int firstedge;			// index into surfedges	
		public short numedges;			// number of surfedges
		public short texinfo;			// texture info
		public short dispinfo;			// displacement info
		public short surfaceFogVolumeID;		// ?	
		public byte[] styles;			// switchable lighting info[4]
		public int lightmap;			// offset into lightmap lump
		public float area;				// face area in units^2
		public int[] LightmapTextureMinsInLuxels;   // texture lighting info
		public int[] LightmapTextureSizeInLuxels;   // texture lighting info
		public int origFace;			// original face this was split from
		public ushort numPrims;		// primitives
		public ushort firstPrimID;
		public uint smoothingGroups;	// lightmap smoothing group

		/// <summary>
		/// Model ID
		/// Used to filter faces in leaves since model faces are belong to models
		/// </summary>
		public int modelId = 0;
		// this define the start of the face light map

	}
	public class face_19t:face_t
	{
		public void Read(BinaryReader source)
		{
			planenum = source.ReadUInt16();		// the plane number
			side = source.ReadByte();			// faces opposite to the node's plane direction
			onNode = source.ReadByte(); 			// 1 of on node, 0 if in leaf
			firstedge = source.ReadInt32();			// index into surfedges	
			numedges = source.ReadInt16();			// number of surfedges
			texinfo = source.ReadInt16();			// texture info
			dispinfo = source.ReadInt16();			// displacement info
			surfaceFogVolumeID = source.ReadInt16();		// ?	
			styles = source.ReadBytes(4);			// switchable lighting info[4]
			lightmap = source.ReadInt32();			// offset into lightmap lump
			area = source.ReadSingle();				// face area in units^2
			LightmapTextureMinsInLuxels = new int[] { source.ReadInt32(), source.ReadInt32() };   // texture lighting info
			LightmapTextureSizeInLuxels = new int[] { source.ReadInt32(), source.ReadInt32() };   // texture lighting info
			origFace = source.ReadInt32();			// original face this was split from
			numPrims = source.ReadUInt16();		// primitives
			firstPrimID = source.ReadUInt16();
			smoothingGroups = source.ReadUInt32();	// lightmap smoothing group
		}
	}
	public class face_17t : face_t
	{
		public void Read(BinaryReader source)
		{
			source.ReadBytes(4);
			planenum = source.ReadUInt16();		// the plane number
			side = source.ReadByte();			// faces opposite to the node's plane direction
			onNode = source.ReadByte(); 			// 1 of on node, 0 if in leaf

			firstedge = source.ReadInt32();			// index into surfedges	
			numedges = source.ReadInt16();			// number of surfedges
			texinfo = source.ReadInt16();			// texture info
			dispinfo = source.ReadInt16();			// displacement info
			source.ReadBytes(50);
			origFace = source.ReadInt32();			// original face this was split from
			smoothingGroups = source.ReadUInt32();	// lightmap smoothing group
		}
	}
}
