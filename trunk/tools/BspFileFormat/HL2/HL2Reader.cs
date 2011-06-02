using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.IO;
using ReaderUtils;
using System.Collections;
using System.Drawing;

namespace BspFileFormat.HL2
{
	public abstract class HL2Reader : IBspReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected string entities;
		protected List<Vector3> vertices;
		protected List<edge_t> edges;
		protected List<face_t> faces;
		protected List<dleaf_t> dleaves;
		protected List<plane_t> planes;
		protected List<BspTreeLeaf> leaves = new List<BspTreeLeaf>();
		protected List<dnode_t> nodes;
		protected List<texinfo_t> surfaces;
		protected List<dtexdata_t> texData;
		protected List<BspTexture> textures = new List<BspTexture> ();
		protected ushort[] listOfFaces;
		protected int[] listOfEdges;
		protected List<cluster_t> clusters;
		protected Vector3[] lightmap;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadEntities(source);
			ReadVertices(source);
			ReadListOfEdges(source);
			ReadListOfFaces(source);
			ReadTextureInfos(source);
			ReadTextureDatas(source);
			ReadEdges(source);
			ReadPlanes(source);
			ReadFaces(source);
			ReadNodes(source);
			ReadVisibilityList(source);
			ReadLeaves(source);
			ReadLightmap(source);
			CollectLeavesInClusters();

