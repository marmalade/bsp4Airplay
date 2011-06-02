using System.Drawing;
using System.IO;
namespace AirplaySDKFileFormats
{
	public class CIwTexture : CIwResource
	{
		public string FilePath;
		public Bitmap Bitmap;

		public override void WrtieToStream(CTextWriter writer)
		{
			string filePath = Path.Combine(Path.GetDirectoryName(writer.FileName), FilePath);
			if (Bitmap != null)
			{
				Bitmap.Save(filePath);
				writer.WriteLine(string.Format("\"{0}\"", FilePath.Replace("\"", "\\\"")));
			}
			else
			{
				if (File.Exists(filePath))
					writer.WriteLine(string.Format("\"{0}\"", FilePath.Replace("\"", "\\\"")));
				else
					writer.WriteLine(string.Format("#\"{0}\"", FilePath.Replace("\"", "\\\"")));

			}
		}
	}
}
