using System.Collections.Generic;
namespace AirplaySDKFileFormats.Model
{
	public class CVertNorms : CIwParseable
	{
		public IList<CIwVec3> Normals = new List<CIwVec3>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteKeyVal("numVertNorms", Normals.Count);
			foreach (var p in Normals)
				writer.WriteVec3Fixed("vn", p);
		}

	}
}