			ReaderHelper.BuildEntities(entities, dest);
			BuildTextures();
			BuildLeaves();
			if (nodes != null && nodes.Count > 0)
			{
				dest.Tree = BuildNode(nodes[0]);
			}
			BuildVisibilityList();
			BuildClusters();
		}
		private void BuildClusters()
		{
			FaceToLeafMap faceMap = new FaceToLeafMap(faces.Count);
			for (int i = 0; i < dleaves.Count; ++i)
			{
				var dleaf = dleaves[i];
				BuilFaceToLeafMap(faceMap, i, dleaf);
				if (dleaf.cluster >= 0)
				{
					foreach (var vis in clusters[dleaf.cluster].lists)
					{
						dleaf = dleaves[vis];
						BuilFaceToLeafMap(faceMap, i, dleaf);
					}
					foreach (var visC in clusters[dleaf.cluster].visiblity)
						foreach (var vis in clusters[visC].lists)
						{
							dleaf = dleaves[vis];
							BuilFaceToLeafMap(faceMap, i, dleaf);
						}
				}
			}
			var keys = faceMap.FindUniqueKeys();

			foreach (var k in keys)
			{
				if (k.Faces.Count > 0)
				{
					var geo = BuildGeometry(k.Faces);
					if (geo != null)
					{
						foreach (var l in k.Key.Leaves)
							leaves[l.Key].Geometries.Add(geo);
					}
				}
			}
		}
		private void BuilFaceToLeafMap(FaceToLeafMap faceMap, int i, dleaf_t dleaf)
		{
			if (dleaf.numleaffaces >= 0 && dleaf.firstleafface >= 0 && dleaf.firstleafface != ushort.MaxValue)
				for (int j = dleaf.firstleafface; j < dleaf.firstleafface + dleaf.numleaffaces; ++j)
					faceMap.Faces[(int)listOfFaces[j]].AddLeaf(i);
		}
		private void ReadVisibilityList(BinaryReader source)
		{
			SeekDir(source, header.Visibility);
			var pos = source.BaseStream.Position;
			int num_clusters = source.ReadInt32();
			clusters = new List<cluster_t>(num_clusters);
			for (int i = 0; i < num_clusters; ++i)
			{
				cluster_t cluster = new cluster_t();
				cluster.offset = source.ReadInt32();
				cluster.phs = source.ReadInt32();
				clusters.Add(cluster);
			}
			for (int i = 0; i < num_clusters; ++i)
			{
				var cluster = clusters[i];
				source.BaseStream.Seek(pos + cluster.offset, SeekOrigin.Begin);
				for (int c = 0; c < num_clusters; )
				{
					byte pvs_buffer = source.ReadByte();
					if (pvs_buffer == 0)
					{
						c += 8 * source.ReadByte();
					}
					else
					{
						for (byte bit = 1; bit != 0; bit *= 2, c++)
						{
							if (0 != (pvs_buffer & bit))
							{
								cluster.visiblity.Add(c);
							}
						}
					}

				}
			}

		}
		private void BuildVisibilityList()
		{
			for (int i = 0; i < dleaves.Count; ++i)
			{
				Dictionary<int, bool> map = new Dictionary<int, bool>();
				if (dleaves[i].cluster >= 0)
					foreach (var c in clusters[dleaves[i].cluster].visiblity)
						foreach (var l in clusters[c].lists)
							map[l] = true;
				foreach (var j in map.Keys)
					if (i != j)
						leaves[i].VisibleLeaves.Add(leaves[j]);
			}
		}
		private void BuildTextures()
		{
			foreach (var t in texData)
				textures.Add(new BspTextureReference() { Name = t.name, Width = (t.width == 0) ? 256 : t.width, Height = (t.height==0)?256:t.height });
		}

		private void ReadTextureDatas(BinaryReader source)
		{
			texData = ReaderHelper.ReadStructs<dtexdata_t>(source, header.Texdata.size, header.Texdata.offset + startOfTheFile, 8*4);
			var indices = ReaderHelper.ReadInt32Array(source, header.TexdataStringTable.size, (uint)(header.TexdataStringTable.offset + startOfTheFile));
			foreach (var t in texData)
			{
				source.BaseStream.Seek(startOfTheFile + header.TexdataStringData.offset + indices[t.nameStringTableID], SeekOrigin.Begin);
				t.name = ReaderHelper.ReadStringSZ(source);
			}
			
			
		}

		private void ReadTextureInfos(BinaryReader source)
		{
			surfaces = ReaderHelper.ReadStructs<texinfo_t>(source, header.Texinfo.size, header.Texinfo.offset + startOfTheFile, 72);
		}
		private void ReadPlanes(BinaryReader source)
		{
			planes = ReaderHelper.ReadStructs<plane_t>(source, header.Planes.size, header.Planes.offset + startOfTheFile, 20);
		}
		private void ReadListOfEdges(BinaryReader source)
		{
			listOfEdges = ReaderHelper.ReadInt32Array(source, header.Surfedges.size, header.Surfedges.offset);
		}
		private void ReadListOfFaces(BinaryReader source)
		{
			listOfFaces = ReaderHelper.ReadUInt16Array(source, header.LeafFaces.size, header.LeafFaces.offset);
		}
		private void BuildLeaves()
		{
			for (int i = 0; i < dleaves.Count; ++i)
			{
				leaves.Add(BuildLeaf(dleaves[i]));
			}
		}
		private BspTreeLeaf BuildLeaf(dleaf_t dleaf)
		{
			var res = new BspTreeLeaf();
			res.Mins = new Vector3(dleaf.box.mins[0], dleaf.box.mins[1], dleaf.box.mins[2]);
			res.Maxs = new Vector3(dleaf.box.maxs[0], dleaf.box.maxs[1], dleaf.box.maxs[2]);

			//if (dleaf.firstleafface != ushort.MaxValue)
			//	res.Geometries.Add(BuildGeometry(dleaf.firstleafface, dleaf.numleaffaces));
			return res;
		}
		Dictionary<int, BspTexture> faceLightmapObjects = new Dictionary<int, BspTexture>();
		private BspGeometry BuildGeometry(List<int> list)
		{
			if (list == null || list.Count == 0)
				return null;
			var res = new BspGeometry() { Faces = new List<BspGeometryFace>() };
			foreach (var faceIndex in list)
			{
				var face = faces[faceIndex];
				if (face.numedges == 0)
					continue;
				plane_t plane = planes[face.planenum];
				var surf = surfaces[face.texinfo];
				var texture_id = (int)surf.texdata;
				var faceVertices = new BspGeometryVertex[face.numedges];

				Vector2 minUV0 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 minUV1 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 maxUV1 = new Vector2(float.MinValue, float.MinValue);
				for (int j = 0; j < (int)face.numedges; ++j)
				{

					var listOfEdgesIndex = (int)face.firstedge + j;
					if (listOfEdgesIndex >= listOfEdges.Length)
						throw new ApplicationException(string.Format("Edge list index {0} is out of range [0..{1}]", listOfEdgesIndex, listOfEdges.Length - 1));
					var edgeIndex = listOfEdges[listOfEdgesIndex];
					if (edgeIndex >= edges.Count)
						throw new ApplicationException(string.Format("Edge index {0} is out of range [0..{1}]", edgeIndex, edges.Count - 1));
					edge_t edge;
					if (edgeIndex >= 0)
					{
						edge = edges[edgeIndex];
					}
					else
					{
						var flippedEdge = edges[-edgeIndex];
						edge = new edge_t() { vertex0 = flippedEdge.vertex1, vertex1 = flippedEdge.vertex0 };
					}
					var edgesvertex0 = edge.vertex0;
					if (edgesvertex0 >= vertices.Count)
						throw new ApplicationException(string.Format("Vertex index {0} is out of range [0..{1}]", edgesvertex0, vertices.Count - 1));
					var edgesvertex1 = edge.vertex1;
					if (edgesvertex1 >= vertices.Count)
						throw new ApplicationException(string.Format("Vertex index {0} is out of range [0..{1}]", edgesvertex1, vertices.Count - 1));
					BspGeometryVertex vertex = BuildVertex(vertices[(short)edgesvertex0], (face.side == 0) ? plane.normal : -plane.normal, face, ref surf);
					faceVertices[j] = vertex;
					if (minUV0.X > vertex.UV0.X)
						minUV0.X = vertex.UV0.X;
					if (minUV0.Y > vertex.UV0.Y)
						minUV0.Y = vertex.UV0.Y;
					if (minUV1.X > vertex.UV1.X)
						minUV1.X = vertex.UV1.X;
					if (minUV1.Y > vertex.UV1.Y)
						minUV1.Y = vertex.UV1.Y;
					if (maxUV1.X < vertex.UV1.X)
						maxUV1.X = vertex.UV1.X;
					if (maxUV1.Y < vertex.UV1.Y)
						maxUV1.Y = vertex.UV1.Y;

				}
				minUV0.X = (float)System.Math.Floor(minUV0.X);
				minUV0.Y = (float)System.Math.Floor(minUV0.Y);

				if (textures[texture_id].Name == "TOOLS/TOOLSSKYBOX")
				{
					minUV0.X = 0;
					minUV0.Y = 0;
					for (int j = 0; j < (int)face.numedges; ++j)
						faceVertices[j].UV0 = new Vector2(0, 0);
				}


				var sizeLightmap = new Vector2(face.LightmapTextureSizeInLuxels[0] + 1, face.LightmapTextureSizeInLuxels[1] + 1);
				for (int j = 0; j < (int)face.numedges; ++j)
				{
					faceVertices[j].UV0 = faceVertices[j].UV0 - minUV0;
				}
				BspTexture lightMap = null;
				if (face.lightmap != -1 && (sizeLightmap.X > 0 && sizeLightmap.Y > 0))
				{
					if (!faceLightmapObjects.TryGetValue(face.lightmap, out lightMap))
					{
						var size2 = (sizeLightmap.X) * (sizeLightmap.Y);
						Bitmap faceLightmap = BuildFaceLightmap(face.lightmap, (int)sizeLightmap.X, (int)sizeLightmap.Y);
						faceLightmap = ReaderHelper.BuildSafeLightmap(faceLightmap);
						//faceLightmap = ReaderHelper.BuildSafeLightmapBothSides(faceLightmap); //Use safeBorderWidth = 2.0f; !!!
						lightMap = new BspEmbeddedTexture()
						{
							Name = "facelightmap" + face.lightmap,
							mipMaps = new Bitmap[] { faceLightmap },
							Width = faceLightmap.Width,
							Height = faceLightmap.Height
						};
						faceLightmapObjects[face.lightmap] = lightMap;
					}
				}
				else
				{
					for (int j = 0; j < (int)face.numedges; ++j)
					{
						faceVertices[j].UV1 = Vector2.Zero;
					}
				}

				var vert0 = faceVertices[0];
				for (int j = 1; j < faceVertices.Length - 1; ++j)
				{
					BspGeometryVertex vert1 = faceVertices[j];
					BspGeometryVertex vert2 = faceVertices[j + 1];
					var geoFace = new BspGeometryFace() { Vertex0 = vert0, Vertex1 = vert1, Vertex2 = vert2, Texture = textures[texture_id], Lightmap = lightMap };
					res.Faces.Add(geoFace);
				}
			}
			return res;
		}
		float safeOffset = 0.5f;//0.5f;
		float safeBorderWidth = 1.0f;

		public Bitmap BuildFaceLightmap(int p, int w, int h)
		{
			p /= 4;
			var bmp = new Bitmap(w, h);
			for (int y = 0; y < h; ++y)
				for (int x = 0; x < w; ++x)
				{
					var r = (byte)lightmap[p].X;
					var g = (byte)lightmap[p].Y;
					var b = (byte)lightmap[p].Z;

					bmp.SetPixel(x, y, Color.FromArgb(r,g,b));
					++p;
				}
			return bmp;
		}

		private byte HDR2Normal(double r)
		{
			if (r > 245)
				r = 245 + (r - 245) / 10.0;
			if (r < 10.0)
				r = 10 + (r - 10) / 10.0;
			if (r > 255) 
				r = 255;
			if (r < 0)
				r = 0;
			return (byte)r;
		}
		private BspGeometryVertex BuildVertex(Vector3 vector3, Vector3 n, face_t f, ref texinfo_t surf)
		{
			var res = new BspGeometryVertex();
			res.Position = vector3;
			res.Normal = n;
			res.UV0 = new Vector2(Vector3.Dot(surf.vectorS, vector3) + surf.distS, Vector3.Dot(surf.vectorT, vector3) + surf.distT);
			res.UV1 = new Vector2(
				Vector3.Dot(surf.lm_vectorS, vector3) + surf.lm_distS - (float)f.LightmapTextureMinsInLuxels[0],
				Vector3.Dot(surf.lm_vectorT, vector3) + surf.lm_distT - (float)f.LightmapTextureMinsInLuxels[1]);
			//if (f.LightmapTextureSizeInLuxels[0] == 0)
			res.UV1.X = (res.UV1.X + safeOffset) / ((float)f.LightmapTextureSizeInLuxels[0] + 1.0f +  safeBorderWidth);
			res.UV1.Y = (res.UV1.Y + safeOffset) / ((float)f.LightmapTextureSizeInLuxels[1] + 1.0f + safeBorderWidth);
			
			BspTexture tex = textures[(int)surf.texdata];
			res.UV0 = new Vector2(res.UV0.X / ((tex.Width != 0) ? (float)tex.Width : 256.0f), res.UV0.Y / ((tex.Height != 0) ? (float)tex.Height : 256.0f));
			return res;
		}
		private BspTreeNode BuildNode(dnode_t node)
		{
			var res = new BspTreeNode();
			res.Mins = new Vector3(node.box.mins[0], node.box.mins[1], node.box.mins[2]);
			res.Maxs = new Vector3(node.box.maxs[0], node.box.maxs[1], node.box.maxs[2]);

			res.PlaneDistance = planes[node.planenum].dist;
			res.PlaneNormal = planes[node.planenum].normal;

			if (0 != (node.front & 0x8000))
				res.Front = leaves[(ushort)~node.front];
			else
				res.Front = BuildNode(nodes[node.front]);
			if (0 != (node.back & 0x8000))
				res.Back = leaves[(ushort)~node.back];
			else
				res.Back = BuildNode(nodes[node.back]);
			return res;
		}
		private void ReadVertices(BinaryReader source)
		{
			SeekDir(source, header.Vertexes);
			if (header.Vertexes.size % 12 != 0)
				throw new Exception();
			int size = (int)(header.Vertexes.size / (12));
			vertices = new List<Vector3>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new Vector3();
				v.X = source.ReadSingle();
				v.Y = source.ReadSingle();
				v.Z = source.ReadSingle();
				if (float.IsInfinity(v.X) || float.IsInfinity(v.Y) || float.IsInfinity(v.Z)) 
					throw new ApplicationException("Wrong vertex " + i + " {" + v.X + ", " + v.Y + ", " + v.Z + "}");
				vertices.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.Vertexes.size + header.Vertexes.offset)
				throw new Exception();
		}
		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
		public abstract void ReadFaces(BinaryReader source);
		private void ReadEdges(BinaryReader source)
		{
			edges = ReaderHelper.ReadStructs<edge_t>(source, header.Edges.size, header.Edges.offset + startOfTheFile, 4);
		}
		private void ReadNodes(BinaryReader source)
		{
			nodes = ReaderHelper.ReadStructs<dnode_t>(source, header.Nodes.size, header.Nodes.offset + startOfTheFile, 32);
		}
		private void CollectLeavesInClusters()
		{
			int totalClusters = 0;
			for (int i = 0; i < dleaves.Count; ++i)
			{
				if (dleaves[i].cluster >= 0)
					clusters[dleaves[i].cluster].lists.Add(i);
			}
		}
		public abstract void ReadLeaves(BinaryReader source);
		private void ReadEntities(BinaryReader source)
		{
			SeekDir(source, header.Entities);
			int size = (int)(header.Entities.size);
			entities = Encoding.ASCII.GetString(source.ReadBytes(size));
		}
		public virtual void ReadLightmap(BinaryReader source)
		{
			SeekDir(source, header.Lighting);
			int size = (int)(header.Lighting.size);
			var bytes = source.ReadBytes(size);
			lightmap = new Vector3[size / 4];
			float maxValue = float.MinValue;
			for (uint i = 0; i < size / 4; ++i)
			{
				float k = (float)Math.Pow(2.0, (double)(sbyte)bytes[i*4+3]);
				float v = (float)bytes[i * 4 + 0] * k;
				if (!float.IsInfinity(v) && v > maxValue) maxValue = v;
				lightmap[i].X = v;

				v = (float)bytes[i * 4 + 1] * k;
				if (!float.IsInfinity(v) && v > maxValue) maxValue = v;
				lightmap[i].Y = v;

				v = (float)bytes[i * 4 + 2] * k;
				if (!float.IsInfinity(v) && v > maxValue) maxValue = v;
				lightmap[i].Z = v;
			}
			if (maxValue > 255)
				maxValue = 255;
			for (uint i = 0; i < size / 4; ++i)
			{
				lightmap[i].X = ClampTo255(lightmap[i].X * 255.0f / maxValue);
				lightmap[i].Y = ClampTo255(lightmap[i].Y * 255.0f / maxValue);
				lightmap[i].Z = ClampTo255(lightmap[i].Z * 255.0f / maxValue);
			}
		}

		private float ClampTo255(float p)
		{
			if (float.IsInfinity(p)) return 0;
			if (p < 0) return 0;
			if (p > 255) return 255;
			return p;
		}
		private void SeekDir(BinaryReader source, dentry_t dir)
		{
			source.BaseStream.Seek(startOfTheFile + dir.offset, SeekOrigin.Begin);
		}
	}
}
