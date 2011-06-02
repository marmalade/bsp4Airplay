using System;
using System.Globalization;
using System.Collections.Generic;
using AirplaySDKFileFormats.Model;

namespace AirplaySDKFileFormats
{
	public class Cb4aLeaf : Cb4aTreeElement
	{
		public List<int> VisibleLeaves = new List<int>();
		public List<int> Clusters = new List<int>();
		public List<Ib4aCollider> Colliders = new List<Ib4aCollider>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			
			if (Clusters.Count > 0)
			{
				writer.WriteKeyVal("num_visible_clusters", Clusters.Count);
				writer.BeginWriteLine();
				writer.Write("visible_clusters");
				foreach (var l in Clusters)
				{
					writer.Write(string.Format(" {0}", l));
				}
				writer.EndWriteLine();
			}
			if (VisibleLeaves.Count > 0)
			{
				writer.WriteKeyVal("num_visible_leaves", VisibleLeaves.Count);
				writer.BeginWriteLine();
				writer.Write("visible_leaves");
				foreach (var l in VisibleLeaves)
				{
					writer.Write(string.Format(" {0}", l));
				}
				writer.EndWriteLine();
			}
			foreach (var l in Colliders)
			{
				((CIwParseable)l).WrtieToStream(writer);
			}
		}

	
		public void SortClusters(Cb4aLevel level)
		{
			if (Clusters.Count == 0) return;
			var ar = Clusters.ToArray();
			Array.Sort(ar, (x, y) => {
				var a = level.subclusters[x];
				var b = level.subclusters[y];
				if (a.VertexBuffer != b.VertexBuffer)
					return a.VertexBuffer - b.VertexBuffer;
				if (a.Material != b.Material)
					return a.Material - b.Material;
				return 0;
			});
			Clusters.Clear();
			Clusters.AddRange(ar);
		}
	}
}
