using System;
using System.IO;
using System.Text;

namespace NagaisoraFramework
{
	using SecuritySystem;
	using System.Linq;

	public class Partition
	{
		public Guid Id { get; private set; }
		public string Name;
		public FileSystemType FileSystemType { get; private set; }

		public bool IsBootPartition { get; private set; }

		public bool IsEncrypt { get; private set; }

		public byte[] Key { get; private set; }

		public ulong[] StartSectors;
		public ulong[] EndSectors;

		public Partition()
		{
			Id = Guid.NewGuid();
		}

		public Partition(string name, FileSystemType fileSystemType, ulong[] startSectors, ulong[] endSectors, Guid guid, bool isBootPartition = false, bool isEncrypt = false, byte[] key = null)
		{
			Id = guid;
			Name = name;
			FileSystemType = fileSystemType;
			IsBootPartition = isBootPartition;
			IsEncrypt = isEncrypt;
			Key = key;
			StartSectors = startSectors;
			EndSectors = endSectors;
		}

		public byte[] ToBinary()
		{
			byte[] bytes = new byte[512];

			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryWriter BW = new BinaryWriter(memoryStream, Encoding.UTF8);
			BW.BaseStream.Seek(0, SeekOrigin.Begin);

			BW.Write(Id.ToByteArray());
			BW.Write(Name);
			BW.Write((int)FileSystemType);
			BW.Write(IsBootPartition);
			BW.Write(IsEncrypt);

			if (StartSectors.Length != EndSectors.Length)
			{
				return null;
			}

			BW.Write((ushort)StartSectors.Length);

			for (int i = 0; i < StartSectors.Length; i++)
			{
				BW.Write(StartSectors[i]);
				BW.Write(EndSectors[i]);
			}

			BW.BaseStream.Seek(512 - 23, SeekOrigin.Begin);
			if (IsEncrypt)
			{
				BW.Write(MD5Cryptography.MD5Encrypt16Byte(Key));
			}
			else
			{
				BW.Write(new byte[16] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
			}

			BW.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
			BW.Write(Encoding.UTF8.GetBytes("END"));

			return memoryStream.ToArray();
		}

		public static Partition FromBinary(byte[] PartitionData)
		{
			MemoryStream memory = new MemoryStream(PartitionData);
			BinaryReader reader = new BinaryReader(memory);

			Guid guid = new Guid(reader.ReadBytes(16));
			string name = reader.ReadString();
			FileSystemType type = (FileSystemType)reader.ReadInt32();
			bool isBootPartition = reader.ReadBoolean();
			bool isEncrypt = reader.ReadBoolean();

			ushort count = reader.ReadUInt16();

			ulong[] startSectors = new ulong[count];
			ulong[] endSectors = new ulong[count];

			for (int i = 0; i < count; i++)
			{
				startSectors[i] = reader.ReadUInt64();
				endSectors[i] = reader.ReadUInt64();
			}

			reader.BaseStream.Seek(512 - 23, SeekOrigin.Begin);
			byte[] keyMD5 = reader.ReadBytes(16);

			Partition partition = new Partition(name, type, startSectors, endSectors, guid, isBootPartition, isEncrypt, keyMD5);
			return partition;
		}

		public new string ToString()
		{
			return "\nRTA分区信息\n" +
				  $"                  分区ID : {BitConverter.ToString(Id.ToByteArray()).Replace('-', ' ')}\n" +
				  $"                    名称 : {Name}\n" +
				  $"                文件系统 : {FileSystemType}\n" +
				  $"                起始扇区 : {StartSectors.Min()}\n" +
				  $"                终止扇区 : {EndSectors.Max()}\n" +
				  $"  是可执行启动引导的分区 : {IsBootPartition}\n" +
				  $"                    加密 : {IsEncrypt}\n";
		}
	}

	public enum FileSystemType
	{
		NONE = 0,
		RAW = 1,
		NTFS = 2,
		FAT16 = 3,
		FAT32 = 4,
		FAT64 = 5,
		NFAT = 6,
	}
}