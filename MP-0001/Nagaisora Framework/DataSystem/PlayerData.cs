using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NagaisoraFramework.DataSystem
{
	[Serializable]
	public struct PlayerData(TimeSpan totalplayertime, int playerindex, int clearindex, PlayerDataItem[,] playerDatas)
	{
		public static int BinaryLength => 32 + (MainSystem.PlayerCount * MainSystem.RankCount * PlayerDataItem.BinaryLength);

		public static PlayerData Default => GetDefault();

		public const string DataHead = "NPRD";

		public TimeSpan TotalPlayerTime = totalplayertime;

		public int PlayerIndex = playerindex;
		public int ClearIndex = clearindex;

		public PlayerDataItem[,] PlayerDatas = playerDatas;

		public PlayerDataItem this[int player, int rank] => PlayerDatas[player, rank];

		public void SetPlayerDataItem(PlayerDataItem item, int player, int rank)
		{
			PlayerDatas[player, rank] = item;
		}

		public static PlayerData GetDefault()
		{
			PlayerDataItem[,] items = new PlayerDataItem[MainSystem.PlayerCount, MainSystem.RankCount];

			for (int i = 0; i < MainSystem.PlayerCount; i++)
			{
				for (int j = 0; j < MainSystem.RankCount; j++)
				{
					PlayerDataScoreItem[] scoreItems = new PlayerDataScoreItem[10];
					for (int k = 0; k < 10; k++)
					{
						scoreItems[k] = new PlayerDataScoreItem(string.Empty, 0, DateTime.Now, 0, 0f);
					}
					SpellCardGetItem[] spellCardGetItems = new SpellCardGetItem[0];
					items[i, j] = new PlayerDataItem(TimeSpan.Zero, 0, 0, scoreItems, spellCardGetItems);
				}
			}

			return new PlayerData(TimeSpan.Zero, 0, 0, items);
		}

		public byte[] ToBinary()
		{
			MemoryStream memory = new(new byte[BinaryLength]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			int Rank0Length = PlayerDatas.GetLength(0);
			int Rank1Length = PlayerDatas.GetLength(1);

			writer.BaseStream.Position = 0;

			writer.Write(DataHead.ToCharArray());

			writer.BaseStream.Position += 4;

			writer.Write(TotalPlayerTime.Ticks);

			writer.Write(PlayerIndex);
			writer.Write(ClearIndex);

			writer.Write(Rank0Length);
			writer.Write(Rank1Length);

			if (PlayerDatas != null)
			{
				for (int i0 = 0; i0 < Rank0Length; i0++)
				{
					for (int i1 = 0; i1 < Rank1Length; i1++)
					{
						byte[] binary = this[i0, i1].ToBinary();

						writer.Write(binary);
					}
				}
			}

			writer.Close();

			return memory.ToArray();
		}

		public static PlayerData FromBinary(byte[] binary)
		{
			if (binary.Length != BinaryLength)
			{
				throw new InvalidDataException($"数据长度不正确，期望长度：{BinaryLength}，实际长度：{binary.Length}");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory);

			string ReadHead = new(reader.ReadChars(DataHead.Length));
			if (ReadHead != DataHead)
			{
				throw new InvalidDataException($"数据标识头不匹配");
			}

			reader.BaseStream.Position += 4;

			TimeSpan totalplayertime = TimeSpan.FromTicks(reader.ReadInt64());

			int playerindex = reader.ReadInt32();
			int clearindex = reader.ReadInt32();

			int Rank0Length = reader.ReadInt32();
			int Rank1Length = reader.ReadInt32();
			
			PlayerDataItem[,] playerDatas = new PlayerDataItem[Rank0Length, Rank1Length];

			for (int i0 = 0; i0 < Rank0Length; i0++)
			{
				for (int i1 = 0; i1< Rank1Length; i1++)
				{
					byte[] dataBinary = reader.ReadBytes(PlayerDataItem.BinaryLength);

					playerDatas[i0, i1] = PlayerDataItem.FromBinary(dataBinary);
				}
			}

			reader.Close();
			memory.Close();

			return new PlayerData(totalplayertime, playerindex, clearindex, playerDatas);
		}

		public override bool Equals(object obj) => base.Equals(obj);

		public override int GetHashCode() => base.GetHashCode();

		public override string ToString() => base.ToString();
	}

	[Serializable]
	public struct PlayerDataItem(TimeSpan totalPlayedTime, uint playedCount, uint clearanceCount, PlayerDataScoreItem[] scoreItems, SpellCardGetItem[] spellCardGetItems)
	{
		public const int BinaryLength = 1024;

		public TimeSpan TotalPlayedTime = totalPlayedTime;
		
		public uint PlayedCount = playedCount;
		public uint ClearanceCount = clearanceCount;

		public PlayerDataScoreItem[] ScoreItems = scoreItems;
		public SpellCardGetItem[] SpellCardGetItems = spellCardGetItems;

		public byte[] ToBinary()
		{
			MemoryStream memory = new(new byte[BinaryLength]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			writer.Write(TotalPlayedTime.Ticks);
			writer.Write(PlayedCount);
			writer.Write(ClearanceCount);

			foreach (PlayerDataScoreItem item in ScoreItems)
			{
				writer.Write(item.ToBinary());
			}

			foreach (SpellCardGetItem item in SpellCardGetItems)
			{
				writer.Write(item.ToBinary());
			}

			if (memory.Position <= BinaryLength - 8)
			{
				writer.Write((long)-1);
			}

			writer.Close();

			return memory.ToArray();
		}

		public static PlayerDataItem FromBinary(byte[] binary)
		{
			if (binary.Length != BinaryLength)
			{
				throw new InvalidDataException("数据长度不正确");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			TimeSpan totalPlayedTime = TimeSpan.FromTicks(reader.ReadInt64());

			uint playedCount = reader.ReadUInt32();
			uint clearanceCount = reader.ReadUInt32();

			PlayerDataScoreItem[] scoreItems = new PlayerDataScoreItem[10];
			for (int i = 0; i < 10; i++)
			{
				byte[] buffer = reader.ReadBytes(PlayerDataScoreItem.BinaryLength);
				scoreItems[i] = PlayerDataScoreItem.FromBinary(buffer);
			}

			List<SpellCardGetItem> spellCardGetItems = [];
			while(memory.Position < memory.Length)
			{
				byte[] buffer = reader.ReadBytes(SpellCardGetItem.BinaryLength);

				if (BitConverter.ToInt64(buffer, 0) == -1)
				{
					break;
				}

				spellCardGetItems.Add(SpellCardGetItem.FromBinary(buffer));
			}

			reader.Close();
			memory.Close();

			return new PlayerDataItem(totalPlayedTime, playedCount, clearanceCount, scoreItems, [.. spellCardGetItems]);
		}
	}

	[Serializable]
	public struct PlayerDataScoreItem
	{
		public const int BinaryLength = 32;

		public string Name
		{
			get => m_Name;
			set
			{
				if (value.Length > 8)
				{
					return;
				}

				m_Name = value;
			}
		}

		public uint Score;
		public DateTime Time;
		public uint State;
		public float CollectionRate;

		private string m_Name;

		public PlayerDataScoreItem(string name, uint score, DateTime time, uint state, float collectionRate)
		{
			m_Name = string.Empty;

			Score = score;
			Time = time;
			State = state;

			CollectionRate = collectionRate;

			Name = name;
		}

		public byte[] ToBinary()
		{
			MemoryStream memory = new(new byte[BinaryLength]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			writer.Write(Name.ToCharArray());

			memory.Position = 8;

			writer.Write(Time.ToBinary());

			writer.Write(Score);
			writer.Write(State);
			writer.Write(CollectionRate);

			writer.Close();

			return memory.ToArray();
		}

		public static PlayerDataScoreItem FromBinary(byte[] binary)
		{
			if (binary.Length != BinaryLength)
			{
				throw new InvalidDataException("数据长度不正确");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			byte[] stringbuffer = reader.ReadBytes(8);
			string name = StringHelper.GetStringFromBinary(stringbuffer, Encoding.ASCII);

			DateTime time = DateTime.FromBinary(reader.ReadInt64());

			uint score = reader.ReadUInt32();
			uint state = reader.ReadUInt32();
			uint collectionRate = reader.ReadUInt32();

			reader.Close();
			memory.Close();

			return new PlayerDataScoreItem(name, score, time, state, collectionRate);
		}
	}

	[Serializable]
	public struct SpellCardGetItem
	{
		public const int BinaryLength = 8;

		public uint Index;
		public ushort LoadCount;
		public ushort GetCount;

		public SpellCardGetItem(uint index, ushort loadCount, ushort getCount)
		{
			Index = index;
			LoadCount = loadCount;
			GetCount = getCount;
		}

		public byte[] ToBinary()
		{
			MemoryStream memory = new(new byte[BinaryLength]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);
			
			writer.Write(Index);
			writer.Write(LoadCount);
			writer.Write(GetCount);

			writer.Close();

			return memory.ToArray();
		}

		public static SpellCardGetItem FromBinary(byte[] binary)
		{
			if (binary.Length != BinaryLength)
			{
				throw new InvalidDataException("数据长度不正确");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			uint index = reader.ReadUInt32();
			ushort loadCount = reader.ReadUInt16();
			ushort clearCount = reader.ReadUInt16();

			reader.Close();
			memory.Close();

			return new SpellCardGetItem(index, loadCount, clearCount);
		}
	}

}
