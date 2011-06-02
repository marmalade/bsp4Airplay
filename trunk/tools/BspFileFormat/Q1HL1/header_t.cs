
using ReaderUtils;
using System.IO;
namespace BspFileFormat.Q1HL1
{
	public class header_t
	{
		public uint version;
		public dentry_t entities;
		public dentry_t planes;
		public dentry_t miptex;
		public dentry_t vertices;
		public dentry_t visilist;
		public dentry_t nodes;
		public dentry_t texinfo;
		public dentry_t faces;
		public dentry_t lightmaps;
		public dentry_t clipnodes;
		public dentry_t leaves;
		public dentry_t lface;
		public dentry_t edges;
		public dentry_t ledges;
		public dentry_t models;

		public void Read(BinaryReader source)
		{
			version = source.ReadUInt32();
			entities.Read(source);
			planes.Read(source);
			miptex.Read(source);
			vertices.Read(source);
			visilist.Read(source);
			nodes.Read(source);
			texinfo.Read(source);
			faces.Read(source);
			lightmaps.Read(source);
			clipnodes.Read(source);
			leaves.Read(source);
			lface.Read(source);
			edges.Read(source);
			ledges.Read(source);
			models.Read(source);
		}
	}
}