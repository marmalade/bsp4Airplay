using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat;
using AirplaySDKFileFormats;
using AirplaySDKFileFormats.Model;
using System.IO;
using Atlasing;
using System.Drawing;
using ReaderUtils;

namespace Bsp2AirplayAdapter
{
	public class Adapter
	{
		Dictionary<BspTreeNode, int> nodeIndices = new Dictionary<BspTreeNode, int>();
		Dictionary<BspTreeLeaf, int> leafIndices = new Dictionary<BspTreeLeaf, int>();
		Dictionary<BspGeometry, int> clusterIndices = new Dictionary<BspGeometry, int>();
		List<BspTreeLeaf> allLeaves = new List<BspTreeLeaf>();
		List<BspGeometry> allClusters = new List<BspGeometry>();
		List<List<int>> subclustersInCluster = new List<List<int>>();
		Dictionary<BspTexture, int> textureIndices = new Dictionary<BspTexture, int>();
		//Dictionary<BspTexture, int> lightmapIndices = new Dictionary<BspTexture, int>();
		CIwResGroup group;
		Cb4aLevel level;
		//CIwModel model;
		LevelVBWriter writer;
		BspTexture commonLightmap;
		public void Convert(BspDocument bsp, CIwResGroup group)
		{
			this.group = group;
			this.level = new Cb4aLevel();
			level.Name = bsp.Name;
			group.AddRes(new CIwTexture() { FilePath = "../textures/checkers.png" });
			group.AddRes(level);
			writer = new LevelVBWriter(level);

			CollectAllLeaves(bsp.Tree);
			CollectAllClusters();
			BuildLightmapAtlas(bsp);
			BuildClusters();


			//level.Materials.Add(new Cb4aLevelMaterial() { Texture="checkers" });

			foreach (var e in bsp.Entities)
			{
				level.Entities.Add(new Cb4aEntity() { classname = e.ClassName, origin = GetVec3(e.Origin), values = e.Values });
			}
			AddLeaves(level);
			if (bsp.Tree != null)
				AddTreeNode(level, bsp.Tree);
		}

		private void BuildClusters()
		{
			int i = 0;
			foreach (var c in allClusters)
			{
				int j = WriteVB(c, writer);
				if (j != i)
					throw new ApplicationException("j!=i");
				++i;
			}
		}

		private void CollectAllClusters()
		{
			foreach (BspTreeLeaf l in allLeaves)
			{
				foreach (var c in l.Geometries)
				{
					int i;
					if (!clusterIndices.TryGetValue(c, out i))
					{
						if (c.Faces.Count > 0)
						{
							i = allClusters.Count;
							allClusters.Add(c);
						}
						else
						{
							i = -1;
						}
						clusterIndices.Add(c, i);
					}
				}
			}
		}

		private void AddLeaves(Cb4aLevel level)
		{
			for (int i = 0; i < allLeaves.Count; ++i)
			{
				var bspTreeLeaf = allLeaves[i];
				i = level.Leaves.Count;
				var ll = new Cb4aLeaf() { Index = i, mins = GetVec3(bspTreeLeaf.Mins), maxs = GetVec3(bspTreeLeaf.Maxs) };
				level.Leaves.Add(ll);
				foreach (var g in bspTreeLeaf.Geometries)
				{
					int clusterIndix = clusterIndices[g];
					if (clusterIndix >= 0)
					{
						foreach (var sub in subclustersInCluster[clusterIndix])
							ll.Clusters.Add(sub);
					}
				}
				ll.SortClusters(level);
				if (bspTreeLeaf.Colliders != null)
				{
					foreach (var collider in bspTreeLeaf.Colliders)
					{
						var c = BuildCollider(collider);
						if (c != null)
							ll.Colliders.Add(c);
					}
				}
				foreach (var v in bspTreeLeaf.VisibleLeaves)
					ll.VisibleLeaves.Add(leafIndices[v]);
			}
		}

		private Ib4aCollider BuildCollider(BspCollisionObject collider)
		{
			if (collider is BspCollisionFaceSoup)
				return BuildCollisionFaceSoup((BspCollisionFaceSoup)collider);
			if (collider is BspCollisionConvexBrush)
				return BuildCollisionConvexBrush((BspCollisionConvexBrush)collider);
			return null;
		}

