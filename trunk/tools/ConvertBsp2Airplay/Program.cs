using System;
using System.Collections.Generic;
using System.Text;
using Bsp2AirplayAdapter;

namespace ConvertBsp2Airplay
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine(@"ConvertBsp2Airplay.exe maps\samplebox.bsp maps\samplebox.group");
				return;
			}
			(new Adapter()).Convert(args[0], args[1]);
		}
	}
}
