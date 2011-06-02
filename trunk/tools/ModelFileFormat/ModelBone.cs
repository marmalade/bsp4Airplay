using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;

namespace ModelFileFormat
{
	public class ModelBone
	{
		public string Name;
		ModelBone parent;
		public ModelBone Parent
		{
			get { return parent; }
		}

		public Vector3 Position;
		public Quaternion Rotaton;

		public Vector3 WorldPosition;
		public Quaternion WorldRotaton;

		public List<ModelBone> childBones = new List<ModelBone>();

		public List<ModelBone> ChildBones { get { return childBones; } }

		internal Vector3 Transform(Vector3 pos)
		{
			return Vector3.Transform(pos,WorldRotaton) + WorldPosition;
		}

		internal void EvalMatrix(List<ModelBone> modelBones)
		{
			WorldPosition = Position;
			WorldRotaton = Rotaton;
			var modelBone = Parent;
			while (modelBone != null)
			{
				WorldPosition = Vector3.Transform(WorldPosition, modelBone.Rotaton) + modelBone.Position;
				WorldRotaton = modelBone.Rotaton * WorldRotaton;
				modelBone = modelBone.Parent;
			}
		}

		public void AddChild(ModelBone bone)
		{
			bone.parent = this;
			childBones.Add(bone);
		}
	}
}