		private Ib4aCollider BuildCollisionConvexBrush(BspCollisionConvexBrush bspCollisionConvexBrush)
		{
			var brush = new Cb4aCollisionConvexBrush();
			foreach (var f in bspCollisionConvexBrush.Planes)
			{
				CIwPlane plane = new CIwPlane() { v = GetVec3Fixed(f.Normal), k = GetFixed(f.Distance) };
				//brush.Planes.Add(writer.WritePlane(plane));
				brush.Planes.Add(plane);
			}
			return brush;
		}


		private Ib4aCollider BuildCollisionFaceSoup(BspCollisionFaceSoup bspCollisionFaceSoup)
		{
			var soup = new Cb4aCollisionMeshSoup();
			foreach (var f in bspCollisionFaceSoup.Faces)
			{
				soup.Faces.Add(BuildCollisionFaceSoupFace(f, soup));
			}
			return soup;
		}
		private Cb4aCollisionMeshSoupFace BuildCollisionFaceSoupFace(BspCollisionFaceSoupFace f, Cb4aCollisionMeshSoup soup)
		{
			var r = new Cb4aCollisionMeshSoupFace();
			
			Vector3 n = Vector3.UnitZ;
			float maxLength = float.MinValue;
			for (int i=0; i<f.Vertices.Count-1;++i)
			{
				Vector3 edge0 = f.Vertices[i] - f.Vertices[(i + 1) % f.Vertices.Count];
				Vector3 nn = Vector3.Cross(f.Vertices[(i + 1) % f.Vertices.Count] - f.Vertices[(i + 2) % f.Vertices.Count], edge0);
				float l = nn.LengthSquared;
				if (l> maxLength)
				{
					maxLength = l;
					n = nn;
				}
				
			}
			if (maxLength < 1)
			{
				maxLength = maxLength;
				//throw new ApplicationException("face is almost zero area!");
			}
			n.Normalize();

			
			r.startPlane = soup.Planes.Count;
			soup.Planes.Add(new CIwPlane() { k = (int)(Vector3.Dot(f.Vertices[0], n)) * AirplaySDKMath.IW_GEOM_ONE, v = GetVec3Fixed(n) });

			int lastD = int.MinValue;
			CIwVec3 lastN = CIwVec3.g_Zero;
			for (int i = 0; i < f.Vertices.Count; ++i)
			{
				var d = Vector3.Cross(n,f.Vertices[i] - f.Vertices[(i + 1) % f.Vertices.Count]);
				d.Normalize();
				CIwVec3 newN = GetVec3Fixed(d);
				CIwVec3 v3 = GetVec3(f.Vertices[i]);
				int newD = newN.x * v3.x + newN.y * v3.y + newN.z * v3.z;//(int)(Vector3.Dot(d, f.Vertices[i]) * AirplaySDKMath.IW_GEOM_ONE);
				if (newN != lastN || newD != lastD)
				{
					lastD=newD;
					lastN = newN;

					//test
					//for (int j = 0; j < f.Vertices.Count; ++j)
					//{
					//    if ((newN.x * f.Vertices[j].X + newN.y * f.Vertices[j].Y + newN.z * f.Vertices[j].Z) - lastD < -64)
					//    {
					//        //TODO: Split such faces into individual triangles. Think how to handle it in graphics
					//        //throw new ApplicationException("wrong edge normal " + newN);
					//    }
					//}

					soup.Planes.Add(new CIwPlane() { k = newD, v = newN });
					//r.edges.Add(new Cb4aCollisionMeshSoupFaceEdge(){Normal=newN, Distance=newD});
				}
			}
			r.numPlanes = soup.Planes.Count - r.startPlane-1;
			return r;
		}

