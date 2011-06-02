using AirplaySDKFileFormats.Model;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class CIwModel : CIwResource
	{
		public List<CMesh> ModelBlocks = new List<CMesh>();
		public CIwVec3 mins;
		public CIwVec3 maxs;
		public CIwAnimSkin Skin;

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			
			foreach (var mesh in ModelBlocks)
				mesh.WrtieToStream(writer);
		}
	}
}
