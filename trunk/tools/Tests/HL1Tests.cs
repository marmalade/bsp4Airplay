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
using DemFileFormat.HL1;
using ModelFileFormat;
using Mdl2AirplayAdapter;

namespace Tests
{
	[TestFixture]
	public class HL1Tests
	{
		//[Test]
		public void TestHL1()
		{
			//(new Adapter()).Convert(@"..\data\maps\hldemo1.bsp", @"..\data\maps\hldemo1.group");
			(new Adapter()).Convert(@"..\data\maps\samplebox.bsp", @"..\data\maps\samplebox.group");
			//(new Adapter()).Convert(@"..\data\maps\madcrabs.bsp", @"..\data\maps\madcrabs.group");
		}
		//[Test]
		public void TestCS_1_6_dust()
		{
			(new Adapter()).Convert(@"..\data\maps\de_dust.bsp", @"..\data\maps\de_dust.group");
		}
		//[Test]
		public void TestCS_1_6_mansion()
		{
			(new Adapter()).Convert(@"..\data\maps\cs_mansion.bsp", @"..\data\maps\cs_mansion.group");
		}
		//[Test]
		public void TestCS_1_6_aztec()
		{
			(new Adapter()).Convert(@"..\data\maps\de_aztec.bsp", @"..\data\maps\de_aztec.group");
		}
		[Test]
		public void TestCSModel()
		{
			(new ModelAdapter()).Convert(@"D:\Program Files\Valve\cstrike\models\player\guerilla\guerilla.mdl", @"..\data\models\guerilla.group");
		}
		[Test]
		public void TestCSHandsModel()
		{
			(new ModelAdapter()).Convert(@"D:\Program Files\Valve\cstrike\models\v_m4a1.mdl", @"..\data\models\v_m4a1.group");
		}
		
		//[Test]
		public void TestDemo()
		{
			var res = new DemoDocument();

			using (var f = File.OpenRead(@"..\data\demos\dustdemo.dem"))
			{
				using (var b = new BinaryReader(f))
				{
					(new HL1DemoReader()).ReadDemo(b, res);
				}
			}
		}
	}
}
