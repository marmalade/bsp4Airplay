using ReaderUtils;
using System.IO;

namespace BspFileFormat.HL2
{
	public class dtexdata_t
	{
		public Vector3 reflectivity;           // RGB reflectivity 	
		public int nameStringTableID;      // index into TexdataStringTable
		public int width, height;         	// source image
		public int view_width, view_height;

		public string name; //calculated

		public void Read(System.IO.BinaryReader source)
		{
			reflectivity.X = source.ReadSingle();
			reflectivity.Y = source.ReadSingle();
			reflectivity.Z = source.ReadSingle();
			nameStringTableID = source.ReadInt32();
			width = source.ReadInt32();
			height = source.ReadInt32();
			view_width = source.ReadInt32();
			view_height = source.ReadInt32();
		}
	};
}
