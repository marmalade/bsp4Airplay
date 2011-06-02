using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats.Model
{
	public class LevelVBWriter
	{
		Dictionary<LevelVBItem, int> map = new Dictionary<LevelVBItem, int>();
		Dictionary<Cb4aLevelMaterial, int> materials = new Dictionary<Cb4aLevelMaterial, int>();
		Dictionary<CIwPlane, int> planes = new Dictionary<CIwPlane, int>();
		private Cb4aLevel level;

		public LevelVBWriter(Cb4aLevel level)
		{
			// TODO: Complete member initialization
			this.level = level;
		}
		public int WriteMaterial(Cb4aLevelMaterial m)
		{
			int i;
			if (!materials.TryGetValue(m, out i))
			{
				i = level.Materials.Count;
				level.Materials.Add(m);
				materials[m] = i;
			}
			return i;
		}
		public void PrepareVertexBuffer(int verticesInCluster)
		{
			if (level.VertexBuffers.Count == 0)
			{
				level.VertexBuffers.Add(new Cb4aLevelVertexBuffer());
				return;
			}
			var lastVB = level.VertexBuffers[level.VertexBuffers.Count - 1];
			if (lastVB.vb.Count + verticesInCluster < 65535)
				return;
			level.VertexBuffers.Add(new Cb4aLevelVertexBuffer());
			map.Clear();
		}
		public int Write(LevelVBItem levelVBItem)
		{
			int index;
			if (!map.TryGetValue(levelVBItem, out index))
			{
				var lastVB = level.VertexBuffers[level.VertexBuffers.Count - 1];
				index = lastVB.vb.Count;
				lastVB.vb.Add(levelVBItem);
				map[levelVBItem] = index;
			}
			return index;
		}

		public int WritePlane(CIwPlane plane)
		{
			int index;
			if (!planes.TryGetValue(plane, out index))
			{
				index = level.Planes.Count;
				level.Planes.Add(plane);
				planes[plane] = index;
			}
			return index;
		}

	}
}
