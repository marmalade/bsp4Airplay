using System.Collections.Generic;
namespace AirplaySDKFileFormats.Model
{
	public class CVertCols : CIwParseable
	{
		public IList<CIwColour> Colours = new List<CIwColour>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteKeyVal("numVertCols", Colours.Count);
			foreach (var p in Colours)
				writer.WriteArray("col", new int[]{p.r,p.g,p.b,p.a});
		}

	}
}
