using System.IO;
using System.Reflection;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.Util
{
	public static class ScreenCapture
	{
		private static string exePath;
		private static string defaultFolder;
		private static string defaultName;

		static ScreenCapture()
		{
			exePath = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			defaultFolder = Path.Combine( exePath, "Screenshots" );
			defaultName = "Screenshot";
		}

		private static int FileCount( string folderPath )
		{
			return Directory.GetFiles( folderPath, "*.*", SearchOption.TopDirectoryOnly ).Length;
		}

		public static void Capture( GraphicsDevice device, string filePath = null, ImageFileType format = ImageFileType.Png )
		{
			if( filePath == null )
			{
				if( !Directory.Exists( defaultFolder ) )
				{
					Directory.CreateDirectory( defaultFolder );
				}
				filePath = Path.Combine( defaultFolder, defaultName + "_" + ( FileCount( defaultFolder ) + 1 ).ToString() + "." + format.ToString().ToLower() );
			}
			device.BackBuffer.Save( filePath, format );
		}

		public static Texture2D CaptureToTexture( GraphicsDevice device )
		{
			return Texture2D.New( device, device.BackBuffer );
		}
	}
}