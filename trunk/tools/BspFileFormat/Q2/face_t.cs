using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;

namespace BspFileFormat.Q2
{
		public class face_t
			{
				public ushort   plane;             // index of the plane the face is parallel to
				public ushort plane_side;        // set if the normal is parallel to the plane normal

				public uint ledge_id;        // index of the first edge (in the face edge array)
				public ushort ledge_num;         // number of consecutive edges (in the face edge array)

				public ushort texture_info;      // index of the texture info structure	

				public byte[] lightmap_syles; // styles (bit flags) for the lightmaps
				public int lightmap;   // offset of the lightmap (in bytes) in the lightmap lump

				public void Read(System.IO.BinaryReader source)
				{
					plane = source.ReadUInt16();
					plane_side = source.ReadUInt16();
					ledge_id = source.ReadUInt32();
					ledge_num = source.ReadUInt16();
					texture_info = source.ReadUInt16();
					lightmap_syles = source.ReadBytes(4);
					lightmap = source.ReadInt32();
				}
			};
}