		//private Cb4aCollisionMeshSoupFaceEdge BuildBspCollisionFaceSoupFaceEdge(BspCollisionFaceSoupFaceEdge e)
		//{
		//    return new Cb4aCollisionMeshSoupFaceEdge() { Normal = GetVec3Fixed(e.Normal), Distance = (int)e.Distance * AirplaySDKMath.IW_GEOM_ONE };
		//}
		private void BuildLightmapAtlas(BspDocument bsp)
		{
			Dictionary<Bitmap, bool> lightmaps = new Dictionary<Bitmap, bool>();
			CollectAllLightmaps(lightmaps);
			//TODO: Make multiple atlases 128x128 istead one huge atlas. The reason is that there is only 4096 steps in UV coordinate!
			Atlas atlas = new Atlas();
			foreach (var l in lightmaps.Keys)
			{
				atlas.Add(l);
			}

			commonLightmap = new BspEmbeddedTexture() { Name = bsp.Name + "_lightmap", mipMaps = new Bitmap[] { atlas.Bitmap } };
			UpdateLightmap(commonLightmap, atlas);
			
		//    
		//    Bitmap Lightmap = new Bitmap(128,128);
		//    using (var g = Graphics.FromImage(Lightmap))
		//    {
		//        g.Clear(Color.Red);
		//    }
		//    BspEmbeddedTexture lm = new BspEmbeddedTexture() { Name = "lightmap", mipMaps = new Bitmap[1] { Lightmap } };
		//    foreach (var l in leaves)
		//    {
		//        if (l.Geometry != null)
		//            foreach (var f in l.Geometry.Faces)
		//                if (f.Lightmap != null)
		//                    f.Lightmap = lm;
		//    }
		}

		private void UpdateLightmap(BspTexture result, Atlas atlas)
		{
			foreach (var geo in allClusters)
			{
				Size dstSize = new Size(atlas.Bitmap.Width, atlas.Bitmap.Height);
				for (int i = 0; i < geo.Faces.Count; ++i)
				{
					var f = geo.Faces[i];
					if (f.Lightmap != null)
					{
						if (f.Lightmap.Equals(result))
							continue;
						var item = atlas.GetItem(((BspEmbeddedTexture)f.Lightmap).mipMaps[0]);
						var ff = new BspGeometryFace() { Texture = f.Texture, Lightmap = result };
						ff.Vertex0 = CorrectLightmapCoords(f.Vertex0, dstSize, item);
						ff.Vertex1 = CorrectLightmapCoords(f.Vertex1, dstSize, item);
						ff.Vertex2 = CorrectLightmapCoords(f.Vertex2, dstSize, item);
						geo.Faces[i] = ff;
					}
				}
			}
		}

		private BspGeometryVertex CorrectLightmapCoords(BspGeometryVertex src, Size dstSize, AtlasItem item)
		{
			BspGeometryVertex res = new BspGeometryVertex();
			res.Color = src.Color;
			res.Normal = src.Normal;
			res.Position = src.Position;
			res.UV0 = src.UV0;
			var size =item.Size;
			var position =item.Position;
			res.UV1.X = ((float)item.Position.X + src.UV1.X * (float)size.Width) / (float)dstSize.Width;
			res.UV1.Y = ((float)item.Position.Y + src.UV1.Y * (float)size.Height) / (float)dstSize.Height;
			return res;
		}

		private void CollectAllLeaves(BspTreeElement bspTreeElement)
		{
			if (bspTreeElement == null)
				return;
			if (bspTreeElement is BspTreeNode)
			{
				CollectAllLeaves(((BspTreeNode)bspTreeElement).Front);
				CollectAllLeaves(((BspTreeNode)bspTreeElement).Back);
				return;
			}
			var leaf = (BspTreeLeaf)bspTreeElement;
			int i;
			if (leafIndices.TryGetValue(leaf, out i))
				return;
			i = allLeaves.Count;
			allLeaves.Add(leaf);
			leafIndices[leaf] = i;
			foreach (var v in leaf.VisibleLeaves)
				CollectAllLeaves(v);
		}
		private void CollectAllLightmaps(Dictionary<Bitmap, bool> lightmaps)
		{
			foreach (var geo in allClusters)
			{
				foreach (var f in geo.Faces)
					if (f.Lightmap != null)
						lightmaps[((BspEmbeddedTexture)f.Lightmap).mipMaps[0]] = true;
			}
		}
		private int AddTreeNode(Cb4aLevel l, BspTreeElement bspTreeElement)
		{
			if (bspTreeElement is BspTreeNode)
				return AddTreeNode(l, (BspTreeNode)bspTreeElement);
			return AddTreeLeaf(l, (BspTreeLeaf)bspTreeElement);
		}

