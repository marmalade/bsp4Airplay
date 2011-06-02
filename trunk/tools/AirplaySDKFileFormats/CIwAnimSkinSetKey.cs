using AirplaySDKFileFormats.Model;
using System.Collections.Generic;
using System.Globalization;

namespace AirplaySDKFileFormats
{
	public struct CIwAnimSkinSetKey
	{
		public IList<string> Bones;

		public override int GetHashCode()
		{
			if (Bones == null)
				return 0;
			int code = 0;
			foreach (var s in Bones)
				code ^= s.GetHashCode();
			return code;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is CIwAnimSkinSetKey))
				return false;

			return this.Equals((CIwAnimSkinSetKey)obj);
		}
		public bool Equals(CIwAnimSkinSetKey other)
		{
			if (Bones == null && other.Bones == null)
				return true;
			if (Bones == null || other.Bones == null)
				return false;
			foreach (var b in Bones)
				if (other.GetBoneIndex(b) < 0)
					return false;
			foreach (var b in other.Bones)
				if (GetBoneIndex(b) < 0)
					return false;
			return true;
		}

		public int GetBoneIndex(string b)
		{
			if (Bones == null || b == null)
				return -1;
			for (int i = 0; i < Bones.Count; ++i)
				if (Bones[i] == b)
					return i;
			return -1;
		}
		public static bool operator ==(CIwAnimSkinSetKey left, CIwAnimSkinSetKey right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(CIwAnimSkinSetKey left, CIwAnimSkinSetKey right)
		{
			return !left.Equals(right);
		}

		internal void WrtieBodyToStream(CTextWriter writer)
		{
			writer.BeginWriteLine();
			writer.Write("useBones { ");
			if (Bones != null)
			{
				foreach (var w in Bones)
					writer.Write(string.Format(CultureInfo.InvariantCulture, "{0} ", w));
			}
			writer.Write("}");
			writer.EndWriteLine();
		}
	}
}
