using ReaderUtils;

namespace BspFileFormat.Q1HL1
{
	public class model_t
	{
		public boundbox_t bound;            // The bounding box of the Model
		public Vector3 origin;               // origin of model, usually (0,0,0)
		public uint node_id0;               // index of first BSP node
		public uint node_id1;               // index of the first Clip node
		public uint node_id2;               // index of the second Clip node
		public uint node_id3;               // usually zero
		public uint numleafs;               // number of BSP leaves
		public uint face_id;                // index of Faces
		public uint face_num;               // number of Faces

		public void Read(System.IO.BinaryReader source)
		{
			bound.Read(source);
			origin.X = source.ReadSingle();
			origin.Y = source.ReadSingle();
			origin.Z = source.ReadSingle();
			node_id0 = source.ReadUInt32();
			node_id1 = source.ReadUInt32();
			node_id2 = source.ReadUInt32();
			node_id3 = source.ReadUInt32();
			numleafs = source.ReadUInt32();
			face_id = source.ReadUInt32();
			face_num = source.ReadUInt32();
		}
	}
}
