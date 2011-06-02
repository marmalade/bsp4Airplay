using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using NUnit.ConsoleRunner;
using System.IO;

namespace Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			//var a = File.ReadAllBytes(@"D:\Downloads\QUAKE_SW\ID1\gfx\palette.lmp");
			//var a = File.ReadAllBytes(@"C:\Sierra\Half-LifeUplink\valve\gfx\palette.lmp");
			//StringBuilder sb = new StringBuilder();
			//sb.Append("new Color[] {");
			//for (int i = 0; i < 256; ++i)
			//{
			//    sb.Append(string.Format("Color.FromArgb({0},{1},{2}), ", a[i * 3], a[i * 3 + 1], a[i * 3 + 2]));
			//}
			//sb.Append("}");

			List<string> a = new List<string>();
			a.Add(typeof(Program).Assembly.Location);
			a.AddRange(args);
			Runner.Main(a.ToArray());

			Console.ReadLine();
		}
	}
}
