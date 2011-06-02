using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aTreeElement : CIwParseable
	{
		public int Index;
		public CIwVec3 mins;
		public CIwVec3 maxs;
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteVec3("mins", mins);
			writer.WriteVec3("maxs", maxs);
		}
	}
}