		private int AddTreeLeaf(Cb4aLevel l, BspTreeLeaf bspTreeLeaf)
		{
			return leafIndices[bspTreeLeaf];
			
		}
		void RegisterTexture(BspTexture t)
		{
			if (t == null)
				return;
			if (textureIndices.ContainsKey(t))
				return;
			textureIndices[t] = 0;
			string subfolder = "../textures/";
			//t.Name = t.Name;
			foreach (var c in Path.GetInvalidFileNameChars())
				t.Name = t.Name.Replace(c, '_');
			Bitmap bmp = null;
			if (t is BspEmbeddedTexture)
			{
				var embeddedTex = (BspEmbeddedTexture)t;
				if (embeddedTex.mipMaps != null && embeddedTex.mipMaps.Length > 0)
					bmp = embeddedTex.mipMaps[0];
			}
			group.AddRes(new CIwTexture() { FilePath = subfolder + t.Name + ".png", Bitmap = bmp });
		}

		private int WriteVB(BspGeometry geometry, LevelVBWriter writer)
		{
			if (geometry == null)
				return -1;
			if (geometry.Faces.Count == 0)
				return -1;

			TessalateFaces(geometry);

			var clusterIndex = subclustersInCluster.Count;
			var cluster = new List<int>();
			subclustersInCluster.Add(cluster);

			writer.PrepareVertexBuffer(geometry.Faces.Count * 3);
			//cluster.VertexBuffer = level.VertexBuffers.Count - 1;
			Dictionary<int, bool> materialMap = new Dictionary<int, bool>();
			List<int> materialInices = new List<int>(geometry.Faces.Count);
			foreach (var f in geometry.Faces)
			{
				RegisterTexture(f.Texture);
				if (f.Lightmap != null && commonLightmap != f.Lightmap)
					throw new ApplicationException("not atlased lightmap");
				RegisterTexture(f.Lightmap);
				int matIndex = writer.WriteMaterial(BuildMaterial(f));
				materialInices.Add(matIndex);
				materialMap[matIndex] = true;
			}
			foreach (int t in materialMap.Keys)
			{
				var sub = new Cb4aLevelVBSubcluster();
				sub.Material = t;
				sub.VertexBuffer = level.VertexBuffers.Count - 1;
				CIwVec3 mins = new CIwVec3(int.MaxValue, int.MaxValue, int.MaxValue);
				CIwVec3 maxs = new CIwVec3(int.MinValue, int.MinValue, int.MinValue);
				for (int i = 0; i < materialInices.Count; ++i)
				{
					if (materialInices[i] == t)
					{
						var f = geometry.Faces[i];

						//TODO: slice face into more faces
						BuildFace(writer, sub, ref mins, ref maxs, f);
					}
				}
				if (mins.x > maxs.x)
				{
					mins = CIwVec3.g_Zero;
					maxs = CIwVec3.g_Zero;
				}
				sub.Mins = mins;
				sub.Maxs = maxs;
				if (sub.Indices.Count > 0)
				{
					cluster.Add(level.subclusters.Count);
					level.subclusters.Add(sub);
				}
			}

			return clusterIndex;
		}

		private static Cb4aLevelMaterial BuildMaterial(BspGeometryFace f)
		{
			BspTexture fTexture = f.Texture;
			var res = new Cb4aLevelMaterial();
			if (fTexture != null) {
				res.Texture = fTexture.Name;
				res.Sky = fTexture.Sky;
				res.Transparent = fTexture.Transparent;
			}
			if (f.Lightmap != null) res.Lightmap = f.Lightmap.Name;
			return res;
		}

		private void TessalateFaces(BspGeometry bspGeometry)
		{
			for (int i = 0; i < bspGeometry.Faces.Count; )
			{
				var f = bspGeometry.Faces[i];
				if (f.MaxUV0Distance > 15.5f)
				{
					bspGeometry.Faces.RemoveAt(i);
					TessalateFace(bspGeometry.Faces, f);
					continue;
				}
				++i;
			}
		}

