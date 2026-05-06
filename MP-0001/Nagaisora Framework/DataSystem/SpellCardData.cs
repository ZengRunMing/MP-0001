using System;

namespace NagaisoraFramework.DataSystem
{
	public struct SpellCardData
	{
		public const int BinaryLength = 128;

		public const string DataHead = "NSCD";

		public const uint Version = 0X00000001;

		public string Name
		{
			get
			{
				return m_Name;
			}

			set
			{
				if (Encoding.UTF8.GetBytes(value).Length > 112)
				{
					throw new ArgumentException("[Spell Card] Name is too long");
				}

				m_Name = value;
			}
		}

		public uint SID;
		public uint Score;
		public Guid FunctionGUID;

		private string m_Name;

		public SpellCardData(string name, uint sid, uint score, Guid functionGUID)
		{
			m_Name = string.Empty;

			SID = sid;
			Score = score;
			FunctionGUID = functionGUID;

			Name = name;
		}

		public void GetFunction()
		{
			
		}

		public byte[] ToBinary()
		{
			MemoryStream memory = new(new byte[BinaryLength]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			memory.Position = 0;

			writer.Write(DataHead.ToCharArray());
			writer.Write(Version);

			writer.Write(SID);
			writer.Write(Score);
			
			writer.Write(FunctionGUID.ToByteArray());
			writer.Write(Name.ToCharArray());

			writer.Close();

			return memory.ToArray();
		}

		public static SpellCardData FromBinary(byte[] binary)
		{
			if (binary.Length != BinaryLength)
			{
				throw new InvalidDataException("数据长度不正确");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			memory.Position = 0;

			string ReadHead = new(reader.ReadChars(DataHead.Length));

			if (ReadHead != DataHead)
			{
				throw new InvalidDataException("数据标识头不匹配");
			}

			uint ReadVersion = reader.ReadUInt32();

			if (ReadVersion != Version)
			{
				throw new InvalidDataException("数据发行版本不匹配");
			}

			uint sid = reader.ReadUInt32();
			uint score = reader.ReadUInt32();
			Guid functionGUID = new(reader.ReadBytes(16));

			byte[] stringbuffer = reader.ReadBytes(112);
			string name = StringHelper.GetStringFromBinary(stringbuffer, Encoding.UTF8);

			reader.Close();
			memory.Close();

			return new SpellCardData(name, sid, score, functionGUID);
		}
	}
}
