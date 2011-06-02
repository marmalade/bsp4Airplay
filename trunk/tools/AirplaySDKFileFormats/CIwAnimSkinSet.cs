using AirplaySDKFileFormats.Model;
using System.Collections.Generic;
using System.Globalization;

namespace AirplaySDKFileFormats
{
	public class CIwAnimSkinSet : CIwParseable
	{
		public CIwAnimSkinSetKey useBones;
		public List<CIwAnimSkinSetVertWeights> vertWeights = new List<CIwAnimSkinSetVertWeights>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);

			useBones.WrtieBodyToStream(writer);
			writer.WriteKeyVal("numVerts", vertWeights.Count);
			foreach (var vw in vertWeights)
			{
				vw.WrtieBodyToStream(writer);
			}
		}
	}
}
