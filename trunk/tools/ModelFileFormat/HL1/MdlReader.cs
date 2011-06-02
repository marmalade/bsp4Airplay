using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;
using System.Drawing;

namespace ModelFileFormat.HL1
{
	/// <summary>
	/// Sample reader: http://www.google.com/codesearch/p?hl=ru#Dss3okGmsW8/plugins/modules/model/halflife/HalfLife.cpp&q=mdl%20halflife&sa=N&cd=6&ct=rc
	/// </summary>
	public class MdlReader: IModelReader
	{
		header_t header;
		private long startOfTheFile;
		private List<mstudio_texture_t> textures;
		private List<Bitmap> textureImages = new List<Bitmap>();
		private List<ModelTexture> modelTextures = new List<ModelTexture>();
		private List<mstudio_bodyparts_t> bodyParts;
		private List<mstudio_bone_t> bones;
		private List<ModelBone> modelBones = new List<ModelBone>();
		private List<mstudio_seq_desc_t> sequences;
		private List<mstudio_seqgroup_t> seqgroups;
		#region IModelReader Members

		public void ReadModel(BinaryReader source, ModelDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadTextures(source);
			ReadBones(source);
			ReadBodyParts(source);
			ReadAnimations(source);
			//dest.Name = header.name;

			foreach (var bp in bodyParts)
				dest.Meshes.Add(BuildMesh(bp));
			BuildAnimations(dest);
			BuildBones(dest);
		}

		private void BuildBones(ModelDocument dest)
		{
			foreach (var bone in modelBones)
				if (bone.Parent == null)
					dest.Bones.Add(bone);
		}

		private void BuildAnimations(ModelDocument dest)
		{
			foreach (var seq in sequences)
			{
				var a = new ModelAnimation() { Name=seq.label };
				if (seq.blends != null)
				{
					int blend = 0;
					//IList<ModelAnimationFrameBone>[] = b
					var f = new ModelAnimationFrame() { Time=0};
					a.Frames.Add(f);
					for (int bone = 0; bone < bones.Count; ++bone)
					{
						mstudioanim_t animBlend = seq.blends[blend, bone];
						f.Bones.Add(new ModelAnimationFrameBone() {Bone=modelBones[bone],Position=modelBones[bone].Position,Rotation=modelBones[bone].Rotaton });
					}
					f = new ModelAnimationFrame() { Time = 1 };
					a.Frames.Add(f);
					for (int bone = 0; bone < bones.Count; ++bone)
					{
						mstudioanim_t animBlend = seq.blends[blend, bone];
						f.Bones.Add(new ModelAnimationFrameBone() { Bone = modelBones[bone], Position = modelBones[bone].Position, Rotation = modelBones[bone].Rotaton });
					}
				}
				dest.Animations.Add(a);
			}
		}

		private void ReadAnimations(BinaryReader source)
		{
			seqgroups = ReaderHelper.ReadStructs<mstudio_seqgroup_t>(source, (uint)header.numseqgroups * 104, header.seqgroupindex + startOfTheFile, 104);
			sequences = ReaderHelper.ReadStructs<mstudio_seq_desc_t>(source, (uint)header.numseq * 176, header.seqindex + startOfTheFile, 176);
			foreach (mstudio_seq_desc_t seq in sequences)
			{
				var group = seqgroups[seq.seqgroup];
				source.BaseStream.Seek(startOfTheFile + group.data + seq.animindex, SeekOrigin.Begin);
				seq.blends = new mstudioanim_t[seq.numblends, bones.Count];
				for (int frame = 0; frame < seq.numblends; ++frame)
				{
					for (int bone = 0; bone < bones.Count; ++bone)
					{
						var b = new mstudioanim_t();
						b.Read(source);
						seq.blends[frame, bone] = b;
					}
				}
				for (int blend = 0; blend < seq.numblends; ++blend)
				{
					for (int bone = 0; bone < bones.Count; ++bone)
					{
						mstudioanim_t animBlend = seq.blends[blend, bone];
						for (int i = 0; i < 6; ++i)
						{
							ushort offset = animBlend.offset[i];
							if (offset > 0)
							{
								if (animBlend.values[i] == null)
									animBlend.values[i] = new List<mstudioanimvalue_t>();
								source.BaseStream.Seek(startOfTheFile + group.data + seq.animindex + offset, SeekOrigin.Begin);
								var framesCount = seq.numframes;
								while (framesCount > 0)
								{
									var v = new mstudioanimvalue_t();
									v.Read(source);
									animBlend.values[i].Add(v);
									framesCount -= v.total;
								}
							}
						}
					}
				}
			}

		}

