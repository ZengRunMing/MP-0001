using Godot;

using System.IO;

namespace NagaisoraFramework
{
	public static class DataPathHelper
	{
		public static readonly string LocalPath;

		static DataPathHelper()
		{
			if (OS.HasFeature("editor"))
			{
				LocalPath = ProjectSettings.GlobalizePath("res://../Data");

				if (!Directory.Exists(LocalPath))
				{
					Directory.CreateDirectory(LocalPath);
				}
			}
			else
			{
				LocalPath = Path.GetDirectoryName(OS.GetExecutablePath());
			}
		}

		public static bool LocalPathFileExists(string path)
		{
			if (File.Exists(Path.Combine(LocalPath, path)))
			{
				return true;
			}
			return false;
		}

		public static bool LocalPathDirectoryExists(string path)
		{
			if (Directory.Exists(Path.Combine(LocalPath, path)))
			{
				return true;
			}
			return false;
		}
	}
}
