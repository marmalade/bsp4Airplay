using ReaderUtils;

namespace BspFileFormat.Q3
{
	public class brush_t
	{
		public int brushside;
		public int n_brushsides;
		public int texture;


		public void Read(System.IO.BinaryReader source)
		{
			brushside = source.ReadInt32();
			n_brushsides = source.ReadInt32();
			texture = source.ReadInt32();
		}
	}
	public class brushside_t
	{
		public int plane;
		public int texture;


		public void Read(System.IO.BinaryReader source)
		{
			plane = source.ReadInt32();
			texture = source.ReadInt32();
		}
	}
}