		private void ReadBones(BinaryReader source)
		{
			bones = ReaderHelper.ReadStructs<mstudio_bone_t>(source, (uint)header.numbones * 112, header.boneindex + startOfTheFile, 112);
			foreach (var b in bones)
			{
				modelBones.Add(new ModelBone(){
					Name = b.name,
					Position = new Vector3(b.value[0], b.value[1], b.value[2]),
					Rotaton = BuildQuaternion(b)
				});
			}
			for (int boneIndex = 0; boneIndex < bones.Count; ++boneIndex)
			{
				var bone = modelBones[boneIndex];
				if (bones[boneIndex].parent >= 0)
				{
					modelBones[bones[boneIndex].parent].AddChild(bone);
				}
			}
			foreach (var bone in modelBones)
			{
				bone.EvalMatrix(modelBones);
			}
		}

		private Quaternion BuildQuaternion(mstudio_bone_t b)
		{
			var ax = b.value[3];
			var ay = b.value[4];
			var az = b.value[5];
			return 
				Quaternion.FromAxisAngle(Vector3.UnitZ, az)*
				Quaternion.FromAxisAngle(Vector3.UnitY, ay)*
				Quaternion.FromAxisAngle(Vector3.UnitX, ax);

			//return Quaternion.FromAxisAngle(Vector3.UnitX, ax) *
			//    Quaternion.FromAxisAngle(Vector3.UnitY, ay) *
			//    Quaternion.FromAxisAngle(Vector3.UnitZ, az);
		}

		private ModelMesh BuildMesh(mstudio_bodyparts_t bp)
		{
			var mesh = new ModelMesh();
			BuildMesh(bp, mesh);
			return mesh;
		}

		private void BuildMesh(mstudio_bodyparts_t bp, ModelMesh mesh)
		{
			foreach (var m in bp.Models)
				BuildMesh(m, mesh);
		}

		private void BuildMesh(mstudio_model_t mdl, ModelMesh mesh)
		{
			foreach (var m in mdl.Meshes)
				BuildMesh(m, mesh);
		}

		private void BuildMesh(mstudio_mesh_t src, ModelMesh mesh)
		{
			foreach (var face in src.Faces)
				mesh.Faces.Add(face);
		}
		private void ReadBodyParts(BinaryReader source)
		{
			if (header.numbodyparts == 0)
				return;
			bodyParts = ReaderHelper.ReadStructs<mstudio_bodyparts_t>(source, (uint)header.numbodyparts * 76, header.bodypartindex + startOfTheFile, 76);

			foreach (var bp in bodyParts)
			{
				bp.Models = ReaderHelper.ReadStructs<mstudio_model_t>(source, (uint)bp.nummodels * 112, 
					bp.modelindex + startOfTheFile, 112);

				foreach (var mdl in bp.Models)
				{
					if (mdl.vertinfoindex != 0)
					{
						source.BaseStream.Seek(mdl.vertinfoindex + startOfTheFile, SeekOrigin.Begin);
						mdl.Weights = source.ReadBytes(mdl.numverts);
					}
					Vector3 mins = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
					Vector3 maxs = new Vector3(float.MinValue, float.MinValue, float.MinValue);
					if (mdl.vertindex != 0)
					{
						mdl.Vertices = new Vector3[mdl.numverts];
						source.BaseStream.Seek(mdl.vertindex + startOfTheFile, SeekOrigin.Begin);
						for (int i = 0; i < mdl.numverts; ++i)
						{
							mdl.Vertices[i].X = source.ReadSingle();
							mdl.Vertices[i].Y = source.ReadSingle();
							mdl.Vertices[i].Z = source.ReadSingle();

							if (mins.X > mdl.Vertices[i].X) mins.X = mdl.Vertices[i].X;
							if (mins.Y > mdl.Vertices[i].Y) mins.Y = mdl.Vertices[i].Y;
							if (mins.Z > mdl.Vertices[i].Z) mins.Z = mdl.Vertices[i].Z;
							if (maxs.X < mdl.Vertices[i].X) maxs.X = mdl.Vertices[i].X;
							if (maxs.Y < mdl.Vertices[i].Y) maxs.Y = mdl.Vertices[i].Y;
							if (maxs.Z < mdl.Vertices[i].Z) maxs.Z = mdl.Vertices[i].Z;
						}
					}
					if (mdl.numnorms != 0)
					{
						mdl.Normals = new Vector3[mdl.numnorms];
						source.BaseStream.Seek(mdl.normindex + startOfTheFile, SeekOrigin.Begin);
						for (int i = 0; i < mdl.numnorms; ++i)
						{
							mdl.Normals[i].X = source.ReadSingle();
							mdl.Normals[i].Y = source.ReadSingle();
							mdl.Normals[i].Z = source.ReadSingle();
							var l = mdl.Normals[i].Length;
							if (Math.Abs(l - 1) > 0.1)
							{
								throw new ArgumentException("Normal length is not 1");
							}
						}
					}
					mdl.Meshes = ReaderHelper.ReadStructs<mstudio_mesh_t>(source, (uint)mdl.nummesh * 20,
						mdl.meshindex + startOfTheFile, 20);

					foreach (var mesh in mdl.Meshes)
					{
						source.BaseStream.Seek(mesh.triindex + startOfTheFile, SeekOrigin.Begin);
						int countTris=0;
						for (; countTris < mesh.numtris; )
						{
							int numVertices = source.ReadInt16();
							if (numVertices == 0)
								break;
							if (numVertices < 0)
							{
								countTris += -numVertices -2;
								ReadTrianglesFan(source, mesh, mdl, -numVertices);
							}
							else
							{
								countTris += numVertices-2;
								ReadTrianglesStrip(source, mesh, mdl, numVertices);
							}
							
						}

					}
				}
			}
		}

