using System;
using System.Collections.Generic;
using System.Text;

namespace BspFileFormat.Utils
{
	public class FaceToLeafCluster
	{
		public FaceToLeafKey Key;
		public int Index;
		public List<int> Faces = new List<int>();
	}
	public class FaceToLeafMap
	{
		public List<FaceToLeafKey> Faces;

		public FaceToLeafMap(int size)
		{
			Faces = new List<FaceToLeafKey>(size);
			while (size > 0)
			{
				Faces.Add(new FaceToLeafKey());
				--size;
			}
		}

		public List<FaceToLeafCluster> FindUniqueKeys()
		{
			FaceToLeafCluster prev = null;
			Dictionary<FaceToLeafKey, FaceToLeafCluster> unique = new Dictionary<FaceToLeafKey, FaceToLeafCluster>();
			List<FaceToLeafCluster> res = new List<FaceToLeafCluster>();
			for (int index=0; index<Faces.Count;++index)
			{
				var f = Faces[index];
				FaceToLeafCluster i;
				if (prev != null && f == prev.Key)
				{
					i = prev;
				}
				else
				{
					if (!unique.TryGetValue(f, out i))
					{
						i = new FaceToLeafCluster() { Index = res.Count, Key=f };
						unique.Add(f, i);
						res.Add(i);
					}
				}
				i.Faces.Add(index);
				prev = i;
			}
			return res;
		}
	}
}