		private void BuildFace(LevelVBWriter writer, Cb4aLevelVBSubcluster sub, ref CIwVec3 mins, ref CIwVec3 maxs, BspGeometryFace f)
		{
			
			while (f.Vertex0.UV0.X >= 8 || f.Vertex1.UV0.X >= 8 || f.Vertex2.UV0.X >= 8)
			{
				f.Vertex0.UV0.X -= 1;
				f.Vertex1.UV0.X -= 1;
				f.Vertex2.UV0.X -= 1;
			}
			while (f.Vertex0.UV0.Y >= 8 || f.Vertex1.UV0.Y >= 8 || f.Vertex2.UV0.Y >= 8)
			{
				f.Vertex0.UV0.Y -= 1;
				f.Vertex1.UV0.Y -= 1;
				f.Vertex2.UV0.Y -= 1;
			}
			if (f.Vertex0.UV0.X < -8) f.Vertex0.UV0.X = -8;
			if (f.Vertex1.UV0.X < -8) f.Vertex1.UV0.X = -8;
			if (f.Vertex2.UV0.X < -8) f.Vertex2.UV0.X = -8;
			if (f.Vertex0.UV0.Y < -8) f.Vertex0.UV0.Y = -8;
			if (f.Vertex1.UV0.Y < -8) f.Vertex1.UV0.Y = -8;
			if (f.Vertex2.UV0.Y < -8) f.Vertex2.UV0.Y = -8;

			if (f.Vertex0.Position.X < mins.x) mins.x = (int)f.Vertex0.Position.X;
			if (f.Vertex1.Position.X < mins.x) mins.x = (int)f.Vertex1.Position.X;
			if (f.Vertex2.Position.X < mins.x) mins.x = (int)f.Vertex2.Position.X;
			if (f.Vertex0.Position.Y < mins.y) mins.y = (int)f.Vertex0.Position.Y;
			if (f.Vertex1.Position.Y < mins.y) mins.y = (int)f.Vertex1.Position.Y;
			if (f.Vertex2.Position.Y < mins.y) mins.y = (int)f.Vertex2.Position.Y;
			if (f.Vertex0.Position.Z < mins.z) mins.z = (int)f.Vertex0.Position.Z;
			if (f.Vertex1.Position.Z < mins.z) mins.z = (int)f.Vertex1.Position.Z;
			if (f.Vertex2.Position.Z < mins.z) mins.z = (int)f.Vertex2.Position.Z;

			if (f.Vertex0.Position.X > maxs.x) maxs.x = (int)f.Vertex0.Position.X;
			if (f.Vertex1.Position.X > maxs.x) maxs.x = (int)f.Vertex1.Position.X;
			if (f.Vertex2.Position.X > maxs.x) maxs.x = (int)f.Vertex2.Position.X;
			if (f.Vertex0.Position.Y > maxs.y) maxs.y = (int)f.Vertex0.Position.Y;
			if (f.Vertex1.Position.Y > maxs.y) maxs.y = (int)f.Vertex1.Position.Y;
			if (f.Vertex2.Position.Y > maxs.y) maxs.y = (int)f.Vertex2.Position.Y;
			if (f.Vertex0.Position.Z > maxs.z) maxs.z = (int)f.Vertex0.Position.Z;
			if (f.Vertex1.Position.Z > maxs.z) maxs.z = (int)f.Vertex1.Position.Z;
			if (f.Vertex2.Position.Z > maxs.z) maxs.z = (int)f.Vertex2.Position.Z;

			var index0 = writer.Write(GetLevelVBItem(f.Vertex0));
			var index1 = writer.Write(GetLevelVBItem(f.Vertex1));
			var index2 = writer.Write(GetLevelVBItem(f.Vertex2));
			if ((index0 == index1) || (index0 == index2) || (index2 == index1))
				return;
			sub.Indices.Add(index2);
			sub.Indices.Add(index1);
			sub.Indices.Add(index0);
		}