		private void ReadTrianglesStrip(BinaryReader source, mstudio_mesh_t mesh, mstudio_model_t mdl, int numVertices)
		{
			
			mesh_vertex_t v0 = new mesh_vertex_t();
			v0.Read(source);
			mesh_vertex_t v1 = new mesh_vertex_t();
			v1.Read(source);
			for (int i = 2; i < numVertices; ++i)
			{
				mesh_vertex_t v2 = new mesh_vertex_t();
				v2.Read(source);

				BuildTriangle(v0, v1, v2, mesh, mdl);

				//v0 = v1;
				//v1 = v2;
				if (0 == (i & 1))
					v0 = v2;
				else
					v1 = v2;
			}
		}

		private void ReadTrianglesFan(BinaryReader source, mstudio_mesh_t mesh, mstudio_model_t mdl, int numVertices)
		{
			mesh_vertex_t v0 = new mesh_vertex_t();
			v0.Read(source);
			mesh_vertex_t v1 = new mesh_vertex_t();
			v1.Read(source);
			for (int i = 2; i < numVertices; ++i)
			{
				mesh_vertex_t v2 = new mesh_vertex_t();
				v2.Read(source);

				BuildTriangle(v0, v1, v2, mesh, mdl);
				
				v1 = v2;
			}
		}

		private void BuildTriangle(mesh_vertex_t v0, mesh_vertex_t v1, mesh_vertex_t v2, mstudio_mesh_t mesh, mstudio_model_t mdl)
		{
			var textureId = mesh.skinref;
			if ((v0.v == v1.v) || (v0.v == v2.v) || (v1.v == v2.v))
				return;
			mesh.Faces.Add(new ModelFace() {
				Texture = modelTextures[textureId],
				Vertex0 = BuildVertex(v0, mdl, textureId),
				Vertex1 = BuildVertex(v1, mdl, textureId),
				Vertex2 = BuildVertex(v2, mdl, textureId)
			});
		}

		private ModelVertex BuildVertex(mesh_vertex_t v0, mstudio_model_t mdl, int textureId)
		{
			var tex = this.textures[textureId];
			var boneID = mdl.Weights[v0.v];
			var bone = modelBones[boneID];
			var pos = mdl.Vertices[v0.v];
			pos = bone.Transform(pos);
			return new ModelVertex() { Position = pos, Normal = mdl.Normals[v0.n], UV0 = new Vector2((float)v0.s / (float)tex.width, 
				(float)v0.t / (float)tex.height),
									   Bones = new ModelBoneWeight[] { new ModelBoneWeight() { Weight = 1.0f, Bone = bone } }
			};
		}

		private void ReadTextures(BinaryReader source)
		{
			if (header.textureindex == 0) 
				return;

			textures = ReaderHelper.ReadStructs<mstudio_texture_t>(source, (uint)header.numtextures * 80, header.textureindex + startOfTheFile, 80);
			foreach (var t in textures)
			{
				string name = t.name;
				if (name.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase))
					name = name.Substring(0, name.Length - 4);
				if (t.index != 0)
				{
					source.BaseStream.Seek(t.index + startOfTheFile, SeekOrigin.Begin);

					var bytes = source.ReadBytes(t.width * t.height);
					var pal = source.ReadBytes(768);
					var bmp = new Bitmap(t.width, t.height);
					textureImages.Add(bmp);
					for (int y = 0; y < t.height; ++y)
						for (int x = 0; x < t.width; ++x)
						{
							var col = 3 * bytes[t.width * y + x];
							bmp.SetPixel(x, y, Color.FromArgb(pal[col], pal[col + 1], pal[col + 2]));
						}
					modelTextures.Add(new ModelEmbeddedTexture(name, bmp));
				}
				else
				{
					modelTextures.Add(new ModelTextureReference(name));
				}
			}
		}

		#endregion

		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
	}
}