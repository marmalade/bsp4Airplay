using AirplaySDKFileFormats.Model;
using System.Collections.Generic;
using System.Globalization;

namespace AirplaySDKFileFormats
{
	public class CIwAnimSkinSetVertWeights
	{
		public int Vertex;
		public float[] Weights;

		internal void WrtieBodyToStream(CTextWriter writer)
		{
			writer.BeginWriteLine();
			writer.Write("vertWeights {");
			writer.Write(Vertex.ToString());
			foreach (var w in Weights)
				writer.Write(string.Format(CultureInfo.InvariantCulture, ",{0}", w));
			writer.Write("}");
			writer.EndWriteLine();
		}
	}
}
