using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aEntity: CIwParseable
	{
		public string classname;
		public CIwVec3 origin;
		public List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteString("classname", classname);
			writer.WriteVec3("origin", origin);
			foreach (var val in values)
			{
				writer.WriteString(val.Key, val.Value);
			}
		}
	}
}