		private void TessalateFace(IList<BspGeometryFace> faces, BspGeometryFace f)
		{
			var v01 = new BspGeometryVertex() { 
				Normal = f.Vertex0.Normal, 
				Position = (f.Vertex0.Position + f.Vertex1.Position)*0.5f,
				UV0 = (f.Vertex0.UV0 + f.Vertex1.UV0) * 0.5f,
				UV1 = (f.Vertex0.UV1 + f.Vertex1.UV1) * 0.5f,
				Color = Color.FromArgb(
					(byte)(((int)f.Vertex0.Color.A + (int)f.Vertex1.Color.A) / 2),
					(byte)(((int)f.Vertex0.Color.R + (int)f.Vertex1.Color.R) / 2),
					(byte)(((int)f.Vertex0.Color.G + (int)f.Vertex1.Color.G) / 2),
					(byte)(((int)f.Vertex0.Color.B + (int)f.Vertex1.Color.B) / 2))
			};
			var v02 = new BspGeometryVertex() { 
				Normal = f.Vertex0.Normal, 
				Position = (f.Vertex0.Position + f.Vertex2.Position)*0.5f,
				UV0 = (f.Vertex0.UV0 + f.Vertex2.UV0) * 0.5f,
				UV1 = (f.Vertex0.UV1 + f.Vertex2.UV1) * 0.5f,
				Color = Color.FromArgb(
					(byte)(((int)f.Vertex0.Color.A + (int)f.Vertex2.Color.A) / 2),
					(byte)(((int)f.Vertex0.Color.R + (int)f.Vertex2.Color.R) / 2),
					(byte)(((int)f.Vertex0.Color.G + (int)f.Vertex2.Color.G) / 2),
					(byte)(((int)f.Vertex0.Color.B + (int)f.Vertex2.Color.B) / 2))
			};
			var v12 = new BspGeometryVertex() { 
				Normal = f.Vertex1.Normal, 
				Position = (f.Vertex1.Position + f.Vertex2.Position)*0.5f,
				UV0 = (f.Vertex1.UV0 + f.Vertex2.UV0) * 0.5f,
				UV1 = (f.Vertex1.UV1 + f.Vertex2.UV1) * 0.5f,
				Color = Color.FromArgb(
					(byte)(((int)f.Vertex1.Color.A + (int)f.Vertex2.Color.A) / 2),
					(byte)(((int)f.Vertex1.Color.R + (int)f.Vertex2.Color.R) / 2),
					(byte)(((int)f.Vertex1.Color.G + (int)f.Vertex2.Color.G) / 2),
					(byte)(((int)f.Vertex1.Color.B + (int)f.Vertex2.Color.B) / 2))
			};
			faces.Add(new BspGeometryFace() 
			{ 
				Texture = f.Texture, Lightmap = f.Lightmap,
				Vertex0 = f.Vertex0,
				Vertex1 = v01,
				Vertex2 = v02
			});
			faces.Add(new BspGeometryFace()
			{
				Texture = f.Texture,
				Lightmap = f.Lightmap,
				Vertex0 = f.Vertex1,
				Vertex1 = v12,
				Vertex2 = v01
			});
			faces.Add(new BspGeometryFace()
			{
				Texture = f.Texture,
				Lightmap = f.Lightmap,
				Vertex0 = f.Vertex2,
				Vertex1 = v02,
				Vertex2 = v12
			});
			faces.Add(new BspGeometryFace()
			{
				Texture = f.Texture,
				Lightmap = f.Lightmap,
				Vertex0 = v01,
				Vertex1 = v12,
				Vertex2 = v02
			});
		}

		private LevelVBItem GetLevelVBItem(BspGeometryVertex bspGeometryVertex)
		{
			return new LevelVBItem() { Position = GetVec3(bspGeometryVertex.Position),
									   Normal = GetVec3Fixed(bspGeometryVertex.Normal),
									   UV0 = GetVec2Fixed(bspGeometryVertex.UV0),
									   UV1 = GetVec2Fixed(bspGeometryVertex.UV1),
									   Colour = GetColour(bspGeometryVertex.Color)
			};
		}

		//private void WriteGeometry(BspGeometry bspGeometry, ModelWriter modelWriter)
		//{
		//    foreach (var face in bspGeometry.Faces)
		//    {
				
