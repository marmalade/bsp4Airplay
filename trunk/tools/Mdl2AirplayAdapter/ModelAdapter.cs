using System;
using System.Collections.Generic;
using System.Text;
using ModelFileFormat;
using AirplaySDKFileFormats;
using AirplaySDKFileFormats.Model;
using ReaderUtils;

namespace Mdl2AirplayAdapter
{
	public class ModelAdapter
	{
		private CIwResGroup group;
		private CIwModel modelMesh;
		private ModelWriter writer;
		float scale = 1;
		public void Convert(ModelDocument model, CIwResGroup group)
		{
			this.group = group;
			this.modelMesh = new CIwModel();
			modelMesh.Name = model.Name;

			group.AddRes(new CIwTexture() { FilePath = "../textures/checkers.png" });
			group.AddRes(modelMesh);

            var mesh = new CMesh();
			mesh.Scale = 1.0f / scale;
			modelMesh.ModelBlocks.Add(mesh);
			writer = new ModelWriter(modelMesh);

			WriteSkeleton(model);
			WriteMaterials(model);
			//WriteMesh(model.Meshes[1]);
			foreach (var m in model.Meshes)
			{
				WriteMesh(m);
				//break;
			}
			WriteAnimations(model,modelMesh);


		}

		private void WriteAnimations(ModelDocument model, CIwModel modelMesh)
		{
			if (model.Animations == null || model.Animations.Count == 0)
				return;
			modelMesh.Skin.Animations = new List<CIwAnim>();
			foreach (var a in model.Animations)
			{
				var anim = new CIwAnim() { Name = a.Name, skeleton=modelMesh.Skin.skeleton };
				if (a.Frames != null)
				foreach (var f in a.Frames)
				{
					var frame = new CIwAnimKeyFrame();
					frame.time = f.Time;
					foreach (var b in f.Bones)
					{
						var bone = new CIwAnimKeyFrameBone();
						bone.bone = b.Bone.Name;
						bone.pos = GetVec3(b.Position);
						bone.rot = GetQuat(b.Rotation);
						frame.bones.Add(bone);
					}
					anim.KeyFrames.Add(frame);
				}
				modelMesh.Skin.Animations.Add(anim);
			}
		}

		private void WriteSkeleton(ModelDocument model)
		{
			foreach (var bone in model.Bones)
				FixBoneName(bone);
			foreach (var bone in model.Bones)
				WriteBone(bone);
		}

		private void FixBoneName(ModelBone bone)
		{
			bone.Name = FixBoneName(bone.Name);
			foreach (var b in bone.ChildBones)
				FixBoneName(b);
		}

		private void WriteBone(ModelBone bone)
		{
			writer.WriteBone(new CIwAnimBone() { Name = bone.Name, parent = (bone.Parent != null) ? bone.Parent.Name : null, pos = GetVec3(bone.Position), rot = GetQuat(bone.Rotaton) });
			foreach (var b in bone.ChildBones)
				WriteBone(b);
		}

		private string FixBoneName(string p)
		{
			return p.Replace(' ','_');
		}

		private CIwQuat GetQuat(Quaternion quaternion)
		{
			return new CIwQuat(
				(quaternion.W),
				(quaternion.X),
				(quaternion.Y),
				(quaternion.Z)
				);
		}

		private void WriteMaterials(ModelDocument model)
		{
			var textures = new Dictionary<ModelTexture,bool>();
			foreach (var mesh in model.Meshes)
				foreach (var face in mesh.Faces)
					if (!textures.ContainsKey(face.Texture))
					{
						textures[face.Texture] = true;
						if (face.Texture is ModelEmbeddedTexture)
							group.AddRes(new CIwTexture() { FilePath = "../textures/" + face.Texture.Name + ".png", Bitmap = ((ModelEmbeddedTexture)face.Texture).Bitmap });
						else
							group.AddRes(new CIwTexture() { FilePath = "../textures/" + face.Texture.Name+".png" });
						group.AddRes(new CIwMaterial() { Texture0 = face.Texture.Name, Name = face.Texture.Name });
					}
		}

		private void WriteMesh(ModelMesh mesh)
		{
			foreach (var face in mesh.Faces)
			{
				var surface = writer.GetSurfaceIndex(face.Texture.Name);

				var srcV = face.Vertex0;
				var v0 = BuildVertex(srcV);
				srcV = face.Vertex1;
				var v1 = BuildVertex(srcV);
				srcV = face.Vertex2;
				var v2 = BuildVertex(srcV);

				writer.AddTriangle(surface, v0, v1, v2);
			}
		}

		private CTrisVertex BuildVertex(ModelVertex srcV)
		{
			return writer.GetVertex(
				GetVec3(srcV.Position * scale),
				GetBones(srcV.Bones),
 				GetWeights(srcV.Bones),
				GetVec3Fixed(srcV.Normal), 
				GetVec2Fixed(srcV.UV0), 
				CIwVec2.g_Zero, 
				CIwColour.White);
		}

		private IList<float> GetWeights(IList<ModelBoneWeight> modelBoneWeight)
		{
			var bones = new float[modelBoneWeight.Count];
			for (int i = 0; i < bones.Length; ++i)
				bones[i] = modelBoneWeight[i].Weight;
			return bones;
		}

		private CIwAnimSkinSetKey GetBones(IList<ModelBoneWeight> modelBoneWeight)
		{
			var bones = new string [modelBoneWeight.Count];
			for (int i=0; i<bones.Length; ++i)
				bones[i] = modelBoneWeight[i].Bone.Name;
			return new CIwAnimSkinSetKey() { Bones=bones };
		}

		public void Convert(string modelFilePath, string groupFilePath)
		{
			var doc = ModelDocument.Load(modelFilePath);
			var group = new CIwResGroup();
			group.Name = doc.Name;
			Convert(doc, group);
			using (var w = new CTextWriter(groupFilePath))
			{
				group.WrtieToStream(w);
				w.Close();
			}
		}

		private int GetFixed(float x)
		{
			return (int)(x * AirplaySDKMath.IW_GEOM_ONE);
		}

		private CIwVec3 GetVec3Fixed(ReaderUtils.Vector3 vector3)
		{
			return new CIwVec3(
				(int)(vector3.X * AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Y * AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Z * AirplaySDKMath.IW_GEOM_ONE)
				);
		}
		private CIwColour GetColour(System.Drawing.Color col)
		{
			return new CIwColour() { r = col.R, g = col.G, b = col.B, a = col.A };
		}
		private CIwVec2 GetVec2Fixed(Vector2 vector3)
		{
			return new CIwVec2(
				(int)(vector3.X * AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Y * AirplaySDKMath.IW_GEOM_ONE)
				);
		}

		private CIwVec3 GetVec3(Vector3 vector3)
		{
			return new CIwVec3(
				(int)(vector3.X),
				(int)(vector3.Y),
				(int)(vector3.Z)
				);
		}
	}
}
