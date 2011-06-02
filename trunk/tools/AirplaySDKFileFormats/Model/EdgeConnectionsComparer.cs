using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats.Model
{
	public class TrisConnectionsComparer : IComparer<StripTriangle>
	{
		#region IComparer<StripEdge> Members

		public int Compare(StripTriangle x, StripTriangle y)
		{
			return x.Connections - y.Connections;
		}

		#endregion
	}
}
