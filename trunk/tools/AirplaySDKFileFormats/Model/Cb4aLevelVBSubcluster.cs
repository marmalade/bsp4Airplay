using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace AirplaySDKFileFormats.Model
{
	public class Cb4aLevelVBSubcluster : CIwParseable
	{
		public List<int> Indices = new List<int>();
		public int Material=0;
		public int VertexBuffer = 0;
		public CIwVec3 Mins;
		public CIwVec3 Maxs;
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("material", Material);
			writer.WriteVec3("mins", Mins);
			writer.WriteVec3("maxs", Maxs);
			writer.WriteKeyVal("vb", VertexBuffer);
			IList<int> res = Indices;
			//res = BuildStrip(Indices);
			writer.WriteKeyVal("num_indices", res.Count);
			foreach (var i in res)
				writer.WriteKeyVal("t", i);
		}

		private IList<int> BuildStrip(List<int> Indices)
		{
			var strip = new List<int>();
			var tris = new List<StripTriangle>();
			var edges = new List<StripEdge>();
			var map = new Dictionary<StripEdge, int>();
			for (int i = 0; i < Indices.Count; i += 3)
			{
				var t = new StripTriangle();
				t.index = tris.Count;
				t.verts = new int[] { Indices[i], Indices[i + 1], Indices[i + 2] };
				tris.Add(t);
				AddEdge(new StripEdge(Indices[i], Indices[i + 1]), map, edges, t);
				AddEdge(new StripEdge(Indices[i + 1], Indices[i + 2]), map, edges, t);
				AddEdge(new StripEdge(Indices[i + 2], Indices[i]), map, edges, t);
			}
			var sortedTris = tris.ToArray();
			Array.Sort(sortedTris, new TrisConnectionsComparer());
			foreach (var e in edges)
			{
				var t = e.tris.ToArray();
				Array.Sort(t, new TrisConnectionsComparer());
				e.tris.Clear();
				e.tris.AddRange(t);
			}
			int left = sortedTris.Length - 1;
			StripTriangle next = sortedTris[0];
			AddFirst(strip, next);
			while (left > 0)
			{
				int lastEdgeIndex;
				if (!map.TryGetValue(new StripEdge(strip[strip.Count - 1], strip[strip.Count - 2]), out lastEdgeIndex))
					if (!map.TryGetValue(new StripEdge(strip[strip.Count - 2], strip[strip.Count - 1]), out lastEdgeIndex))
					{
						throw new ApplicationException(String.Format("No edge of {0},{1}, last tris {2}", strip[strip.Count - 1], strip[strip.Count - 2], next));
					}
				var e = edges[lastEdgeIndex];
				next = null;
				foreach (var t in e.tris)
					if (!t.used)
					{
						next = t;
						break;
					}
				if (next != null)
				{
					next.used = true;
					--left;
					foreach (var nextE in next.edges)
						if (nextE.v0 == strip[strip.Count - 1])
						{
							strip.Add(nextE.v1);
							break;
						}
					continue;
				}
				int pos = 0;
				while (sortedTris[pos].used) ++pos;
				--left;
				next = sortedTris[pos];
				JumpTo(strip, next);
			}
			return strip;
		}

		private void JumpTo(List<int> strip, StripTriangle tris)
		{
			strip.Add(strip[strip.Count - 1]);
			int maxEdge = (tris.edges[0].Connections < tris.edges[1].Connections) ? 0 : 1;
			if (tris.edges[maxEdge].Connections < tris.edges[2].Connections) maxEdge = 2;
			tris.used = true;
			foreach (var i in tris.verts)
				if (i != tris.edges[maxEdge].v0 && i != tris.edges[maxEdge].v1)
				{
					strip.Add(i);
					AddTriangleFrom(strip, tris, i);
				}
		}

		private void AddFirst(List<int> strip, StripTriangle tris)
		{
			int maxEdge = (tris.edges[0].Connections < tris.edges[1].Connections) ? 0 : 1;
			if (tris.edges[maxEdge].Connections < tris.edges[2].Connections) maxEdge=2;
			tris.used = true;
			foreach (var i in tris.verts)
				if (i!= tris.edges[maxEdge].v0 && i != tris.edges[maxEdge].v1)
					AddTriangleFrom(strip, tris, i);
		}

		private static void AddTriangleFrom(List<int> strip, StripTriangle tris, int startIndex)
		{
			if (startIndex == tris.verts[0])
			{
				strip.Add(tris.verts[0]);
				strip.Add(tris.verts[1]);
				strip.Add(tris.verts[2]);
			}
			else if (startIndex == tris.verts[1])
			{
				strip.Add(tris.verts[1]);
				strip.Add(tris.verts[2]);
				strip.Add(tris.verts[0]);
			}
			else if (startIndex == tris.verts[2])
			{
				strip.Add(tris.verts[2]);
				strip.Add(tris.verts[0]);
				strip.Add(tris.verts[1]);
			}
		}

		private static void AddEdge(StripEdge stripEdge, Dictionary<StripEdge, int> map, List<StripEdge> edges, StripTriangle t)
		{
			int i;
			if (!map.TryGetValue(stripEdge, out i))
			{
				i = edges.Count;
				stripEdge.tris = new List<StripTriangle>();
				edges.Add(stripEdge);
				map[stripEdge] = i;
			}
			else
			{
				stripEdge = edges[i];
			}
			
			stripEdge.tris.Add(t);
			t.edges.Add(stripEdge);
		}

		
	}
	public class StripTriangle
	{
		public int index;
		public bool used;
		public List<StripEdge> edges = new List<StripEdge>();
		public int[] verts;
		public int Connections { get { return Math.Max(Math.Max(edges[0].Connections, edges[1].Connections), edges[2].Connections); } }
	}
	public class StripEdge
	{
		public int v0;
		public int v1;
		public List<StripTriangle> tris;
		public int Connections { get { return tris.Count; } }
		

		public StripEdge(int p, int p_2)
		{
			this.v0 = p;
			this.v1 = p_2;
		}

		public override int GetHashCode()
		{
			return v0.GetHashCode() ^ v1.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (!(obj is StripEdge))
				return false;

			return this.Equals((StripEdge)obj);
		}
		public bool Equals(StripEdge other)
		{
			return
				(v0 == other.v0 &&
				v1 == other.v1) || (v1 == other.v0 &&
				v0 == other.v1);
		}
		public static bool operator ==(StripEdge left, StripEdge right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(StripEdge left, StripEdge right)
		{
			return !left.Equals(right);
		}
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{{{0},{1}}}", v0, v1);
		}
	}
}
