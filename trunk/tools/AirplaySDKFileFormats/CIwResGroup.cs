using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AirplaySDKFileFormats
{
	public class CIwResGroup: CIwManaged
	{
		protected CIwManagedList list = new CIwManagedList();

		public void AddChild(CIwResGroup pGroup)
		{
			throw new NotImplementedException();
		}
		public void AddRes(CIwResource pData)
		{
			list.Add(pData);
		}
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);

			writer.WriteLine("shared true");


			var materials = new List<CIwMaterial>();
			var geos = new List<CIwModel>();
			var textures = new List<CIwTexture>();
			var others = new List<CIwManaged>();
			foreach (var l in list)
			{
				if (l is CIwMaterial)
					materials.Add((CIwMaterial)l);
				else if (l is CIwModel)
					geos.Add((CIwModel)l);
				else if (l is CIwTexture)
					textures.Add((CIwTexture)l);
				else
					others.Add((CIwManaged)l);
			}
			string name = Path.GetFileNameWithoutExtension(writer.FileName);
			string subdir = Path.Combine(writer.FileDirectory, name);
			if (textures.Count > 0)
			{
				foreach (var l in textures)
				{
					l.WrtieToStream(writer);
				}
			}
			if (materials.Count > 0)
			{
				writer.WriteLine(string.Format("\"./{0}.mtl\"", name));
				using (CTextWriter subWriter = new CTextWriter(Path.Combine(writer.FileDirectory, name + ".mtl")))
				{
					foreach (var l in materials)
					{
						l.WrtieToStream(subWriter);
					}
					subWriter.Close();
				}
			}
			if (geos.Count > 0)
			{
				foreach (var l in geos)
				{
					var geoFileName = l.Name;
					foreach (var c in Path.GetInvalidFileNameChars())
						geoFileName = geoFileName.Replace(c, '_');
					writer.WriteLine(string.Format("\"./{0}.geo\"", geoFileName));
					
					string fullGeoFileName = Path.Combine(writer.FileDirectory, geoFileName + ".geo");

					using (CTextWriter subWriter = new CTextWriter(fullGeoFileName))
					{
						l.WrtieToStream(subWriter);
						subWriter.Close();
					}

					if (l.Skin != null)
					{
						if (l.Skin.skeleton != null)
						{
							writer.WriteLine(string.Format("\"./{0}.skel\"", geoFileName));
							fullGeoFileName = Path.ChangeExtension(fullGeoFileName, ".skel");
							using (CTextWriter subWriter = new CTextWriter(fullGeoFileName))
							{
								l.Skin.skeleton.WrtieToStream(subWriter);
								subWriter.Close();
							}
						}

						writer.WriteLine(string.Format("\"./{0}.skin\"", geoFileName));
						fullGeoFileName = Path.ChangeExtension(fullGeoFileName, ".skin");
						using (CTextWriter subWriter = new CTextWriter(fullGeoFileName))
						{
							l.Skin.WrtieToStream(subWriter);
							subWriter.Close();
						}
						if (l.Skin.Animations != null)
						{
							var fullAnimFileName = Path.Combine(Path.Combine(Path.GetDirectoryName(fullGeoFileName), ".."), "animations");
							if (!Directory.Exists(fullAnimFileName))
								Directory.CreateDirectory(fullAnimFileName);
							fullAnimFileName = Path.Combine(fullAnimFileName, geoFileName);
							if (!Directory.Exists(fullAnimFileName))
								Directory.CreateDirectory(fullAnimFileName);
							foreach (var a in l.Skin.Animations)
							{
								string animationName = a.Name;
								foreach (var c in Path.GetInvalidFileNameChars())
									animationName = animationName.Replace(c, '_');

								writer.WriteLine(string.Format("\"../animations/{0}/{1}.anim\"", geoFileName, animationName));
								fullGeoFileName = Path.Combine(fullAnimFileName, animationName+".anim");
								using (CTextWriter subWriter = new CTextWriter(fullGeoFileName))
								{
									a.WrtieToStream(subWriter);
									subWriter.Close();
								}
							}
						}
					}
				}
			}
			//if (geos.Count > 0)
			//{
			//    if (!string.IsNullOrEmpty(subdir) && !Directory.Exists(subdir))
			//        Directory.CreateDirectory(subdir);
			//    foreach (var l in geos)
			//    {
			//        var geoFileName = l.Name;
			//        foreach (var c in Path.GetInvalidFileNameChars())
			//            geoFileName = geoFileName.Replace(c,'_');
			//        writer.WriteLine(string.Format("\"./{0}/{1}.geo\"", Path.GetFileName(subdir),geoFileName));
			//        using (CTextWriter subWriter = new CTextWriter(Path.Combine(subdir, geoFileName + ".geo")))
			//        {
			//            l.WrtieToStream(subWriter);
			//            subWriter.Close();
			//        }
			//    }
			//}
			foreach (var l in others)
			{
				l.WrtieToStream(writer);
			}
		}
	}
}
