using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BspFileFormat;
using Bsp2AirplayAdapter;

namespace Tests
{
	[TestFixture]
	public class Q2Tests
	{
		//[Test]
		public void TestQ2()
		{
			//(new Adapter()).Convert(@"..\data\maps\moo.bsp", @"..\data\maps\moo.group");
			(new Adapter()).Convert(@"..\data\maps\match1.bsp", @"..\data\maps\match1.group");
		}
	}
}
