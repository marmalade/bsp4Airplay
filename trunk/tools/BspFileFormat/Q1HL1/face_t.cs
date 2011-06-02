using System.IO;

namespace BspFileFormat.Q1HL1
{
	public class face_t
	{
		public ushort plane_id;            // The plane in which the face lies
		//           must be in [0,numplanes[ 
		public ushort side;                // 0 if in front of the plane, 1 if behind the plane
		public int ledge_id;               // first edge in the List of edges
		//           must be in [0,numledges[
		public ushort ledge_num;           // number of edges in the List of edges
		public ushort texinfo_id;          // index of the Texture info the face is part of
		//           must be in [0,numtexinfos[ 
		public byte typelight;            // type of lighting, for the face
		public byte baselight;            // from 0xFF (dark) to 0 (bright)
		public byte[] light;             // two additional light models  [2]
		public int lightmap;               // Pointer inside the general light map, or -1

		/// <summary>
		/// Model ID
		/// Used to filter faces in leaves since model faces are belong to models
		/// </summary>
		public int modelId = 0;
		// this define the start of the face light map

		public void Read(BinaryReader source)
		{
			plane_id = source.ReadUInt16();
			side = source.ReadUInt16();
			ledge_id = source.ReadInt32();
			ledge_num = source.ReadUInt16();
			texinfo_id = source.ReadUInt16();

			typelight = source.ReadByte();
			baselight = source.ReadByte();
			light = source.ReadBytes(2);

			lightmap = source.ReadInt32();
		}
	} ;
}
