using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BspFileFormat.Q1HL1;
using BspFileFormat.Utils;
using BspFileFormat.HL2;
using BspFileFormat.Q3;
using BspFileFormat.Q2;
using ReaderUtils;

namespace BspFileFormat
{
	public class BspDocument: IEntityContainer
	{
		List<Entity> entities = new List<Entity>();
		public IList<Entity> Entities
		{
			get
			{
				return entities;
			}
		}
		public void AddEntity(Entity e)
		{
			Entities.Add(e);
		}
		List<BspGeometry> models = new List<BspGeometry>();
		public IList<BspGeometry> Models
		{
			get
			{
				return models;
			}
		}

		public BspTreeElement Tree
		{
			get;
			set;
		}

		public BspDocument()
		{
		}

		public static BspDocument Load(string p)
		{
			string name = Path.GetFileNameWithoutExtension(p);
			var r = Load(File.OpenRead(p));
			if (r.Name == null)
				r.Name = name;
			return r;
		}

		private static BspDocument Load(Stream fileStream)
		{
			using (BinaryReader r = new BinaryReader(fileStream))
			{
				return Load(r);
			}
		}

		private static BspDocument Load(BinaryReader r)
		{
			var res = new BspDocument();
			var pos = r.BaseStream.Position;
			var magic = r.ReadUInt32();
			IBspReader reader = null;
			if (magic == 0x1D)
				reader = new Quake1Reader();
			else if (magic == 0x1E)
				reader = new HL1Reader();
			else if (magic == 0x50534256)
			{
				magic = r.ReadUInt32();
				if (magic == 17)
					reader = new HL2Reader17();
				else if (magic == 19)
					reader = new HL2Reader19();
				else if (magic == 20)
					reader = new HL2Reader20();
			}
			else if (magic == 0x50534249)
			{
				magic = r.ReadUInt32();
				if (magic == 0x26)
					reader = new Quake2Reader();
				else if (magic == 0x2E)
					reader = new Quake3Reader();
				else if (magic == 0x2F)
					reader = new QuakeLiveReader();
			}
			if (reader == null)
				throw new ApplicationException("Format is not supported");
			r.BaseStream.Seek(pos, SeekOrigin.Begin);
			reader.ReadBsp(r, res);
			return res;
		}

		public void AddModel(BspGeometry bspGeometry)
		{
			models.Add(bspGeometry);
		}

		public string Name { get; set; }
	}
}
