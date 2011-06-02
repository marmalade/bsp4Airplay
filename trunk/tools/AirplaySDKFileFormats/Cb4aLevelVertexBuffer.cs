using System;
using System.Globalization;
using System.Collections.Generic;
using AirplaySDKFileFormats.Model;

namespace AirplaySDKFileFormats
{
	public class Cb4aLevelVertexBuffer : CIwParseable
	{
		public List<LevelVBItem> vb = new List<LevelVBItem>();
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			Dictionary<CIwVec3, int> posMap = new Dictionary<CIwVec3, int>();
			List<CIwVec3> posList = new List<CIwVec3>();

			Dictionary<CIwVec3, int> normMap = new Dictionary<CIwVec3, int>();
			List<CIwVec3> normList = new List<CIwVec3>();

			var uvMap = new Dictionary<CIwVec2, int>();
			var uvList = new List<CIwVec2>();

			var colMap = new Dictionary<CIwColour, int>();
			var colList = new List<CIwColour>();

			var tris = new List<CTrisVertex>();

			foreach (var l in vb)
			{
				tris.Add(
					new CTrisVertex(
						GetIndex(l.Position, posMap, posList),
						GetIndex(l.Normal, normMap, normList),
						GetIndex(l.UV0, uvMap, uvList),
						GetIndex(l.UV1, uvMap, uvList),
						GetIndex(l.Colour, colMap, colList)	));
			}

			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_pos", posList.Count);
			foreach (var v in posList)
				writer.WriteVec3("v",v);
			writer.WriteKeyVal("num_n", normList.Count);
			foreach (var v in normList)
				writer.WriteVec3("vn", v);
			writer.WriteKeyVal("num_uvs", uvList.Count);
			foreach (var v in uvList)
				writer.WriteVec2("uv", v);
			writer.WriteKeyVal("num_cols", colList.Count);
			foreach (var v in colList)
				writer.WriteColour("col", v);
				
			writer.WriteKeyVal("num_vertices", tris.Count);
			foreach (var v in tris)
			{
				writer.WriteLine(string.Format("i {{{0},{1},{2},{3},{4}}}", v.pos, v.n, v.uv0, v.uv1, v.color));
			}
		}

		private int GetIndex<T>(T key, Dictionary<T, int> posMap, List<T> posList)
		{
			int i;
			if (posMap.TryGetValue(key, out i))
				return i;
			i = posList.Count;
			posList.Add(key);
			posMap[key] = i;
			return i;
		}
	}
}
