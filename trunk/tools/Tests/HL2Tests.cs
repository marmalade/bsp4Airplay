using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using Bsp2AirplayAdapter;

namespace Tests
{
	[TestFixture]
	public class HL2Tests
	{
		//[Test]
		public void TestHL2_19()
		{
			(new Adapter()).Convert(@"..\data\maps\leonHL2_1.bsp", @"..\data\maps\leonHL2_1.group");
		}
		//[Test]
		public void TestHL2_20()
		{
			(new Adapter()).Convert(@"..\data\maps\al_test_map_02.bsp", @"..\data\maps\al_test_map_02.group");
		}
	}
}
