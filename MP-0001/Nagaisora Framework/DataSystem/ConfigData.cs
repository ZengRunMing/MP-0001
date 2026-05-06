using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

namespace NagaisoraFramework.DataSystem
{
	[Serializable]
	public struct ConfigData
	{
		public const string DataHead = "NCFG";

		public const uint Version = 0x00000001;

		public const uint EngineVersion = 7655135;

		public byte MusicVolume;
		public byte SEVolume;
		public byte BGMode;
		public string OutputDeviceName;
		public uint ResolutionX;
		public uint ResolutionY;
		public byte DrawMode;
		public byte Frame;

		public Dictionary<string, KeyConfig> KeyConfigs;

		public static readonly ConfigData Default = new()
		{
			MusicVolume = 10,
			SEVolume = 8,
			BGMode = 0,
			OutputDeviceName = "Microsoft GS Wavetable Synth",
			ResolutionX = 1440,
			ResolutionY = 1080,
			DrawMode = 0,
			Frame = 2,
			KeyConfigs = new Dictionary<string, KeyConfig>()
			{
				{"default", KeyConfig.Default },
			},
		};

		public readonly byte[] ToBinary()
		{
			MemoryStream memory = new();
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			memory.Position = 0;

			writer.Write(DataHead.ToCharArray());
			writer.Write(Version);
			writer.Write(EngineVersion);

			memory.Position = 16;

			writer.Write(MusicVolume);
			writer.Write(SEVolume);
			writer.Write(BGMode);
			writer.Write(OutputDeviceName);
			writer.Write(ResolutionX);
			writer.Write(ResolutionY);
			writer.Write(DrawMode);
			writer.Write(Frame);

			string[] keys = [.. KeyConfigs.Keys];
			KeyConfig[] values = [.. KeyConfigs.Values];

			writer.Write((uint)KeyConfigs.Count);

			for (uint i = 0; i < KeyConfigs.Count; i++)
			{
				writer.Write(keys[i]);

				byte[] bytes = values[i].ToBinary();
				
				writer.Write((uint)bytes.Length);
				writer.Write(bytes);
			}

			writer.Close();

			return memory.ToArray();
		}

		public static ConfigData FromBinary(byte[] binary)
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

			ConfigData configData = new()
			{
				MusicVolume = reader.ReadByte(),
				SEVolume = reader.ReadByte(),
				BGMode = reader.ReadByte(),
				OutputDeviceName = reader.ReadString(),
				ResolutionX = reader.ReadUInt32(),
				ResolutionY = reader.ReadUInt32(),
				DrawMode = reader.ReadByte(),
				Frame = reader.ReadByte(),
			};

			uint count = reader.ReadUInt32();

			configData.KeyConfigs = [];

			for (uint i = 0; i < count; i++)
			{
				string name = reader.ReadString();

				uint length = reader.ReadUInt32();
				byte[] buffer = reader.ReadBytes((int)length);
				KeyConfig keyConfig = KeyConfig.FromBinary(buffer);

				configData.KeyConfigs.Add(name, keyConfig);
			}

			reader.Close();
			memory.Close();

			return configData;
		}

		public readonly ConfigData Copy()
		{
			byte[] bytes = ToBinary();
			return FromBinary(bytes);
		}

		public static void SaveConfig(Stream stream, ConfigData ConfigData)
		{
			stream.SetLength(0);
			stream.Flush();

			BinaryWriter writer = new(stream, Encoding.UTF8, true);
			
			stream.Position = 0;

			writer.Write(ConfigData.ToBinary());

			writer.Close();
		}

		public static ConfigData LoadConfig(Stream stream)
		{
			BinaryReader reader = new(stream, Encoding.UTF8, true);
			
			stream.Position = 0;

			ConfigData ConfigData = FromBinary(reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position)));

			reader.Close();

			return ConfigData;
		}
	}

}