		//        var surface = modelWriter.GetSurfaceIndex(face.Texture.Name);
		//        CIwVec2 v0uv0 = GetVec2Fixed(face.Vertex0.UV0);
		//        CIwVec2 v1uv0 = GetVec2Fixed(face.Vertex1.UV0);
		//        CIwVec2 v2uv0 = GetVec2Fixed(face.Vertex2.UV0);
		//        while (v0uv0.x > 32767 || v1uv0.x > 32767 || v2uv0.x > 32767)
		//        {
		//            v0uv0.x = v0uv0.x-AirplaySDKMath.IW_GEOM_ONE;
		//            v1uv0.x = v1uv0.x-AirplaySDKMath.IW_GEOM_ONE;
		//            v2uv0.x = v2uv0.x-AirplaySDKMath.IW_GEOM_ONE;
		//        }
		//        while (v0uv0.y > 32767 || v1uv0.y > 32767 || v2uv0.y > 32767)
		//        {
		//            v0uv0.y -= AirplaySDKMath.IW_GEOM_ONE;
		//            v1uv0.y -= AirplaySDKMath.IW_GEOM_ONE;
		//            v2uv0.y -= AirplaySDKMath.IW_GEOM_ONE;
		//        }
		//        BspGeometryVertex vertex = face.Vertex0;
		//        var v0 = GetVertex(modelWriter, v0uv0, vertex);
		//        vertex = face.Vertex1;
		//        var v1 = GetVertex(modelWriter, v1uv0, vertex);
		//        vertex = face.Vertex2;
		//        var v2 = GetVertex(modelWriter, v2uv0, vertex);
		//        modelWriter.AddTriangle(surface, v0, v1, v2);
		//    }
		//}

		//private CTrisVertex GetVertex(ModelWriter modelWriter, CIwVec2 v0uv0, BspGeometryVertex vertex)
		//{
		//    return modelWriter.GetVertex(GetVec3(vertex.Position), GetVec3Fixed(vertex.Normal), v0uv0, GetVec2Fixed(vertex.UV1), CIwColour.White);
		//}

		

		private int AddTreeNode(Cb4aLevel l, BspTreeNode bspTreeNode)
		{
			int i;
			if (!nodeIndices.TryGetValue(bspTreeNode, out i))
			{
				i = l.Nodes.Count;
				var node = new Cb4aNode() { Index = i, mins = GetVec3(bspTreeNode.Mins), maxs = GetVec3(bspTreeNode.Maxs) };
				nodeIndices[bspTreeNode] = i;
				l.Nodes.Add(node);
				node.Plane = writer.WritePlane(new CIwPlane() { v = GetVec3Fixed(bspTreeNode.PlaneNormal), k = (int)(bspTreeNode.PlaneDistance) });
				//node.PlaneDistance = bspTreeNode.PlaneDistance;
				//node.PlaneNormal = GetVec3Fixed(bspTreeNode.PlaneNormal);
				node.IsFrontLeaf = bspTreeNode.Front is BspTreeLeaf;
				node.Front = AddTreeNode(l, bspTreeNode.Front);
				node.IsBackLeaf = bspTreeNode.Back is BspTreeLeaf;
				node.Back = AddTreeNode(l, bspTreeNode.Back);
			}
			return i;
		}

		private int GetFixed(float x)
		{
			return (int)(x * AirplaySDKMath.IW_GEOM_ONE);
		}

		private CIwVec3 GetVec3Fixed(ReaderUtils.Vector3 vector3)
		{
			return new CIwVec3(
				(int)(vector3.X*AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Y*AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Z*AirplaySDKMath.IW_GEOM_ONE)
				);
		}
		private CIwColour GetColour(System.Drawing.Color col)
		{
			return new CIwColour() { r=col.R, g=col.G,b=col.B,a=col.A };
		}
		private CIwVec2 GetVec2Fixed(Vector2 vector3)
		{
			return new CIwVec2(
				(int)(vector3.X * AirplaySDKMath.IW_GEOM_ONE),
				(int)(vector3.Y * AirplaySDKMath.IW_GEOM_ONE)
				);
		}

		private CIwVec3 GetVec3(ReaderUtils.Vector3 vector3)
		{
			return new CIwVec3(
				(int)(vector3.X),
				(int)(vector3.Y),
				(int)(vector3.Z)
				);
		}
		
		public void Convert(string bspFilePath, string groupFilePath)
		{
			var doc = BspDocument.Load(bspFilePath);
			var group = new CIwResGroup();
			group.Name = doc.Name;
			Convert(doc, group);
			using (var w = new CTextWriter(groupFilePath))
			{
				group.WrtieToStream(w);
				w.Close();
			}
		}
	}
}
