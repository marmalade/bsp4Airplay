using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats
{
	public class CIwParseable
	{
		public virtual bool  ParseAttribute (CIwTextParserITX pParser, string pAttrName)
		{
			throw new ApplicationException("Unknown attribute");
		}
		public virtual void ParseClose(CIwTextParserITX pParser)
		{
		}
		public virtual void ParseOpen(CIwTextParserITX pParser)
		{
		}
		public virtual void WrtieToStream(CTextWriter writer)
		{
			writer.OpenChild(this.GetType().Name);
			WrtieBodyToStream(writer);
			writer.CloseChild();
		}
		public virtual void WrtieBodyToStream(CTextWriter writer)
		{
		}

	}
}
