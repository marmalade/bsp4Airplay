using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat
{
	public class ModelDocument
	{
		public string Name { get; set; }

		public List<ModelMesh> Meshes = new List<ModelMesh>();
		public List<ModelAnimation> Animations = new List<ModelAnimation>();
		public List<ModelBone> Bones = new List<ModelBone>();
		public static ModelDocument Load(string p)
		{
			string name = Path.GetFileNameWithoutExtension(p);
			ModelDocument r = Load(File.OpenRead(p));
			if (r.Name == null)
				r.Name = name;
			return r;
		}

		private static ModelDocument Load(Stream fileStream)
		{
			using (BinaryReader r = new BinaryReader(fileStream))
			{
				return Load(r);
			}
		}

		private static ModelDocument Load(BinaryReader r)
		{
			var res = new ModelDocument();
			var pos = r.BaseStream.Position;
			IModelReader reader = null;

			var magic = r.ReadUInt32();

			if (magic == 0x54534449)
			{
				magic = r.ReadUInt32();
				if (magic == 44 || magic == 48)
					reader = new HL2.MdlReader();
				else if (magic == 10)
					reader = new HL1.MdlReader();
			}
			else if (magic == 0x4F504449)
				reader = new Q1.MdlReader();

			if (reader == null)
				throw new ApplicationException("Format is not supported");
			r.BaseStream.Seek(pos, SeekOrigin.Begin);
			reader.ReadModel(r, res);
			return res;
		}
	}
}
