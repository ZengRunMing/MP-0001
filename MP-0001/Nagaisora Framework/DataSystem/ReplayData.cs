using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NagaisoraFramework.DataSystem
{
	[Serializable]
	public struct ReplayData
	{
		public const string DataHead = "NRPY";

		public int Seed;

		public string Name;
		public DateTime SaveTime;
		public int Score;
		public byte Livel;
		public byte Player;
		public byte State;
		public string User;

		public byte StartLife;
		public byte StartBomb;
		public byte EndLife;
		public byte EndBomb;
		public float Power;
		public int Point;
		public int Graze;
		public int GetSpellCardCount;
		public byte DeathCount;

		public StageReplayData[] StageReplayDatas;

		public static ReplayData FromBinary(byte[] binary)
		{
			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory);
			
			memory.Position = 0;

			try
			{
				string ReadHead = new(reader.ReadChars(DataHead.Length));

				if (ReadHead != DataHead)
				{
					throw new InvalidDataException($"数据标识头不匹配");
				}

				ReplayData replayData = new()
				{
					Seed = reader.ReadInt32(),
					Name = reader.ReadString(),
					User = reader.ReadString(),
					SaveTime = DateTime.FromBinary(reader.ReadInt64()),
					Player = reader.ReadByte(),
					Livel = reader.ReadByte(),
					Score = reader.ReadInt32(),
					Power = reader.ReadSingle(),
					Point = reader.ReadInt32(),
					Graze = reader.ReadInt32(),
					State = reader.ReadByte(),
					StartLife = reader.ReadByte(),
					StartBomb = reader.ReadByte(),
					EndLife = reader.ReadByte(),
					EndBomb = reader.ReadByte(),
					GetSpellCardCount = reader.ReadInt32(),
					DeathCount = reader.ReadByte()
				};

				List<StageReplayData> stageReplayDatas = [];

				int Length = reader.ReadInt32();
				for (int a = 0; a < Length; a++)
				{
					StageReplayData stageReplayData = new()
					{
						Name = reader.ReadString(),
						Score = reader.ReadUInt32(),
					};

					int ScreenshotDataLength = reader.ReadInt32();
					if (ScreenshotDataLength > 0)
					{
						stageReplayData.ScreenshotData = reader.ReadBytes(ScreenshotDataLength);
					}

					Dictionary<string, List<ReplayActionData>> actionDatas = [];

					int ActionsCount = reader.ReadInt32();

					for (int ld = 0; ld < ActionsCount; ld++)
					{
						string keyname = reader.ReadString();
						int valuecount = reader.ReadInt32();

						actionDatas.Add(keyname, []);

						for (int b = 0; b < valuecount; b++)
						{
							actionDatas[keyname].Add(new(reader.ReadUInt32(), reader.ReadUInt16()));
						}
					}

					stageReplayData.ActionDatas = actionDatas;

					stageReplayDatas.Add(stageReplayData);
				}

				replayData.StageReplayDatas = [.. stageReplayDatas];
				reader.Close();

				return replayData;
			}
			catch (Exception E)
			{
				reader.Close();
				throw new FileLoadException($"数据无法正常读取，错误信息 : {E.Message}");
			}
		}

		public byte[] ToBinary()
		{
			MemoryStream memoryStream = new();
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			binaryWriter.Write(DataHead.ToCharArray());

			binaryWriter.Write(Seed);
			binaryWriter.Write(Name);
			binaryWriter.Write(User);
			binaryWriter.Write(SaveTime.ToBinary());
			binaryWriter.Write(Player);
			binaryWriter.Write(Livel);
			binaryWriter.Write(Score);
			binaryWriter.Write(Power);
			binaryWriter.Write(Point);
			binaryWriter.Write(Graze);
			binaryWriter.Write(State);
			binaryWriter.Write(StartLife);
			binaryWriter.Write(StartBomb);
			binaryWriter.Write(EndLife);
			binaryWriter.Write(EndBomb);
			binaryWriter.Write(GetSpellCardCount);
			binaryWriter.Write(DeathCount);

			if (StageReplayDatas == null)
			{
				binaryWriter.Write(0);
				goto Return;
			}

			binaryWriter.Write(StageReplayDatas.Length);
			foreach (StageReplayData item in StageReplayDatas)
			{
				binaryWriter.Write(item.Name);
				binaryWriter.Write(item.Score);

				if (item.ScreenshotData == null || item.ScreenshotData.Length == 0)
				{
					binaryWriter.Write(0);
				}
				else
				{
					binaryWriter.Write(item.ScreenshotData.Length);
					binaryWriter.Write(item.ScreenshotData);
				}

				if (item == null)
				{
					binaryWriter.Write(0);
					continue;
				}

				binaryWriter.Write(item.ActionDatasCount);

				foreach (KeyValuePair<string, List<ReplayActionData>> item0 in item.ActionDatas)
				{
					binaryWriter.Write(item0.Key);
					binaryWriter.Write(item0.Value.Count);
					foreach (ReplayActionData item1 in item0.Value)
					{
						binaryWriter.Write(item1.GameTime);
						binaryWriter.Write(item1.DownKeys);
					}
				}
			}

			Return:

			binaryWriter.Close();
			return memoryStream.ToArray();
		}

		public static ReplayData[] LoadReplayDatas(string Path)
		{
			if (Path == null || Path == "")
			{
				Path = DataPathDefine.ReplayPath;
			}

			if (!Directory.Exists(Path))
			{
				throw new DirectoryNotFoundException("LoadReplayDatas() => 未找到Replay文件夹");
			}

			DirectoryInfo direction = new(Path);

			FileInfo[] file = direction.GetFiles($"*.rpy", SearchOption.AllDirectories);

			List<ReplayData> ReplayDatas = [];

			for (int i = 0; i < file.Length; i++)
			{
				ReplayDatas.Add(LoadReplayData(file[i].FullName));
			}

			return [.. ReplayDatas];
		}

		public static ReplayData LoadReplayData(string Path)
		{
			return FromBinary(File.ReadAllBytes(Path));
		}

		public static void SaveReplay(string Path, ReplayData STL)
		{
			FileStream FS = new(Path, FileMode.CreateNew, FileAccess.Write);
			BinaryWriter BWF = new(FS, Encoding.UTF8);

			byte[] binary = STL.ToBinary();
			BWF.Write(binary);

			BWF.Close();
			FS.Close();
		}
	}

	[Serializable]
	public class StageReplayData
	{
		public int ActionDatasCount => ActionDatas.Count;

		public string Name;

		public uint Score;

		public byte[] ScreenshotData;

		public Dictionary<string, List<ReplayActionData>> ActionDatas;
		public Dictionary<uint, int> Keys;

		public StageReplayData()
		{
			ActionDatas = [];
			Keys = [];
		}

		public StageReplayData(string name, uint score) : this()
		{
			Name = name;
			Score = score;
		}

		public void AddAction(string name, ReplayActionData data)
		{
			if (!ActionDatas.TryGetValue(name, out List<ReplayActionData> value))
			{
				value = [];
				ActionDatas.Add(name, value);
			}

			value.Add(data);
		}

		public void RemoveAction(string name, ReplayActionData data)
		{
			ActionDatas[name].Remove(data);
		}

		public void AddKey(uint gametime, int value)
		{
			Keys.Add(gametime, value);
		}

		public void RemoveKey(uint gametime)
		{
			Keys.Remove(gametime);
		}

		public void Clear()
		{
			ActionDatas.Clear();
		}
	}

	[Serializable]
	public class ReplayActionData
	{
		public uint GameTime;
		public ushort DownKeys;

		public ReplayActionData()
		{

		}

		public ReplayActionData(uint gametime)
		{
			GameTime = gametime;
		}

		public ReplayActionData(uint gametime, ushort downKeys)
		{
			GameTime = gametime;
			DownKeys = downKeys;
		}
	}
}
