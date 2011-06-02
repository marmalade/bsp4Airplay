using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Drawing;

namespace ReaderUtils
{
	public class Entity
	{
		public string ClassName;
		public Vector3 Origin = Vector3.Zero;
		public List<KeyValuePair<string, string>> Values = new List<KeyValuePair<string, string>>();
	}
	public interface IEntityContainer
	{
		void AddEntity(Entity entity);
	}
	public static class ReaderHelper
	{
		public static void BuildEntities(string entities, IEntityContainer dest)
		{
			var lines = entities.Split(new char[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
			Entity entity = null;
			foreach (var rawline in lines)
			{
				var line = rawline.Trim();
				if (line == "{")
				{
					entity = new Entity();
					dest.AddEntity(entity);
					continue;
				}
				if (line == "}")
				{
					entity = null;
					continue;
				}
				int keyStartsAt = line.IndexOf('\"')+1;
				if (keyStartsAt <= 0)
					continue;
				int keyEndsAt = line.IndexOf('\"', keyStartsAt);
				int valueStartsAt = line.IndexOf('\"', keyEndsAt+1)+1;
				var key = line.Substring(keyStartsAt, keyEndsAt - keyStartsAt);
				var val = line.Substring(valueStartsAt, line.Length - 1 - valueStartsAt);
				if (key == "classname")
				{
					entity.ClassName = val;
				}
				else if (key == "origin")
				{
					var vals = val.Split(new char[]{' '});
					entity.Origin = new Vector3(
						float.Parse(vals[0], CultureInfo.InvariantCulture),
						float.Parse(vals[1], CultureInfo.InvariantCulture),
						float.Parse(vals[2], CultureInfo.InvariantCulture)
						);
				}
				else
				{
					entity.Values.Add(new KeyValuePair<string, string>(key,val));
				}
			}
		}

		public static List<T> ReadStructs<T>(System.IO.BinaryReader source, uint size, long offset, uint itemSize) where T:new()
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			if (size % itemSize != 0)
				throw new ArgumentException("Wrong size "+itemSize+" for " + typeof(T).Name);
			int numItems = (int)(size / itemSize);
			var planes = new List<T>(numItems);
			var Read = typeof(T).GetMethod("Read");
			if (Read == null)
				throw new ArgumentException("No Read(stream) method in "+typeof(T).Name);
			for (int i = 0; i < numItems; ++i)
			{
				var v = new T();
				Read.Invoke(v, new object[] { source });
				//v.Read(source);
				planes.Add(v);
			}
			if (source.BaseStream.Position != size + offset)
				throw new ArgumentException("Wrong item size for " + typeof(T).Name);
			return planes;
		}

		public static ushort[] ReadUInt16Array(BinaryReader source, uint bufSize, uint offset)
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			int size = (int)(bufSize / 2);
			var listOfFaces = new ushort[size];
			for (int i = 0; i < size; ++i)
			{
				listOfFaces[i] = source.ReadUInt16();
			}
			return listOfFaces;
		}
		public static uint[] ReadUInt32Array(BinaryReader source, uint bufSize, uint offset)
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			int size = (int)(bufSize / 4);
			var listOfFaces = new uint[size];
			for (int i = 0; i < size; ++i)
			{
				listOfFaces[i] = source.ReadUInt32();
			}
			return listOfFaces;
		}
		public static int[] ReadInt32Array(BinaryReader source, uint bufSize, uint offset)
		{
			source.BaseStream.Seek(offset, SeekOrigin.Begin);
			int size = (int)(bufSize / 4);
			var listOfFaces = new int[size];
			for (int i = 0; i < size; ++i)
			{
				listOfFaces[i] = source.ReadInt32();
			}
			return listOfFaces;
		}

		public static string ReadStringSZ(BinaryReader source)
		{
			var buf = new List<byte>(16);
			for (; ; )
			{
				var b = source.ReadByte();
				if (b == 0) break;
				buf.Add(b);
			}
			return Encoding.ASCII.GetString(buf.ToArray());
		}

		public static Bitmap BuildSafeLightmap(System.Drawing.Bitmap faceLightmap)
		{
			var res = new Bitmap(faceLightmap.Width + 1, faceLightmap.Height + 1);
			using (var gr = Graphics.FromImage(res))
			{
				gr.Clear(Color.Red);
				gr.DrawImageUnscaledAndClipped(faceLightmap,new Rectangle(new Point(0,0), new Size(1,1)));
				gr.DrawImageUnscaledAndClipped(faceLightmap,new Rectangle(new Point(0,1), new Size(1,faceLightmap.Height)));
				gr.DrawImageUnscaledAndClipped(faceLightmap,new Rectangle(new Point(1,0), new Size(faceLightmap.Width,1)));
				gr.DrawImage(faceLightmap, new Point(1, 1));
			}
			return res;
		}

		public static Bitmap BuildSafeLightmapBothSides(Bitmap faceLightmap)
		{
			var res = new Bitmap(faceLightmap.Width + 2, faceLightmap.Height + 2);
			using (var gr = Graphics.FromImage(res))
			{
				gr.Clear(Color.Red);
				gr.DrawImage(faceLightmap, new Point(1, 1));
				gr.Flush();
			}
			for (int x = 1; x <= faceLightmap.Width; ++x)
			{
				res.SetPixel(x, 0, Color.Red);// faceLightmap.GetPixel(x - 1, 0));
				res.SetPixel(x, res.Height - 1, Color.Green);//faceLightmap.GetPixel(x - 1, faceLightmap.Height - 1));
			}
			for (int y = 1; y <= faceLightmap.Height; ++y)
			{
				res.SetPixel(0, y, Color.Red);// faceLightmap.GetPixel(0, y - 1));
				res.SetPixel(res.Width - 1, y, Color.Green);// faceLightmap.GetPixel(faceLightmap.Width - 1, y - 1));
			}
			res.SetPixel(0, 0, Color.Red);// faceLightmap.GetPixel(0, 0));
			res.SetPixel(0, res.Height - 1, Color.Green);// faceLightmap.GetPixel(0, faceLightmap.Height - 1));
			res.SetPixel(res.Width - 1, res.Height - 1, Color.Green);// faceLightmap.GetPixel(faceLightmap.Width - 1, faceLightmap.Height - 1));
			res.SetPixel(res.Width - 1, 0, Color.Green);// faceLightmap.GetPixel(faceLightmap.Width - 1, 0));
			return res;
		}
	}
}
