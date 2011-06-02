using ReaderUtils;
using System.IO;
using System.Collections.Generic;

namespace BspFileFormat.HL2
{
	public class cluster_t
	{
		public int offset;
		public int phs;
		public List<int> lists = new List<int>();
		public List<int> visiblity = new List<int>();
	}
}
