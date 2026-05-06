using System;
using System.IO;

using Godot;

namespace NagaisoraFramework
{
	using DataSystem;

	public static class GlobalData
	{
		public static ConfigData ConfigData;
		public static ScoreData ScoreData;

		public static void LoadConfigData(string path)
		{
			ConfigData = ConfigData.Default;

			FileStream ConfigDataStream = new(path, FileMode.OpenOrCreate, System.IO.FileAccess.Read);

			if (ConfigDataStream.Length == 0)
			{
				GD.Print($"[Framework Kernel] Config data file is no fount, apply default config and save");

				ConfigData.SaveConfig(ConfigDataStream, ConfigData);
			}
			else
			{
				GD.Print($"[Framework Kernel] Load config data file");
			}

			try
			{
				ConfigData = ConfigData.LoadConfig(ConfigDataStream);
			}
			catch (Exception e)
			{
				throw new Exception("[Framework Kernel] Config data load failed, the file may be corrupted", e);
			}

			ConfigDataStream.Close();
		}

		public static void LoadScoreData(string path)
		{
			ScoreData = ScoreData.ScoreDataLoad(path);
		}

		public static void SaveScoreData()
		{
			ScoreData.TotalRunTime = MainSystem.ClockSystem.TotalRunTime;
			ScoreData.ScoreDataSave(ScoreData, DataPathDefine.ScoreData);
		}

		public static void FlushALLDoneScoreData(string path)
		{
			ScoreData = ScoreData.AllDone();
		}

		public static void LoadBGMData(string path)
		{
			if (!File.Exists(path))
			{
				GD.Print("[Framework Kernel] BGM data not found in the specified path，load skip");
				return;
			}

			GD.Print($"[Framework Kernel] Load BGM data");
			BGMPackData BGMPackData = BGMPackData.FromBinary(File.ReadAllBytes(path));
			MainSystem.BGMControler.BGMPackData = BGMPackData;

			throw new InvalidProgramException("[Framework Kernel] LoadBGMData() is not supported in non-Unity Runtime platform");
		}
	}
}
