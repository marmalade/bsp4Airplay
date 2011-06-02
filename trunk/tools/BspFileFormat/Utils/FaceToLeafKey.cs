using System;
using System.Collections.Generic;
using System.Text;

namespace BspFileFormat.Utils
{
	public class FaceToLeafKey
	{
		public Dictionary<int,bool> Leaves = new Dictionary<int,bool>();
		public int Index;
		internal void AddLeaf(int i)
		{
			Leaves[i] = true;
		}

		public override int GetHashCode()
		{
			int res = 0;
			foreach (var i in Leaves)
				res = res ^ i.Key.GetHashCode();
			return res;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is FaceToLeafKey))
				return false;

			return this.Equals((FaceToLeafKey)obj);
		}
		public bool Equals(FaceToLeafKey other)
		{
			if ((object)other == null)
				return false;
			if (Leaves.Count != other.Leaves.Count)
				return false;
			foreach (var kv in Leaves)
				if (!other.Leaves.ContainsKey(kv.Key))
					return false;
			foreach (var kv in other.Leaves)
				if (!Leaves.ContainsKey(kv.Key))
					return false;
			return true;
		}
		public static bool operator ==(FaceToLeafKey left, FaceToLeafKey right)
		{
			if ((object)left == null)
				return ((object)right == null);
			return left.Equals(right);
		}
		public static bool operator !=(FaceToLeafKey left, FaceToLeafKey right)
		{
			return !left.Equals(right);
		}
	}
}
