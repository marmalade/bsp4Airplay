using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats.Model
{
	public class Cb4aLevelVBCluster : CIwParseable
	{
		public List<Cb4aLevelVBSubcluster> Subclusters = new List<Cb4aLevelVBSubcluster>();
		public int VertexBuffer = 0;
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("vertexbuffer", VertexBuffer);
			writer.WriteKeyVal("num_subclusters", Subclusters.Count);
			foreach (var i in Subclusters)
				i.WrtieToStream(writer);
		}
	}
}
