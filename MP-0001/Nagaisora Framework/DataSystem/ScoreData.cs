using System;
using System.IO;
using System.Text;

using Godot;

namespace NagaisoraFramework.DataSystem
{
	using SecuritySystem;

	[Serializable]
	public struct ScoreData
	{
		public static ScoreData Default => GetDefault();

		public const string DataHead = "NGSD";

		public const uint Version = 0X00000001;

		public string CreaterName
		{
			get => m_CreaterName;

			set
			{
				if (value.Length > 16)
				{
					return;
				}

				m_CreaterName = value;
			}
		}

		public DateTime CreateTime;
		public TimeSpan TotalRunTime;

		public bool[] BGMUnlockData;

		public PlayerData PlayerData;

		private string m_CreaterName;

		public ScoreData(string createrName, DateTime createTime, TimeSpan totalRunTime, bool[] bgmUnlockData, PlayerData playerData)
		{
			m_CreaterName = string.Empty;

			CreateTime = createTime;
			TotalRunTime = totalRunTime;
			PlayerData = playerData;
			BGMUnlockData = bgmUnlockData;

			CreaterName = createrName;
		}

		public static ScoreData GetDefault()
		{
			bool[] BGMUnlockData = new bool[MainSystem.BGMCount];

			for (int i = 0; i < BGMUnlockData.Length; i++)
			{
				BGMUnlockData[i] = false;

				if (i == 0)
				{
					BGMUnlockData[i] = true;
				}
			}

			return new ScoreData("System", DateTime.Now, TimeSpan.Zero, BGMUnlockData, PlayerData.Default);
		}

		public static ScoreData AllDone()
		{
			bool[] MusicRoomGetData = new bool[MainSystem.BGMCount];

			for (int i = 0; i < MusicRoomGetData.Length; i++)
			{
				MusicRoomGetData[i] = true;
			}

			return new ScoreData("System", DateTime.Now, TimeSpan.Zero, MusicRoomGetData, PlayerData.Default);
		}

		public byte[] ToBinady()
		{
			MemoryStream memory = new();
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			writer.Write(DataHead.ToCharArray());
			writer.Write(Version);

			memory.Position = 16;
			writer.Write(new byte[16]);

			writer.Write(CreaterName.ToCharArray());

			memory.Position = 48;

			writer.Write(CreateTime.ToBinary());
			writer.Write(TotalRunTime.Ticks);

			writer.Write(BGMUnlockData.Length);

			byte[] boolbuffer = BitArrayBool.ToBinary(BGMUnlockData);

			writer.Write(boolbuffer);

			int remain = (int)memory.Position % 16;

			writer.Write(new byte[remain > 0 ? 16 - remain : 0]);

			writer.Write(PlayerData.ToBinary());

			writer.Flush();
			memory.Flush();

			memory.Position = 0;

			byte[] buffer = new byte[memory.Length];
			memory.Read(buffer, 0, buffer.Length);

			byte[] MD5Binary = MD5Cryptography.MD5Encrypt16Byte(buffer);

			memory.Position = 16;
			writer.Write(MD5Binary);

			writer.Close();

			return memory.ToArray();
		}

		public static ScoreData FromBinary(byte[] binary)
		{
			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			memory.Position = 0;

			string ReadHead = new(reader.ReadChars(DataHead.Length));
			if (ReadHead != DataHead)
			{
				throw new InvalidDataException($"数据标识头不匹配");
			}

			uint ReadVersion = reader.ReadUInt32();
			if (ReadVersion != Version)
			{
				throw new InvalidDataException($"数据发行版本不匹配");
			}

			memory.Position = 16;

			Guid ReadMD5 = new(reader.ReadBytes(16));

			memory.Position = 0;

			byte[] bytes = new byte[memory.Length];
			memory.Read(bytes, 0, bytes.Length);

			MemoryStream tempMemory = new(bytes)
			{
				Position = 16
			};

			byte[] mbuffer = new byte[16];
			tempMemory.Write(mbuffer, 0, mbuffer.Length);

			Guid EncryptMD5 = new(MD5Cryptography.MD5Encrypt16Byte(tempMemory.ToArray()));

			tempMemory?.Close();

			if (ReadMD5 != EncryptMD5)
			{
				throw new InvalidDataException("校验不通过");
			}

			memory.Position = 32;

			byte[] stringbuffer = reader.ReadBytes(16);
			string createrName = StringHelper.GetStringFromBinary(stringbuffer, Encoding.ASCII);

			DateTime createTime = DateTime.FromBinary(reader.ReadInt64());
			TimeSpan totalRunTime = new(reader.ReadInt64());

			int bgmCount = reader.ReadInt32();

			int readcount = (int)Math.Ceiling(bgmCount / 8f);
			byte[] boolbuffer = reader.ReadBytes(readcount);

			bool[] bools = BitArrayBool.FromBinary(boolbuffer, bgmCount);

			int remain = (int)reader.BaseStream.Position % 16;

			reader.BaseStream.Seek(remain > 0 ? 16 - remain : 0, SeekOrigin.Current);

			byte[] dataBinary = reader.ReadBytes(PlayerData.BinaryLength);
			PlayerData data = PlayerData.FromBinary(dataBinary);

			return new ScoreData(createrName, createTime, totalRunTime, bools, data);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}

		/// <summary>
		/// 读取Score数据
		/// </summary>
		/// <param name="Path">读取地址，如果为Null或者无字符则在默认地址读取</param>
		/// <returns>返回ScoreData数据</returns>
		public static ScoreData ScoreDataLoad(string Path)
		{
			if (!File.Exists(Path))
			{
				GD.Print($"[Framework Kernel] 不存在玩家数据文件，将创建初始化数据");
				ScoreDataSave(Default, Path);
			}
			else
			{
				GD.Print($"[Framework Kernel] 装载玩家数据");
			}

			return FromBinary(File.ReadAllBytes(Path));
		}

		/// <summary>
		/// 保存Score数据
		/// </summary>
		/// <param name="scoreData">要保存的Score数据</param>
		/// <param name="Path">保存地址，如果为Null或者无字符则保存在默认地址</param>
		public static void ScoreDataSave(ScoreData scoreData, string Path)
		{
			FileStream fileStream = new(Path, FileMode.Create, System.IO.FileAccess.ReadWrite);

			byte[] bytes = scoreData.ToBinady();

			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Close();
		}
	}
}
