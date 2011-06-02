using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats.Model
{
	public class ModelWriter
	{
		Dictionary<CIwVec3, int> positions = new Dictionary<CIwVec3,int>();
		Dictionary<CIwVec3, int> normals = new Dictionary<CIwVec3,int>();
		Dictionary<CIwVec2, int> uv0s = new Dictionary<CIwVec2,int>();
		Dictionary<CIwVec2, int> uv1s = new Dictionary<CIwVec2,int>();
		Dictionary<CIwColour, int> colors = new Dictionary<CIwColour,int>();
		Dictionary<CIwAnimBone, int> bones = new Dictionary<CIwAnimBone, int>();
		Dictionary<CIwAnimSkinSetKey, int> animSets = new Dictionary<CIwAnimSkinSetKey, int>();
		Dictionary<string, int> surfaces = new Dictionary<string, int>();
		public CMesh TargetMesh { get; set; }
		CIwModel modelMesh;

		//public ModelWriter(CMesh targetMesh)
		//{
		//    this.TargetMesh = targetMesh;
		//}

		public ModelWriter(CIwModel modelMesh)
		{
			this.modelMesh = modelMesh;
			if (modelMesh.ModelBlocks.Count > 0)
				TargetMesh = modelMesh.ModelBlocks[0];
		}

		public int GetSurfaceIndex(string material)
		{
			int i;
			if (surfaces.TryGetValue(material, out i))
				return i;
			i = TargetMesh.Surfaces.Count;
			TargetMesh.Surfaces.Add(new CSurface(){Material = material});
			surfaces[material] = i;
			return i;
		}
		public int GetPositionIndex(CIwVec3 v)
		{
			return GetPositionIndex(v, new CIwAnimSkinSetKey(), null);
		}
		public int GetPositionIndex(CIwVec3 v, CIwAnimSkinSetKey bonesKey, IList<float> w)
		{
			int i;
			if (positions.TryGetValue(v, out i))
				return i;
			i = TargetMesh.Verts.Positions.Count;
			TargetMesh.Verts.Positions.Add(v);
			positions[v] = i;

			if (modelMesh.Skin != null)
			{
				int animSetId = 0;
				if (!animSets.TryGetValue(bonesKey, out animSetId))
				{
					animSetId = modelMesh.Skin.SkinSets.Count;
					animSets[bonesKey] = animSetId;
					modelMesh.Skin.SkinSets.Add(new CIwAnimSkinSet() { useBones = bonesKey });
				}
				modelMesh.Skin.SkinSets[animSetId].vertWeights.Add(GetSortedWeights(i, modelMesh.Skin.SkinSets[animSetId].useBones, bonesKey, w));
			}

			return i;
		}

		public int GetNormalIndex(CIwVec3 v)
		{
			int i;
			if (normals.TryGetValue(v, out i))
				return i;
			i = TargetMesh.VertNorms.Normals.Count;
			TargetMesh.VertNorms.Normals.Add(v);
			normals[v] = i;
			return i;
		}
		public int GetColorIndex(CIwColour v)
		{
			int i;
			if (colors.TryGetValue(v, out i))
				return i;
			i = TargetMesh.VertCols.Colours.Count;
			TargetMesh.VertCols.Colours.Add(v);
			colors[v] = i;
			return i;
		}
		public int GetUV0Index(CIwVec2 v)
		{
			int i;
			if (uv0s.TryGetValue(v, out i))
				return i;
			while (TargetMesh.UVs.Count <= 0)
				TargetMesh.UVs.Add(new CUVs() { SetID = TargetMesh.UVs.Count });
			i = TargetMesh.UVs[0].UVs.Count;
			TargetMesh.UVs[0].UVs.Add(v);
			uv0s[v] = i;
			return i;
		}
		public int GetUV1Index(CIwVec2 v)
		{
			int i;
			if (uv1s.TryGetValue(v, out i))
				return i;
			while (TargetMesh.UVs.Count <= 1)
				TargetMesh.UVs.Add(new CUVs() { SetID = TargetMesh.UVs.Count });
			i = TargetMesh.UVs[1].UVs.Count;
			TargetMesh.UVs[1].UVs.Add(v);
			uv1s[v] = i;
			return i;
		}

		public CTrisVertex GetVertex(CIwVec3 p, CIwAnimSkinSetKey bonesKey, IList<float> w, CIwVec3 n, CIwVec2 uv0, CIwVec2 uv1, CIwColour col)
		{
			int vertexIndex = GetPositionIndex(p,bonesKey,w);

			
			
			return new CTrisVertex(vertexIndex, GetNormalIndex(n), GetUV0Index(uv0), GetUV1Index(uv1), GetColorIndex(col));
		}

		private CIwAnimSkinSetVertWeights GetSortedWeights(int v, CIwAnimSkinSetKey cIwAnimSkinSetKey, CIwAnimSkinSetKey bonesKey, IList<float> w)
		{
			var res = new CIwAnimSkinSetVertWeights() {Vertex=v,Weights = new float[bonesKey.Bones.Count]};
			for (int i = 0; i < bonesKey.Bones.Count; ++i)
				res.Weights[cIwAnimSkinSetKey.GetBoneIndex(bonesKey.Bones[i])] = w[i];
			return res;
		}

		public void AddTriangle(int surface, CTrisVertex v0, CTrisVertex v1, CTrisVertex v2)
		{
			var t = new CTrisElement() { Vertex0 = v0, Vertex1 = v1, Vertex2 = v2 };
			TargetMesh.Surfaces[surface].Triangles.Elements.Add(t);
		}

	
		public void WriteBone(CIwAnimBone cIwAnimBone)
		{
			if (modelMesh.Skin == null)
			{
				modelMesh.Skin = new CIwAnimSkin() { Name = modelMesh.Name };
				modelMesh.Skin.skeleton = new CIwAnimSkel() { Name = modelMesh.Name };
				modelMesh.Skin.model = modelMesh;
			}
			bones[cIwAnimBone] = modelMesh.Skin.skeleton.Bones.Count;
			modelMesh.Skin.skeleton.Bones.Add(cIwAnimBone);
		}
	}
}
