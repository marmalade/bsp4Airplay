using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats.Model
{
	public class CVerts : CIwParseable
	{
		public IList<CIwVec3> Positions = new List<CIwVec3>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteKeyVal("numVerts", Positions.Count);
			foreach (var p in Positions)
				writer.WriteVec3("v", p);
		}
	}
}
