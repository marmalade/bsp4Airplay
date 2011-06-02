using ReaderUtils;
using System.Collections.Generic;
namespace BspFileFormat
{
	public class BspCollisionFaceSoupFace
	{
		public List<Vector3> Vertices = new List<Vector3>();
	}
	public class BspCollisionFaceSoup : BspCollisionObject
	{
		public List<BspCollisionFaceSoupFace> Faces = new List<BspCollisionFaceSoupFace>();
	}
}
