using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.IO;
using ReaderUtils;
using System.Drawing;

namespace BspFileFormat.Q2
{
	public class Quake2Reader : IBspReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected string entities;
		protected List<plane_t> planes;
		protected List<face_t> faces;
		protected List<node_t> nodes;
		protected List<leaf_t> dleaves;
		protected List<BspTreeLeaf> leaves = new List<BspTreeLeaf>();
		protected List<Vector3> vertices;
		protected List<texinfo_t> texInfos;
		Dictionary<string, BspTexture> texturesMap = new Dictionary<string, BspTexture>();
		protected ushort[] listOfFaces;
		protected short[] listOfEdges;
		protected List<edge_t> edges;
		protected List<cluster_t> clusters;
		private byte[] lightmap;
		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			ReadHeader(source);
			ReadEntities(source);
			ReadVertices(source);
			ReadPlanes(source);
			ReadEdges(source);
			ReadNodes(source);
			ReadFaces(source);
			ReadListOfFaces(source);
			ReadListOfEdges(source);
			ReadVisibilityList(source);
			ReadLeaves(source);
			ReadTextures(source);
			ReadLightmap(source);
			BuildLeaves();
			if (nodes != null && nodes.Count > 0)
			{
				dest.Tree = BuildNode(nodes[0]);
			}
			BuildVisibilityList();
			BuildClusters();
			ReaderHelper.BuildEntities(entities, dest);
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
		private void BuilFaceToLeafMap(FaceToLeafMap faceMap, int i, leaf_t dleaf)
		{
			if (dleaf.first_leaf_face >= 0 && dleaf.num_leaf_faces >= 0)
				for (int j = dleaf.first_leaf_face; j < dleaf.first_leaf_face + dleaf.num_leaf_faces; ++j)
					faceMap.Faces[(int)listOfFaces[j]].AddLeaf(i);
		}
		private void ReadVisibilityList(BinaryReader source)
		{
			SeekDir(source, header.visibility);
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
				Dictionary<int,bool> map = new Dictionary<int,bool>();
				if (dleaves[i].cluster >= 0)
					foreach (var c in clusters[dleaves[i].cluster].visiblity)
						foreach (var l in clusters[c].lists)
							map[l] = true;
				foreach (var j in map.Keys)
					if (i != j)
						leaves[i].VisibleLeaves.Add(leaves[j]);
			}
		}
		private BspTreeNode BuildNode(node_t node)
		{
			var res = new BspTreeNode();
			res.Mins = new Vector3(node.box.mins[0], node.box.mins[1], node.box.mins[2]);
			res.Maxs = new Vector3(node.box.maxs[0], node.box.maxs[1], node.box.maxs[2]);
			res.PlaneDistance = planes[(int)node.planenum].dist;
			res.PlaneNormal = planes[(int)node.planenum].normal;

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
		private void ReadLightmap(BinaryReader source)
		{
			SeekDir(source, header.lightmaps);
			int size = (int)(header.lightmaps.size);
			lightmap = source.ReadBytes(size);
		}

		private void BuildLeaves()
		{
			for (int i = 0; i < dleaves.Count; ++i)
			{
				leaves.Add(BuildLeaf(dleaves[i]));
			}
		}
		public virtual Bitmap BuildFaceLightmap(int p, int w, int h)
		{
			var b = new Bitmap(w, h);
			for (int y = 0; y < h; ++y)
				for (int x = 0; x < w; ++x)
				{
					b.SetPixel(x, y, Color.FromArgb(lightmap[p], lightmap[p + 1], lightmap[p + 2]));
					p += 3;
				}
			return b;
		}
		private BspTreeLeaf BuildLeaf(leaf_t dleaf)
		{
			var res = new BspTreeLeaf();
			res.Mins = new Vector3(dleaf.box.mins[0], dleaf.box.mins[1], dleaf.box.mins[2]);
			res.Maxs = new Vector3(dleaf.box.maxs[0], dleaf.box.maxs[1], dleaf.box.maxs[2]);
			//for (int i = dleaf.first_leaf_brush; i < dleaf.first_leaf_brush + dleaf.num_leaf_brushes; ++i)
			//{
			//    BspCollisionObject b = BuildLeafBrush((int)listOfBrushes[i]);
			//    if (b != null)
			//        res.Colliders.Add(b);
			//}
			return res;
		}
		//private BspCollisionObject BuildLeafBrush(int brushIndex)
		//{
		//    var b = this.brushes[brushIndex];
		//    BspCollisionConvexBrush brush = new BspCollisionConvexBrush();
		//    for (int i = b.brushside; i < b.brushside + b.n_brushsides; ++i)
		//    {
		//        var p = new Plane();
		//        p.Normal = this.planes[brushsides[i].plane].normal;
		//        p.Distance = this.planes[brushsides[i].plane].dist;
		//        brush.Planes.Add(p);
		//    }
		//    return brush;
		//}
		private void ReadListOfEdges(BinaryReader source)
		{
			SeekDir(source, header.faceEdgeTable);
			int size = (int)(header.faceEdgeTable.size / 4);
			listOfEdges = new short[size];
			for (int i = 0; i < size; ++i)
			{
				listOfEdges[i] = (short)source.ReadInt32();
			}
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
				plane_t plane = planes[face.plane];
				var surf = texInfos[face.texture_info];
				BspTexture texture = null;
				if (!texturesMap.TryGetValue(surf.name, out texture))
				{
					texture = new BspTextureReference() { Width = 128, Height = 128, Name = surf.name };
					texturesMap[surf.name] = texture;
				}
				var faceVertices = new BspGeometryVertex[face.ledge_num];
				Vector2 minUV0 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 minUV1 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 maxUV1 = new Vector2(float.MinValue, float.MinValue);

				for (int j = 0; j < (int)face.ledge_num; ++j)
				{

					var listOfEdgesIndex = (int)face.ledge_id + j;
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
					BspGeometryVertex vertex = BuildVertex(vertices[(short)edgesvertex0], (face.plane_side == 0) ? plane.normal : -plane.normal, ref surf);
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

				minUV1.X = (float)System.Math.Floor(minUV1.X);
				minUV1.Y = (float)System.Math.Floor(minUV1.Y);
				maxUV1.X = (float)System.Math.Ceiling(maxUV1.X);
				maxUV1.Y = (float)System.Math.Ceiling(maxUV1.Y);
				var sizeLightmap = maxUV1 - minUV1 + new Vector2(1, 1);
				for (int j = 0; j < (int)face.ledge_num; ++j)
				{
					faceVertices[j].UV0 = faceVertices[j].UV0 - minUV0;
					faceVertices[j].UV1.X = (faceVertices[j].UV1.X - minUV1.X + 0.5f) / sizeLightmap.X;
					faceVertices[j].UV1.Y = (faceVertices[j].UV1.Y - minUV1.Y + 0.5f) / sizeLightmap.Y;
				}
				BspTexture lightMap = null;
				if (face.lightmap != -1)
				{
					if (!faceLightmapObjects.TryGetValue(face.lightmap, out lightMap))
					{
						var size2 = (sizeLightmap.X) * (sizeLightmap.Y);
						lightMap = new BspEmbeddedTexture()
						{
							Name = "facelightmap" + face.lightmap,
							mipMaps = new Bitmap[] { BuildFaceLightmap(face.lightmap, (int)sizeLightmap.X, (int)sizeLightmap.Y) },
							Width = (int)sizeLightmap.X,
							Height = (int)sizeLightmap.Y
						};
						faceLightmapObjects[face.lightmap] = lightMap;
					}
				}

				var vert0 = faceVertices[0];
				for (int j = 1; j < faceVertices.Length - 1; ++j)
				{
					BspGeometryVertex vert1 = faceVertices[j];
					BspGeometryVertex vert2 = faceVertices[j + 1];
					var geoFace = new BspGeometryFace() { Vertex0 = vert0, Vertex1 = vert1, Vertex2 = vert2, Texture = texture, Lightmap = lightMap };
					res.Faces.Add(geoFace);
				}
			}
			return res;
		}

		private BspGeometryVertex BuildVertex(Vector3 vector3,  Vector3 n, ref texinfo_t surf)
		{
			var res = new BspGeometryVertex();
			res.Position = vector3;
			res.Normal = n;
			res.UV0 = new Vector2(Vector3.Dot(surf.vectorS, vector3) + surf.distS, Vector3.Dot(surf.vectorT, vector3) + surf.distT);
			res.UV1 = new Vector2(res.UV0.X / 16.0f, res.UV0.Y / 16.0f);

			float textureWidth = 128;// (float)textures[(int)surf.texture_id].Width;
			float textureHeight = 128;// (float)textures[(int)surf.texture_id].Height;
			res.UV0 = new Vector2(res.UV0.X / textureWidth, res.UV0.Y / textureHeight);
			return res;

		}
		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
		private void ReadTextures(BinaryReader source)
		{
			texInfos = ReaderHelper.ReadStructs<texinfo_t>(source, header.textureInformation.size, header.textureInformation.offset + startOfTheFile, 76);
		}
		private void ReadVertices(BinaryReader source)
		{
			SeekDir(source, header.vertices);
			int size = (int)(header.vertices.size/2);
			vertices = new List<Vector3>(size);
			for (int i = 0; i < size; ++i)
			{
				float x = source.ReadSingle();
				float y = source.ReadSingle();
				float z = source.ReadSingle();
				vertices.Add(new Vector3(x,y,z));
			}
		}
		private void ReadEdges(BinaryReader source)
		{
			edges = ReaderHelper.ReadStructs<edge_t>(source, header.edges.size, header.edges.offset + startOfTheFile, 4);
		}
		private void ReadEntities(BinaryReader source)
		{
			SeekDir(source, header.entities);
			int size = (int)(header.entities.size);
			entities = Encoding.ASCII.GetString(source.ReadBytes(size));
		}
		private void ReadLeaves(BinaryReader source)
		{
			int totalClusters = 0;
			dleaves = ReaderHelper.ReadStructs<leaf_t>(source, header.leaves.size, header.leaves.offset + startOfTheFile, 4 + 2 + 2 + 2 * 3 + 2 * 3 + 2*4);
			for (int i = 0; i < dleaves.Count; ++i)
			{
				if (dleaves[i].cluster > totalClusters) totalClusters = dleaves[i].cluster;
				if (dleaves[i].cluster >= 0)
					clusters[dleaves[i].cluster].lists.Add(i);
			}
		}
		private void ReadNodes(BinaryReader source)
		{
			nodes = ReaderHelper.ReadStructs<node_t>(source, header.nodes.size, header.nodes.offset + startOfTheFile, 4 + 4 + 4 + 2 * 3 + 2 * 3 + 2 + 2);
		}
		private void ReadFaces(BinaryReader source)
		{
			faces = ReaderHelper.ReadStructs<face_t>(source, header.faces.size, header.faces.offset + startOfTheFile, 2+2+4+2+2+4+4);

		}
		private void ReadListOfFaces(BinaryReader source)
		{
			listOfFaces = ReaderHelper.ReadUInt16Array(source, header.leafFaceTable.size, header.leafFaceTable.offset);
		}
		private void ReadPlanes(BinaryReader source)
		{
			planes = ReaderHelper.ReadStructs<plane_t>(source, header.planes.size, header.planes.offset + startOfTheFile, 20);
		}
		private void SeekDir(BinaryReader source, dentry_t dir)
		{
			source.BaseStream.Seek(startOfTheFile + dir.offset, SeekOrigin.Begin);
		}
	}
}
