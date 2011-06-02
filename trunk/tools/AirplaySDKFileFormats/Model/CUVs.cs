using System.Collections.Generic;
namespace AirplaySDKFileFormats.Model
{
	public class CUVs : CIwParseable
	{
		public int SetID;
		public IList<CIwVec2> UVs = new List<CIwVec2>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteKeyVal("setID", SetID);
			writer.WriteKeyVal("numUVs", UVs.Count);
			foreach (var p in UVs)
				writer.WriteVec2Fixed("uv", p);
		}
	}
}
