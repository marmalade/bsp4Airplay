using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;

namespace ModelFileFormat
{
	public class ModelVertex
	{
		public Vector3 Position = Vector3.Zero;
		public Vector3 Normal = Vector3.UnitZ;
		public Vector2 UV0 = Vector2.Zero;
		public ModelBoneWeight[] Bones;

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Normal.GetHashCode() ^ UV0.GetHashCode() ^ GetBoneWeightHashCode();
		}

		private int GetBoneWeightHashCode()
		{
			if (Bones == null)
				return 0;
			int h = 0;
			foreach (var b in Bones)
			{
				h = h ^ b.Weight.GetHashCode() ^ b.Bone.GetHashCode();
			}
			return h;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ModelVertex))
				return false;

			return this.Equals((ModelVertex)obj);
		}

		public bool Equals(ModelVertex other)
		{
			if (!(Position == other.Position &&
				Normal == other.Normal &&
				UV0 == other.UV0))
				return false;
			foreach (var b1 in Bones)
			{
				foreach (var b2 in other.Bones)
					if (b1.Bone == b2.Bone && b1.Weight == b2.Weight)
						goto nextBone1;
				return false;
				nextBone1:;
			}
			foreach (var b2 in other.Bones)
			{
				foreach (var b1 in Bones)
					if (b1.Bone == b2.Bone && b1.Weight == b2.Weight)
						goto nextBone2;
				return false;
			nextBone2: ;
			}
			return true;
		}
	}
}
