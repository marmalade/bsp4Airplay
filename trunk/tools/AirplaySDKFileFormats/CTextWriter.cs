using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;

namespace AirplaySDKFileFormats
{
	public class CTextWriter : IDisposable
	{
		System.IO.TextWriter writer;
		string filePath;

		bool isLineOpened;
		int depth = 0;
		public string FileName
		{
			get
			{
				return filePath;
			}
		}
		public string FileDirectory
		{
			get
			{
				return Path.GetDirectoryName(Path.GetFullPath(filePath));
			}
		}
		public CTextWriter(string groupFilePath)
		{
			filePath = groupFilePath;
			writer = new StreamWriter(File.Create(filePath));
		}
		public void Write(string text)
		{
			writer.Write(text);
		}
		public void WriteLine(string text)
		{
			BeginWriteLine();
			Write(text);
			EndWriteLine();
		}
		public void WriteKeyVal(string name, object val)
		{
			WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1}", name, val));
		}
		public void WriteString(string name, string val)
		{
			if (val != null)
				WriteKeyVal(name, string.Format("\"{0}\"", val.Replace("\"", "\\\"")));
		}
		public void OpenChild(string name)
		{
			WriteLine(name);
			WriteLine("{");
			++depth;
		}
		public void CloseChild()
		{
			--depth;
			WriteLine("}");
		}
		public void BeginWriteLine()
		{
			if (isLineOpened) return;
			isLineOpened = true;
			for (int i = 0; i < depth; ++i)
				writer.Write('\t');
		}
		public void EndWriteLine()
		{
			writer.Write('\n');
			isLineOpened = false;
		}

		public void Close()
		{
			while (depth > 0)
				CloseChild();
			if (writer != null)
				writer.Close();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (writer != null)
				writer.Dispose();
		}

		#endregion

		public void WriteColour(string name, CIwColour val)
		{
			WriteArray(name, new byte[] { val.r, val.g, val.b, val.a });
		}
		public void WriteVec3(string name, CIwVec3 val)
		{
			WriteArray(name, new int[]{val.x, val.y, val.z});
		}
		public void WriteVec2(string name, CIwVec2 val)
		{
			WriteArray(name, new int[] { val.x, val.y });
		}
		public void WriteVec3Fixed(string name, CIwVec3 val)
		{
			WriteArray(name, new float[] { val.x / 4096.0f, val.y / 4096.0f, val.z / 4096.0f });
		}
		public void WriteVec2Fixed(string name, CIwVec2 val)
		{
			WriteArray(name, new float[] { val.x / 4096.0f, val.y / 4096.0f });
		}
		internal void WriteArray(string name, IList array)
		{
			BeginWriteLine();
			writer.Write(name);
			writer.Write(" ");
			writer.Write("{");
			if (array != null && array.Count > 0)
			{
				writer.Write(string.Format(CultureInfo.InvariantCulture, "{0}", array[0]));
				for (int i=1; i<array.Count; ++i)
					writer.Write(string.Format(CultureInfo.InvariantCulture, ", {0}", array[i]));
			}
			
			writer.Write("}");
			EndWriteLine();
		}

		internal void WriteVec2FixedFloat(string p, CIwVec2 p_2)
		{
			throw new NotImplementedException();
		}

		internal void WriteQuat(string name, CIwQuat val)
		{
			WriteArray(name, new float[] { val.w, val.x, val.y, val.z });

		}
	}
}
