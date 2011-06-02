using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using System.IO;
using System.Globalization;
using Bsp2AirplayAdapter;
using DemFileFormat.Quake1;
using DemFileFormat;

namespace Tests
{
	[TestFixture]
	public class Q1Tests
	{
		//[Test]
		public void TestQ1()
		{
			(new Adapter()).Convert(@"..\data\maps\sg0503.bsp", @"..\data\maps\sg0503.group");
		}
		//[Test]
		public void TestQ1Demo()
		{
			var res = new DemoDocument();

			using (var f = File.OpenRead(@"..\data\demos\demo1.dem"))
			{
				using (var b = new BinaryReader(f))
				{
					(new Quake1DemoReader()).ReadDemo(b,res);
				}
			}
		}
		
	}
}