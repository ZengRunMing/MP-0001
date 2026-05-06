using System;
using System.IO;
using System.Text;
using System.Collections;
using System.IO.MemoryMappedFiles;

// NFAT文件系统
// 作者 : 万影 星梦
// 创建时间 : 2023/06/??
// 最后修改 : 2026/03/??
namespace NagaisoraFramework
{
	using System.Collections.Generic;
	using System.Linq;
	using Underlyingsystem;

	public abstract class INFATBlockObject
	{
		public abstract long BlockAddress { get; set; } 

		public abstract void ToStream(NFATtream stream, long blockAddress = -1);
	}

	public abstract class ILinkBlock : INFATBlockObject
	{
		public abstract long NextBlockAddress { get; set; }
	}

	public class NFATInfo(ulong startBlock, ulong endBlock, ulong blockSize, ulong dataLength, byte[] saveID, DateTime createTime, DateTime saveTime, long saveIndex, bool encrypted, bool canList, bool canExport, bool canSet) : IDiskInfo
	{
		public long BlockAddress { get; set; }

		public override ulong StartBlock { get; } = startBlock;
		public override ulong EndBlock { get; } = endBlock;
		public override ulong BlockSize { get; } = blockSize;
		public override ulong DataLength { get; } = dataLength;
		public override byte[] SaveID { get; } = saveID;

		public DateTime CreateTime = createTime;
		public DateTime SaveTime = saveTime;
		public long SaveIndex = saveIndex;
		public bool Encrypted = encrypted;
		public bool CanList = canList;
		public bool CanExport = canExport;
		public bool CanSet = canSet;

		public static NFATInfo FromStream(byte[] vs)
		{
			MemoryStream memoryStream = new(vs);
			BinaryReader br = new(memoryStream);

			br.BaseStream.Position = 4;

			return new NFATInfo(br.ReadUInt64(), br.ReadUInt64(), br.ReadUInt64(), br.ReadUInt64(), br.ReadBytes(16), DateTime.FromBinary(br.ReadInt64()),
						DateTime.FromBinary(br.ReadInt64()), br.ReadInt64(), br.ReadBoolean(), br.ReadBoolean(), br.ReadBoolean(), br.ReadBoolean());
		}

		public void ToStream(NFATtream stream, long blockAddress = 0)
		{
			MemoryStream memory = new(1024);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			writer.Write("NFAT".ToCharArray());

			writer.Write(StartBlock);
			writer.Write(EndBlock);
			writer.Write(BlockSize);

			writer.Write(DataLength);

			writer.Write(SaveID);

			writer.Write(CreateTime.ToBinary());
			writer.Write(SaveTime.ToBinary());

			writer.Write(SaveIndex);

			writer.Write(Encrypted);
			writer.Write(CanList);
			writer.Write(CanExport);
			writer.Write(CanSet);

			writer.Close();

			stream.WriteBlock(blockAddress, memory.ToArray());
		}
	}

	// 存储分配信息块
	public class StorageInformationBlock : INFATBlockObject
	{
		public override long BlockAddress { get; set; }

		protected List<StorageInformation> StorageInformations;

		public int Count => StorageInformations.Count;

		public StorageInformationBlock(StorageInformation[] storageInformations)
		{
			StorageInformations = [.. storageInformations];
		}

		public StorageInformationBlock()
		{
			StorageInformations = [];
		}

		public StorageInformation[] GetAllInformation()
		{
			return [.. StorageInformations];
		}

		public void AddInformation(StorageInformation information)
		{
			if (Count >= 32)
			{
				string message = "该块存储信息数量已达到上限，无法添加更多的存储分配信息";
				throw new ArgumentOutOfRangeException(message);
			}

			StorageInformations.Add(information);
		}

		public void RemoveInformation(StorageInformation information)
		{
			StorageInformations.Remove(information);
		}

		public override void ToStream(NFATtream stream, long blockAddress = -1)
		{
			MemoryStream memoryStream = new(1024);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			Queue<StorageInformation> informations = new(StorageInformations);

			while (binaryWriter.BaseStream.Position < 1024)
			{
				if (Count <= 0)
				{
					binaryWriter.Write((long)- 1);
					break;
				}

				binaryWriter.Write(informations.Dequeue().ToBinary());
			}

			binaryWriter.Close();

			if (BlockAddress < 0 || blockAddress >= 0)
			{
				BlockAddress = blockAddress;
			}

			if (BlockAddress < 0)
			{
				BlockAddress = stream.AllocateBlock();
			}

			stream.WriteBlock(BlockAddress, memoryStream.ToArray());
		}

		public static StorageInformationBlock FromStream(NFATtream stream, long blockAddress)
		{
			byte[] binary = stream.ReadBlock(blockAddress);

			MemoryStream memoryStream = new(binary);
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);

			binaryReader.BaseStream.Position = 0;

			StorageInformationBlock block = new();

			while (binaryReader.BaseStream.Position < 1024)
			{
				if (binaryReader.ReadInt64() == -1)
				{
					break;
				}

				binaryReader.BaseStream.Position -= 8;

				StorageInformation info = StorageInformation.FromBinary(binaryReader.ReadBytes(32));

				if (info == null)
				{
					break;
				}

				block.AddInformation(info);
			}

			binaryReader.Close();

			return block;
		}
	}

	public class AddressBlock : INFATBlockObject, IEnumerable
	{
		public override long BlockAddress { get; set; }
		
		protected List<long> Addresses;

		public long this[int index] => Addresses[index];

		public int Count => Addresses.Count;

		public AddressBlock(long[] addresses)
		{
			Addresses = [.. addresses];
		}

		public AddressBlock()
		{
			Addresses = [];
		}

		public void PushAddresses(params long[] addresses)
		{
			if ((addresses.Length + Count) > 128)
			{
				throw new ArgumentException("一个块只能存储128个地址映射信息");
			}

			Addresses.AddRange(addresses);
		}

		public void RemoveAddresses(params long[] addresses)
		{
			foreach (long address in addresses)
			{
				Addresses.Remove(address);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return Addresses.GetEnumerator();
		}

		public long[] ToArray() => [.. Addresses];

		public List<long> ToList() => [.. Addresses];

		public override void ToStream(NFATtream stream, long blockAddress = -1)
		{
			MemoryStream memoryStream = new(1024);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			Queue<long> AddressesQueue = new(Addresses);

			while (binaryWriter.BaseStream.Position < 1024)
			{
				if (AddressesQueue.Count <= 0)
				{
					binaryWriter.Write((long)-1);
					break;
				}
				binaryWriter.Write(AddressesQueue.Dequeue());
			}

			binaryWriter.Close();

			if (BlockAddress < 0 || blockAddress >= 0)
			{
				BlockAddress = blockAddress;
			}

			if (BlockAddress < 0)
			{
				BlockAddress = stream.AllocateBlock();
			}

			stream.WriteBlock(BlockAddress, memoryStream.ToArray());
		}

		public static AddressBlock FromStream(NFATtream stream, long blockAddress)
		{
			byte[] binary = stream.ReadBlock(blockAddress);

			MemoryStream memoryStream = new(binary);
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);

			AddressBlock block = new();

			while (binaryReader.BaseStream.Position < 1024)
			{
				long address = binaryReader.ReadInt64();
				if (address == -1)
				{
					break;
				}

				block.PushAddresses(address);
			}

			return block;
		}
	}

	public class StorageAddressBlock : ILinkBlock
	{
		public override long BlockAddress { get; set; }

		public override long NextBlockAddress { get; set; } = -1;

		protected List<long> BlockAddresses;

		public int Count => BlockAddresses.Count;

		public long this[int index] => BlockAddresses[index];

		public StorageAddressBlock()
		{
			BlockAddresses = [];
		}

		public StorageAddressBlock(params long[] addresses)
		{
			if (addresses.Length > 127)
			{
				throw new ArgumentException("地址数量不能超过127个");
			}

			BlockAddresses = [.. addresses];
		}

		public void PushAddresses(params long[] addresses)
		{
			if ((Count + addresses.Length) > 127)
			{
				throw new ArgumentException("地址数量不能超过127个");
			}

			BlockAddresses.AddRange(addresses);
		}

		public bool RemoveAddress(long address)
		{
			return BlockAddresses.Remove(address);
		}

		public void RemoveAddresses(params long[] addresses)
		{
			foreach (long address in addresses)
			{
				BlockAddresses.Remove(address);
			}
		}

		public override void ToStream(NFATtream stream, long blockAddress = 2)
		{
			MemoryStream memoryStream = new(1024);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			binaryWriter.Write(NextBlockAddress);

			foreach (long address in BlockAddresses)
			{
				binaryWriter.Write(address);
			}

			if (binaryWriter.BaseStream.Position < 1024)
			{
				binaryWriter.Write((long)-1);
			}

			binaryWriter.Close();

			if (BlockAddress < 0 || blockAddress >= 0)
			{
				BlockAddress = blockAddress;
			}

			if (BlockAddress < 0)
			{
				BlockAddress = stream.AllocateBlock();
			}

			stream.WriteBlock(BlockAddress, memoryStream.ToArray());
		}

		public static StorageAddressBlock FromStream(NFATtream stream, long blockAddress = 2)
		{
			byte[] binary = stream.ReadBlock(blockAddress);
			MemoryStream memoryStream = new(binary);
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);

			StorageAddressBlock block = new()
			{
				NextBlockAddress = binaryReader.ReadInt64()
			};

			while (binaryReader.BaseStream.Position < 1024)
			{
				long address = binaryReader.ReadInt64();

				if (address == -1)
				{
					break;
				}

				block.BlockAddresses.Add(address);
			}
			
			return block;
		}
	}

	public enum StorageInformationType
	{
		Single,
		Continuous,
	}

	// 存储分配信息
	public class StorageInformation(StorageInformationType storageInformationType, long blockAddress, long length)
	{
		public const string DataHeader = "NFAT FAD";

		public StorageInformationType StorageInformationType = storageInformationType;

		public long BlockAddress { get; set; } = blockAddress;

		public long Length { get; set; } = length;

		public byte[] ToBinary()
		{
			MemoryStream memoryStream = new(32);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			binaryWriter.Write(DataHeader.ToCharArray());
			binaryWriter.Write((byte)StorageInformationType);

			binaryWriter.BaseStream.Position += 7;

			binaryWriter.Write(BlockAddress);
			binaryWriter.Write(Length);

			binaryWriter.Close();

			return memoryStream.ToArray();
		}

		public static StorageInformation FromBinary(byte[] binary)
		{
			if (binary.Length != 32)
			{
				throw new ArgumentException("数据的长度必须是32位");
			}

			MemoryStream memoryStream = new(binary);
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);

			string Hander = new(binaryReader.ReadChars(DataHeader.Length));

			if (Hander != DataHeader)
			{
				return null;
			}

			StorageInformationType type = (StorageInformationType)Enum.ToObject(typeof(StorageInformationType), binaryReader.ReadByte());

			binaryReader.BaseStream.Position += 7;

			long BlcokIndex = binaryReader.ReadInt64();
			long Length = binaryReader.ReadInt64();

			binaryReader.Close();

			return new StorageInformation(type, BlcokIndex, Length);
		}
	}

	// 成组链接空闲块信息
	public class IdleBlockInformation : ILinkBlock
	{
		public override long BlockAddress { get; set; }

		public int IdleBlockCount => IdleBlocks.Count;

		public Stack<long> IdleBlocks { get; protected set; }

		public bool IsLast { get; protected set; }

		public override long NextBlockAddress
		{
			get
			{
				return m_NextBlockAddress;
			}

			set
			{
				m_NextBlockAddress = value;

				if (value < 0)
				{
					IsLast = true;
				}
				else
				{
					IsLast = false;
				}
			}
		}

		private long m_NextBlockAddress;

		public IdleBlockInformation(long nextBlock = -1)
		{
			IdleBlocks = new Stack<long>(127);

			NextBlockAddress = nextBlock;
		}

		public void PushIdleBlock(long blockAddress, NFATtream stream)
		{
			// 如果空闲块数量未达到上限，则将当前信息压入栈中
			if (IdleBlockCount < 127)
			{
				IdleBlocks.Push(blockAddress);
				ToStream(stream);
				return;
			}

			// 如果空闲块数量达到上限，则将当前信息写入新加入的空闲块中，并新建空闲块信息到根信息
			ToStream(stream, blockAddress);  // 将当前信息写入指定块

			stream.RootIdleInformation = new IdleBlockInformation(blockAddress)
			{
				NextBlockAddress = blockAddress // 设定下一个块索引
			};// 新建空闲块信息对象

			stream.RootIdleInformation.ToStream(stream);
		}

		public long PopIdleBlock(NFATtream stream)
		{
			// 如果还有空闲块信息，直接弹出栈顶信息
			if (IdleBlockCount > 0)
			{
				long IdleBlock = IdleBlocks.Pop();
				ToStream(stream);
				return IdleBlock;
			}
			// 如果当前链空闲块信息耗尽

			//判断当前是否为链表最后一个
			if (IsLast)
			{
				return -1;
			}

			IdleBlockInformation info = FromStream(stream, NextBlockAddress);		// 读取指定块的二进制数据解析为空闲块信息对象

			stream.RootIdleInformation = info;                                      //设定根信息
			stream.RootIdleInformation.ToStream(stream);

			return NextBlockAddress;
		}
		
		/// <summary>
		/// 将信息转化为二进制数据
		/// </summary>
		/// <returns>返回1024字节的二进制数据</returns>
		public override void ToStream(NFATtream stream, long blockAddress = 1)
		{
			MemoryStream memoryStream = new(new byte[1024]);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			binaryWriter.Write(NextBlockAddress);

			long[] blocks = [.. IdleBlocks];				// 获取空闲块索引数组
			for (int i = blocks.Length - 1; i>= 0; i--)		// 倒序写入每个空闲块索引
			{
				binaryWriter.Write(blocks[i]);				// 8字节空闲块索引
			}

			if (binaryWriter.BaseStream.Position < 1024)
			{
				binaryWriter.Write((long)-1);
			}

			binaryWriter.Close();							// 关闭二进制写入器

			stream.WriteBlock(blockAddress, memoryStream.ToArray());  // 将数据写入指定块
		}

		public static IdleBlockInformation FromStream(NFATtream stream, long blockAddress = 1)
		{
			byte[] binary = stream.ReadBlock(blockAddress);   // 读取指定块的二进制数据

			// 新建空闲块信息对象
			IdleBlockInformation infomation = new();

			MemoryStream memoryStream = new(binary);                       // 新建内存流
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);  // 新建二进制读取器

			infomation.NextBlockAddress = binaryReader.ReadInt64();

			// 读取每个空闲块索引并压入栈中
			for (int i = 0; i < 127; i++)
			{
				long blk = binaryReader.ReadInt64();

				if (blk < 0)
				{
					break;
				}

				infomation.IdleBlocks.Push(blk);	// 8字节空闲块索引
			}

			binaryReader.Close();

			return infomation; // 返回空闲块信息对象
		}
	}

	// NFAT对象接口
	public abstract class INFATObject : INFATBlockObject, DiskSystemObject
	{
		public override long BlockAddress { get; set; } = -1;

		public const string DataHeader = "NFAT OBJ";

		public string Name
		{
			get
			{
				return m_Name;
			}

			set
			{
				if (value.Length <= 200)
				{
					m_Name = value;
				}
			}
		}

		public string Path
					{
			get
			{
				if (Parent is null)
				{
					return Name;
				}
				return System.IO.Path.Combine(Parent.Path, Name);
			}
		}

		public Guid ID { get; protected set; }

		public DateTime CreateTime { get; protected set; }
		public DateTime UpdateTime { get; protected set; }

		public NFATFolder Parent { get; protected set; }

		public Guid ParentID { get; protected set; }

		private string m_Name;

		public void NewUpdateTime()
		{
			UpdateTime = DateTime.Now;

			if (Parent is null)
			{
				return;
			}

			Parent.NewUpdateTime();
		}

		public void SetParent(NFATFolder parent, bool updateTime)
		{
			Parent = parent;

			Parent.AddToParent(this);

			if (updateTime)
			{
				Parent.NewUpdateTime();
			}
		}

		public void SetParent(Guid parentGUID, bool updateTime)
		{

		}

		public override abstract void ToStream(NFATtream stream, long blockAddress);

		public static INFATObject FromStream(NFATtream stream, long blockAddress)
		{
			byte[] binary = stream.ReadBlock(blockAddress);

			MemoryStream memoryStream = new(binary);
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);

			string header = new(binaryReader.ReadChars(8));

			if (header != DataHeader)
			{
				throw new InvalidDataException("数据标识不符");
			}

			byte type = binaryReader.ReadByte();

			INFATObject @object = type switch
			{
				0 => NFATFile.FromStream(binaryReader, stream),
				1 => NFATFolder.FromStream(binaryReader),
				_ => throw new InvalidDataException("未知的数据类型"),
			};
			
			@object.BlockAddress = blockAddress;

			binaryReader.Close();

			return @object;
		}

		public long[] GetAllocateBlocks(NFATtream stream)
		{
			List<long> AllocateBlocks = [];

			if (this is NFATFile)
			{
				byte[] binary = stream.ReadBlock(BlockAddress);

				MemoryStream memoryStream = new(binary);
				BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);

				binaryReader.BaseStream.Position = 512;
				
				while (binaryReader.BaseStream.Position < 1024)
				{
					long blockAddress = binaryReader.ReadInt64();
					
					if (blockAddress == -1)
					{
						break;
					}

					AllocateBlocks.AddRange(GetBlockAddresses(stream, GetStorageBlockObject(stream, blockAddress)));
				}
			}

			AllocateBlocks.Add(BlockAddress);

			return [.. AllocateBlocks];
		}

		public static long[] GetBlockAddresses(NFATtream stream, INFATBlockObject block)
		{
			List<long> AllocateBlocks = [];

			if (block is AddressBlock)
			{
				AddressBlock addressBlock = block as AddressBlock;
				foreach (long address in addressBlock)
				{
					AllocateBlocks.AddRange(GetBlockAddresses(stream, GetStorageBlockObject(stream, address)));
				}
			}

			AllocateBlocks.Add(block.BlockAddress);

			return [.. AllocateBlocks];
		}

		public static INFATBlockObject GetStorageBlockObject(NFATtream stream, long blockAddress)
		{
			byte[] headBinary = [.. stream.ReadBlock(blockAddress).Take(8)];

			INFATBlockObject @object;
			if (Encoding.UTF8.GetString(headBinary) == StorageInformation.DataHeader)
			{
				@object = StorageInformationBlock.FromStream(stream, blockAddress);
			}
			else
			{
				@object = AddressBlock.FromStream(stream, blockAddress);
			}

			return @object;
		}
	}

	// NFAT文件对象
	public class NFATFile : INFATObject
	{
		public StorageInformation[] StorageInformations { get; set; }

		public long Size
		{
			get
			{
				long size = 0;

				StorageInformation[] storageInformations = StorageInformations;

				foreach (StorageInformation storageInformation in storageInformations)
				{
					if (storageInformation == null)
					{
						continue;
					}

					if (storageInformation.StorageInformationType == StorageInformationType.Single)
					{
						size += storageInformation.Length;
					}
					else if (storageInformation.StorageInformationType == StorageInformationType.Continuous)
					{
						long length = NFATtream.BlockSize * storageInformation.Length;

						size += length;
					}
				}

				return size;
			}
		}

		public string SizeText => StringHelper.GetFileSizeString((ulong)Size);

		public NFATFile(string name, StorageInformation[] storageInformations) : this(name, Guid.NewGuid(), storageInformations)
		{

		}

		public NFATFile(string name, Guid id, StorageInformation[] storageInformations) : this(name, id, DateTime.Now, DateTime.Now, storageInformations)
		{

		}

		public NFATFile(string name, Guid id, DateTime createTime, DateTime updateTime, StorageInformation[] storageInformations)
		{
			Name = name;

			ID = id;

			CreateTime = createTime;
			UpdateTime = updateTime;

			StorageInformations = storageInformations;
		}

		public override void ToStream(NFATtream stream, long blockAddress = -1)
		{
			MemoryStream memoryStream = new(1024);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			binaryWriter.Write(DataHeader.ToCharArray());												// 写入数据标识头

			binaryWriter.Write((byte)0);																// 写入文件类型标识
			binaryWriter.Write((ushort)Name.Length);													// 写入名称长度

			binaryWriter.BaseStream.Position = 16;														// 定位到第16字节处写入文件基本信息

			binaryWriter.Write(CreateTime.ToFileTime());												// 写入创建时间
			binaryWriter.Write(UpdateTime.ToFileTime());												// 写入修改时间
			binaryWriter.Write(ID.ToByteArray());														// 写入文件ID
			binaryWriter.Write(Parent.ID.ToByteArray());												// 写入父级ID
			binaryWriter.Write(Name.ToCharArray());														// 写入文件名
			
			binaryWriter.BaseStream.Position = 320;                                                     // 定位到当前块第一扇区第320字节处写入内置存储信息

			Queue<StorageInformation> informations = new(StorageInformations);							// 创建存储分配信息队列

			while (binaryWriter.BaseStream.Position < 512)												// 如果还有存储空间，继续写入
			{
				if (informations.Count <= 0)															// 如果存储信息写完，写入结束标志
				{
					binaryWriter.Write((long)-1);														// 64位长整型-1作为结束标志
					break;																				// 跳出循环
				}

				StorageInformation information = informations.Dequeue();								// 弹出队列存储信息

				binaryWriter.Write(information.ToBinary());												// 写入存储信息的二进制数据
			}

			binaryWriter.BaseStream.Position = 512;														// 定位到当前块第二扇区写入外置存储信息

			long blockcount = informations.Count / 32;													// 计算需要的存储信息块数量
			long remain = informations.Count % 32;														// 计算是否有余量

			long totalBlocks = blockcount + (remain <= 0 ? 0 : 1);										// 根据是否有余量决定是否需要额外的存储信息块

			int i = 0;                                                                                  // 设定索引

			StorageInformationBlock[] blocks = new StorageInformationBlock[totalBlocks];                // 新建存储信息块数组

			while (i < totalBlocks)                                                                     // 循环建立存储信息块
			{
				StorageInformationBlock block = new();													// 新建存储信息块

				while (informations.Count > 0 && block.Count < 32)                                      // 如果还有存储信息，且当前块未满，继续添加存储信息
				{
					block.AddInformation(informations.Dequeue());                                       // 添加存储信息
				}

				blocks[i] = block;                                                                      // 将存储信息块添加到数组中

				i++;                                                                                    // 块索引加一
			}

			i = 0;                                                                                      // 设定索引

			// TODO: 处理大于64块的情况
			// 后512字节最多支持64块存储信息块

			// 如果需要的存储信息块数量大于64，进行额外建立地址映射表处理
			if (blocks.Length > 64)
			{
				List<INFATBlockObject> iblocks = [.. blocks];											// 新建块对象列表

				ReAllocate:
				long AddressBlockCount = iblocks.Count / 128;
				long AddressBlockRemain = iblocks.Count % 128;

				long totalAddressBlocks = AddressBlockCount + (AddressBlockRemain <= 0 ? 0 : 1);        // 根据是否有余量决定是否需要额外的地址映射块

				Queue<INFATBlockObject> blockqueue = new(iblocks);										// 创建队列

				AddressBlock[] addressBlocks = new AddressBlock[totalAddressBlocks];					// 创建地址映射块数组

				while (i < totalAddressBlocks)															// 循环新建地址映射块
				{
					AddressBlock addressBlock = new();													// 新建地址映射块

					while (blockqueue.Count > 0 && addressBlock.Count < 128)                            // 如果还有存储信息块，且当前地址映射块未满，继续添加存储信息块地址
					{
						INFATBlockObject @object = blockqueue.Dequeue();                                // 弹出队列存储信息块

						@object.ToStream(stream);														// 将存储信息块写入指定块

						addressBlock.PushAddresses(@object.BlockAddress);								// 将地址压入地址映射块中
					}

					addressBlocks[i] = addressBlock;                                                    // 将地址映射块添加到数组中
					i++;																				// 索引加一
				}

				// 如果一轮分配后还大于64个，则将块进行下一轮地址映射分配
				if (addressBlocks.Length > 64)
				{
					iblocks = [.. addressBlocks];														// 设定要分配的块
					i = 0;                                                                              // 重置索引

					goto ReAllocate;                                                                    // 跳转到重新分配
				}
			}
			else
			{
				while (binaryWriter.BaseStream.Position < 1024)											// 如果还有存储空间，继续写入
				{
					if (blocks.Length <= 0)																// 如果存储信息块写完，写入结束标志
					{
						binaryWriter.Write((long)-1);													// 64位长整型-1作为结束标志
						break;																			// 跳出循环
					}

					blocks[i].ToStream(stream);															// 将存储信息块写入指定块

					binaryWriter.Write(blocks[i].BlockAddress);											// 写入存储信息块的地址

					i++;                                                                                // 块索引加一
				}
			}

			binaryWriter.Close();                                                                       // 关闭二进制写入器

			if (BlockAddress < 0 || blockAddress >= 0)
			{
				BlockAddress = blockAddress;
			}

			if (BlockAddress < 0)																		// 如果当前对象块索引为-1，分配一个新块索引
			{
				BlockAddress = stream.AllocateBlock();                                                  // 分配一个块索引
			}

			stream.WriteBlock(BlockAddress, memoryStream.ToArray());									// 将文件对象数据写入指定块

			return;
		}

		public static NFATFile FromStream(BinaryReader binaryReader, NFATtream stream)
		{
			ushort NameLength = binaryReader.ReadUInt16();

			binaryReader.BaseStream.Position = 16;

			DateTime CreateTime = DateTime.FromFileTime(binaryReader.ReadInt64());
			DateTime UpdateTime = DateTime.FromFileTime(binaryReader.ReadInt64());
			Guid ID = new(binaryReader.ReadBytes(16));
			Guid ParentID = new(binaryReader.ReadBytes(16));
			string Name = new(binaryReader.ReadChars(NameLength));

			List<StorageInformation> storageInformations = [];

			binaryReader.BaseStream.Position = 320;

			while (binaryReader.BaseStream.Position < 512)
			{
				if (binaryReader.ReadInt64() == -1)
				{
					break;
				}

				binaryReader.BaseStream.Position -= 8;

				storageInformations.Add(StorageInformation.FromBinary(binaryReader.ReadBytes(32)));
			}

			binaryReader.BaseStream.Position = 512;

			List<long> blockAddresses = [];

			while (binaryReader.BaseStream.Position < 1024)
			{
				long blockAddress = binaryReader.ReadInt64();

				if (blockAddress == -1)
				{
					break;
				}

				blockAddresses.Add(blockAddress);
			}

			storageInformations.AddRange(GetExtendStorageInformation([.. blockAddresses], stream));

			NFATFile file = new(Name, ID, CreateTime, UpdateTime, [.. storageInformations])
			{
				ParentID = ParentID
			};

			return file;
		}

		public static StorageInformation[] GetExtendStorageInformation(long[] blockAddresses, NFATtream stream)
		{
			List<StorageInformation> Informations = [];

			foreach (long blockAddress in blockAddresses)
			{
				StorageInformationBlock block = StorageInformationBlock.FromStream(stream, blockAddress);

				if (block == null)
				{
					AddressBlock addressBlock = AddressBlock.FromStream(stream, blockAddress);

					Informations.AddRange(GetExtendStorageInformation(addressBlock.ToArray(), stream));
					continue;
				}

				Informations.AddRange(block.GetAllInformation());
			}

			return [.. Informations];
		}
	}

	// NFAT文件夹
	public class NFATFolder : INFATObject
	{
		public long SubItemCount
		{
			get
			{
				if (SubItems is null)
				{
					return 0;
				}

				return SubItems.Count;
			}
		}

		public List<INFATObject> SubItems { get; set; }

		public NFATFolder(string name, INFATObject[] files = null) : this(name, Guid.NewGuid(), DateTime.Now, DateTime.Now, files)
		{

		}

		public NFATFolder(string name, Guid id, INFATObject[] files = null) : this(name, id, DateTime.Now, DateTime.Now, files)
		{

		}

		public NFATFolder(string name, DateTime createtime, DateTime updatetime, INFATObject[] files = null) : this(name, Guid.NewGuid(), createtime, updatetime, files)
		{

		}

		public NFATFolder(string name, Guid id, DateTime createtime, DateTime updatetime, INFATObject[] files = null)
		{
			Name = name;

			ID = id;

			CreateTime = createtime;
			UpdateTime = updatetime;

			if (files is null)
			{
				SubItems = [];
				return;
			}

			SubItems = [.. files];
		}

		public override void ToStream(NFATtream stream, long blockAddress = -1)
		{
			MemoryStream memoryStream = new(1024);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			binaryWriter.Write(DataHeader.ToCharArray());                                               // 写入数据标识头

			binaryWriter.Write((byte)1);                                                                // 写入文件夹类型标识
			binaryWriter.Write((ushort)Name.Length);                                                    // 写入名称长度

			binaryWriter.BaseStream.Position = 16;                                                      // 定位到第16字节处写入文件夹基本信息

			binaryWriter.Write(CreateTime.ToFileTime());                                                // 写入创建时间
			binaryWriter.Write(UpdateTime.ToFileTime());                                                // 写入修改时间
			binaryWriter.Write(ID.ToByteArray());                                                       // 写入文件夹ID
			binaryWriter.Write(Parent.ID.ToByteArray());                                                // 写入父级ID
			binaryWriter.Write(Name.ToCharArray());                                                     // 写入文件夹名

			binaryWriter.Close();                                                                       // 关闭二进制写入器

			if (BlockAddress == -1)                                                                     // 如果当前对象块索引为-1，分配一个新块索引
			{
				BlockAddress = stream.AllocateBlock();                                                  // 分配一个块索引
			}

			stream.WriteBlock(BlockAddress, memoryStream.ToArray());                                    // 将文件夹对象数据写入指定块
		}

		public static NFATFolder FromStream(BinaryReader binaryReader)
		{
			ushort NameLength = binaryReader.ReadUInt16();

			binaryReader.BaseStream.Position = 16;

			DateTime CreateTime = DateTime.FromFileTime(binaryReader.ReadInt64());
			DateTime UpdateTime = DateTime.FromFileTime(binaryReader.ReadInt64());
			
			Guid ID = new(binaryReader.ReadBytes(16));
			Guid ParentID = new(binaryReader.ReadBytes(16));

			string Name = new(binaryReader.ReadChars(NameLength));

			NFATFolder folder = new(Name, ID, CreateTime, UpdateTime)
			{
				ParentID = ParentID
			};

			return folder;
		}

		public void AddToParent(INFATObject @object)
		{
			if (!SubItems.Contains(@object))
			{
				SubItems.Add(@object);
			}
		}
	}

	public class NFATInformation(ulong version, DateTime dateTime, Guid guid, long expandBlockLength) : INFATBlockObject
	{
		public override long BlockAddress { get; set; }= 0;

		public ulong Version = version;           // 版本号
		public DateTime CreateTime = dateTime;     // 创建时间
		public Guid Guid = guid;               // 文件系统GUID
		public long ExpandBlockLength = expandBlockLength;  // 已拓展块长度

		public static NFATInformation CreateNew()
		{
			return new NFATInformation(1, DateTime.Now, Guid.NewGuid(), 0);
		}

		public override void ToStream(NFATtream stream, long blockAddress = 0)
		{
			MemoryStream memoryStream = new(1024);
			BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);

			binaryWriter.Write("NFAT".ToCharArray());	// 4字节标识
			binaryWriter.Seek(4, SeekOrigin.Current);	// 4字节保留
			binaryWriter.Write(Version);				// 8字节版本号
			binaryWriter.Write(Guid.ToByteArray());		// 16字节GUID
			binaryWriter.Write(CreateTime.ToBinary());	// 8字节创建时间
			binaryWriter.Write(ExpandBlockLength);		// 8字节已拓展块长度

			binaryWriter.Close();

			stream.WriteBlock(blockAddress, memoryStream.ToArray());
		}

		public static NFATInformation FromStream(NFATtream stream, long blockAddress = 0)
		{
			byte[] binary = stream.ReadBlock(blockAddress);

			MemoryStream memoryStream = new(binary);
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);
			string header = new(binaryReader.ReadChars(4)); // 读取4字节标识
			if (header != "NFAT") // 如果标识不正确，则需要新建数据
			{
				return null;
			}
			
			binaryReader.BaseStream.Seek(4, SeekOrigin.Current); // 跳过4字节保留
			
			ulong version = binaryReader.ReadUInt64(); // 读取版本号
			Guid guid = new(binaryReader.ReadBytes(16)); // 读取GUID
			DateTime createTime = DateTime.FromBinary(binaryReader.ReadInt64()); // 读取创建时间
			long expandBlockLength = binaryReader.ReadInt64(); // 读取已拓展块长度
			
			binaryReader.Close();

			return new NFATInformation(version, createTime, guid, expandBlockLength);
		}
	}

	// NFAT文件系统读写流
	public class NFATtream : Stream, IDisposable
	{
		public const int BlockSize = 1024;

		public NFATInformation NFATInformation;

		public Stream Stream;

		public BinaryWriter DataWriter;
		public BinaryReader DataReader;

		public MemoryMappedFile MappedFile;

		public IdleBlockInformation RootIdleInformation;
		public StorageAddressBlock StorageAddressBlock;

		public NFATFolder Root;

		public override bool CanRead => Stream.CanRead;

		public override bool CanSeek => Stream.CanSeek;

		public override bool CanWrite => Stream.CanWrite;

		public override long Length => Stream.Length;

		public override long Position
		{
			get
			{
				return Stream.Position;
			}

			set
			{
				Stream.Position = value;
			}
		}

		public long UnusedBlocksCount
		{
			get
			{
				return TotalBlockCount - NFATInformation.ExpandBlockLength;
			}
		}

		public long TotalBlockCount
		{
			get
			{
				return Stream.Length / BlockSize;
			}
		}

		public long IdleBlocksCount => GetIdleBlockCount(RootIdleInformation); // 空闲块数量

		public long ExpandBlockCurrent => NFATInformation.ExpandBlockLength + 10; // 已拓展块数量

		public NFATtream(Stream stream, bool create)
		{
			Stream = stream; // 设置底层流

			DataWriter = new BinaryWriter(Stream, Encoding.UTF8, true); // 初始化二进制写入器
			DataReader = new BinaryReader(Stream, Encoding.UTF8, true); // 初始化二进制读取器

			if (create)
			{
				if (Stream is FileStream)
				{
					Stream.SetLength(0); // 清空文件流内容
					Stream.Flush();
				}

				NFATInformation = NFATInformation.CreateNew();			// 新建NFAT信息对象
				RootIdleInformation = new IdleBlockInformation();       // 新建空闲块信息对象
				StorageAddressBlock = new StorageAddressBlock();		// 新建存储信息地址映射块
				WriteRecordingArea();									// 写入记录区信息
			}

			ReadRecordingArea(); // 读取记录区信息

			Root = new NFATFolder(".", NFATInformation.Guid, NFATInformation.CreateTime, DateTime.MinValue); // 初始化根文件夹对象

			LoadAllStorageInformation();
		}

		/* NFAT结构
		 * +---+---+---+-------+------------------+
		 * | 0 | 1 | 2 | 3 - 9 | 10 20 30 40 More |
		 * +---+---+---+-------+------------------+
		 *
		 *          Block0 : 标识以及基本数据块
		 *          Block1 : 空闲块根数据块
		 *          Block2 : 存储信息数据块
		 *        Block3-9 : 日志信息
		 * Block10 To More : 数据&动态标识块
		 */

		public const long NFATInformationBlock = 0;
		public const long IdleBlockInformationBlock = 1;
		public const long StorageInformationBlcok = 2;

		public void ReadRecordingArea()
		{
			NFATInformation = NFATInformation.FromStream(this) ?? throw new Exception("不存在NFAT文件系统的特征信息");	// 读取并解析0号块的二进制数据为NFAT信息对象

			RootIdleInformation = IdleBlockInformation.FromStream(this);												// 解析二进制数据为空闲块信息对象

			StorageAddressBlock = StorageAddressBlock.FromStream(this);													// 解析二进制数据为存储信息地址映射块

		}

		public void WriteRecordingArea()
		{
			if (TotalBlockCount < 10)                                               // 如果当前块数量小于10
			{
				ExpandBlocks(10, false);                                            // 扩展10个块
			}

			NFATInformation.ToStream(this);											// 写入0号块的NFAT信息对象
			RootIdleInformation.ToStream(this);                                     // 获取空闲块信息对象的二进制数据
			StorageAddressBlock.ToStream(this, StorageInformationBlcok);			// 写入2号块的存储信息地址映射块
		}

		public NFATFile AddFile(NFATFolder folder, FileStream data)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("只读模式下不允许添加文件和文件夹");
			}

			string name = Path.GetFileName(data.Name);
			data.Position = 0;

			BinaryReader binaryReader = new(data, Encoding.UTF8, true);

			long HeadBlockAddress = AllocateBlock();

			StorageInformation[] storageInformations = AllocateNewStorageAddress(data.Length);

			NFATFile file = new(name, storageInformations);

			file.SetParent(folder, true);
			folder.SubItems.Add(file);

			file.ToStream(this, HeadBlockAddress);
			AddStorageAddress(file.BlockAddress);

			foreach (StorageInformation storageInfo in file.StorageInformations)
			{
				if (storageInfo.StorageInformationType == StorageInformationType.Single)
				{
					byte[] binary = binaryReader.ReadBytes((int)storageInfo.Length);

					WriteBlock(storageInfo.BlockAddress, binary);
				}
				else if (storageInfo.StorageInformationType == StorageInformationType.Continuous)
				{
					long index = storageInfo.BlockAddress;
					long BlockCount = storageInfo.Length;

					while (index < storageInfo.BlockAddress + BlockCount)
					{
						byte[] binary = binaryReader.ReadBytes(BlockSize);

						WriteBlock(index, binary);
						index++;
					}
				}
				else
				{
					throw new InvalidDataException("不支持的存储信息类型");
				}
			}
			binaryReader.Close();
			
			Flush();
			
			return file;
		}

		public NFATFile[] AddFile(NFATFolder folder, params FileStream[] datas)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("只读模式下不允许添加文件和文件夹");
			}

			List<NFATFile> files = [];

			foreach (FileStream data in datas)
			{
				files.Add(AddFile(folder, data));
			}

			return [.. files];
		}

		public void RemoveFile(NFATFolder folder, params NFATFile[] files)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("只读模式下不允许删除文件和文件夹");
			}

			foreach (NFATFile file in files)
			{
				List<long> addresses = [];

				foreach (StorageInformation infomation in file.StorageInformations)
				{
					if (infomation.StorageInformationType == StorageInformationType.Single)
					{
						addresses.Add(infomation.BlockAddress);
					}
					else if (infomation.StorageInformationType == StorageInformationType.Continuous)
					{
						for (long i = infomation.BlockAddress; i < infomation.BlockAddress + infomation.Length; i++)
						{
							addresses.Add(i);
						}
					}
					else
					{
						throw new InvalidDataException("不支持的存储信息类型");
					}
				}

				addresses.AddRange(file.GetAllocateBlocks(this));

				addresses.Sort();

				foreach (long addr in addresses)
				{
					RootIdleInformation.PushIdleBlock(addr, this);
				}

				RemoveStorageAddress(file.BlockAddress);

				folder.SubItems.Remove(file);
			}
		}

		public NFATFolder AddFolder(NFATFolder folder, string name)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("只读模式下不允许添加文件和文件夹");
			}

			NFATFolder Folder = new(name);
			
			Folder.SetParent(folder, true);
			folder.SubItems.Add(Folder);

			Folder.ToStream(this);
			AddStorageAddress(Folder.BlockAddress);

			return Folder;
		}

		public NFATFolder[] AddFolder(NFATFolder folder, params string[] names)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("只读模式下不允许添加文件和文件夹");
			}

			List<NFATFolder> folders = [];

			foreach (string foldername in names)
			{
				folders.Add(AddFolder(folder, foldername));
			}

			return [.. folders];
		}

		public void RemoveFolder(NFATFolder folder, params NFATFolder[] folders)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("只读模式下不允许删除文件和文件夹");
			}

			foreach (NFATFolder folder1 in folders)
			{
				folder.SubItems.Remove(folder1);
			}

			long[] addresses = folder.GetAllocateBlocks(this);

			foreach (long addr in addresses)
			{
				RootIdleInformation.PushIdleBlock(addr, this);
			}

			RemoveStorageAddress(folder.BlockAddress);
		}

		public void AddStorageAddress(long Address)
		{
			if (StorageAddressBlock.Count < 127)
			{
				StorageAddressBlock.PushAddresses(Address);
				StorageAddressBlock.ToStream(this);
				return;
			}

			StorageAddressBlock.ToStream(this, -1);
			long NextBlockAddress = StorageAddressBlock.BlockAddress;

			StorageAddressBlock = new StorageAddressBlock()
			{
				NextBlockAddress = NextBlockAddress,
			};

			StorageAddressBlock.PushAddresses(Address);
			StorageAddressBlock.ToStream(this);
		}

		public void RemoveStorageAddress(long Address)
		{
			StorageAddressBlock block = StorageAddressBlock;

			Remove:
			if (!block.RemoveAddress(Address))
			{
				block = StorageAddressBlock.FromStream(this, block.NextBlockAddress);
				goto Remove;
			}

			if (StorageAddressBlock.Count <= 0)
			{
				if (StorageAddressBlock.NextBlockAddress != -1)
				{
					StorageAddressBlock = StorageAddressBlock.FromStream(this, StorageAddressBlock.NextBlockAddress);
					StorageAddressBlock.ToStream(this);
				}
			}

			block.ToStream(this, -1);
		}

		public override void Close()
		{
			DataWriter.Close();
			DataReader.Close();
			Stream.Close();
			base.Close();

			Dispose();
		}

		public new void Dispose()
		{
			Stream.Dispose();
			GC.SuppressFinalize(this);
		}

		public override void Flush()
		{
			DataWriter.Flush();
			Stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return Stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			Stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return Stream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Stream.Write(buffer, offset, count);
		}

		public void LoadAllStorageInformation()
		{
			Dictionary<Guid, NFATFolder> folders = [];
			Dictionary<Guid, NFATFile> files = [];

			long[] addresses = GetStorageInformationAddressTable();

			foreach (long address in addresses)
			{
				INFATObject @object = GetNFATObject(address);
				if (@object is NFATFolder folder)
				{
					folders.Add(folder.ID, folder);
				}
				else if (@object is NFATFile file)
				{
					files.Add(file.ID, file);
				}
				else
				{
					throw new InvalidDataException("不支持的NFAT对象类型");
				}
			}

			foreach (NFATFolder folder in folders.Values)
			{
				if (folders.TryGetValue(folder.ParentID, out NFATFolder value))
				{
					folder.SetParent(value, false);
					continue;
				}

				if (folder.ParentID == Root.ID)
				{
					folder.SetParent(Root, false);
					continue;
				}
			}

			foreach (NFATFile file in files.Values)
			{
				if (folders.TryGetValue(file.ParentID, out NFATFolder value))
				{
					file.SetParent(value, false);
					continue;
				}

				if (file.ParentID == Root.ID)
				{
					file.SetParent(Root, false);
					continue;
				}
			}
		}

		public long[] GetStorageInformationAddressTable()
		{
			List<long> addresses = [];

			StorageAddressBlock block = StorageAddressBlock;

			reg:

			int i = 0;

			while (i < block.Count)
			{
				addresses.Add(block[i]);
				i++;
			}

			if (block.NextBlockAddress > 0)
			{
				block = StorageAddressBlock.FromStream(this, block.NextBlockAddress);
				goto reg;
			}

			return [.. addresses];
		}

		public INFATObject GetNFATObject(long blockAddress)
		{
			MemoryStream memoryStream = new(ReadBlock(blockAddress));
			BinaryReader binaryReader = new(memoryStream, Encoding.UTF8);

			string header = new(binaryReader.ReadChars(INFATObject.DataHeader.Length));

			if (header != INFATObject.DataHeader)
			{
				throw new InvalidDataException("不正确的数据标识头");
			}

			byte type = binaryReader.ReadByte();

			INFATObject obj;
			if (type == 0)
			{
				obj = NFATFile.FromStream(binaryReader, this);
			}
			else if (type == 1)
			{
				obj = NFATFolder.FromStream(binaryReader);
			}
			else
			{
				throw new InvalidDataException("不正确的数据类型");
			}

			return obj;
		}

		public static (long TotalLength, long FileLength, long FolderLength) GetNFATFileTree(INFATObject[] files)
		{
			if (files == null || files.Length == 0)
			{
				return (0, 0, 0);
			}

			long Totallength = 0;
			long FileLength = 0;
			long FolderLength = 0;

			for (int i = 0; i < files.Length; i++)
			{
				if (files[i] is NFATFolder)
				{
					NFATFolder folder = files[i] as NFATFolder;

					if (folder.SubItems != null && folder.SubItemCount != 0)
					{
						(long, long, long) IMT = GetNFATFileTree([.. folder.SubItems]);

						Totallength += IMT.Item1;
						FileLength += IMT.Item2;
						FolderLength += IMT.Item3;
					}

					FolderLength++;
				}
				else
				{
					FileLength++;
				}

				Totallength++;
			}

			return (Totallength, FileLength, FolderLength);
		}

		public NFATFile[] ListALLFiles()
		{
			return TraverseFiles(Root);
		}

		public static NFATFile[] TraverseFiles(NFATFolder folder)
		{
			List<NFATFile> files = [];

			INFATObject[] objects = [.. folder.SubItems];
			foreach (INFATObject obj in objects)
			{
				if (obj is NFATFolder subfolder)
				{
					files.AddRange(TraverseFiles(subfolder));
					continue;
				}

				files.Add(obj as NFATFile);
			}

			return [.. files];
		}

		/// <summary>
		///	查找指定数量的使用过但已空闲的块
		/// </summary> 
		/// <returns>使用过但已空闲的块索引数组</returns>
		public (long[], long) AllocateIdleBlocks(long BlockCount)
		{
			if (RootIdleInformation is null)
			{
				return (null, BlockCount);
			}

			List<long> idleBlocks = []; // 新建空闲块列表

			for (long i = BlockCount; i > 0 ; i--)
			{
				long block = RootIdleInformation.PopIdleBlock(this); // 弹出一个空闲块索引
				
				if (block < 0)
				{
					return (idleBlocks.ToArray(), i); // 返回空闲块索引数组以及是否分配完
				}

				idleBlocks.Add(block); // 将空闲块索引添加到列表中
			}

			return (idleBlocks.ToArray(), 0); // 返回空闲块索引数组以及是否分配完
		}

		public long GetIdleBlockCount(IdleBlockInformation infomation)
		{
			long count = infomation.IdleBlockCount; // 获取当前组空闲块数量

			// 如果不是最后一组空闲块信息，则递归计算下一组空闲块数量
			if (!infomation.IsLast)
			{
				long[] blocks = [.. infomation.IdleBlocks];													// 获取当前空闲块索引数组
				IdleBlockInformation nextinfo = IdleBlockInformation.FromStream(this, blocks[^1]);			// 解析二进制数据为空闲块信息对象

				count += GetIdleBlockCount(nextinfo);														// 递归计算下一组空闲块数量
			}

			return count; // 返回空闲块数量
		}

		public static long GetRequiredBlocksCount(long size)
		{
			long blocksCount = size / BlockSize; // 计算数据存储所需的块数量
			long requiredBlocksCount = size % BlockSize == 0 ? blocksCount : blocksCount + 1; // 如果数据整数分配块后有剩余数据，则需要额外的一个块存储剩余数据

			return requiredBlocksCount; // 返回所需的块数量
		}


		/// <summary>
		/// 分配一个块
		/// </summary>
		/// <returns>块索引</returns>
		public long AllocateBlock()
		{
			long blockAddress = RootIdleInformation.PopIdleBlock(this);

			if (blockAddress == -1)
			{
				blockAddress = ExpandBlockCurrent;
				ExpandBlocks(1);
			}

			return blockAddress;
		}

		/// <summary>
		/// 分配新的存储地址
		/// 使用未使用或使用过但已空闲的块根据数据大小进行分配
		/// </summary>
		/// <param name="size">需要分配的数据大小</param>
		/// <returns>存储分配数据</returns>
		public StorageInformation[] AllocateNewStorageAddress(long size)
		{
			List<StorageInformation> storageInformations = []; // 新建存储分配信息列表

			long BaseExpandBlockCurrent = ExpandBlockCurrent; // 记录扩展块的索引

			long filesize = size; // 内部设定文件大小

			long RequiredBlocksCount = GetRequiredBlocksCount(filesize); // 计算所需的块数量
			(long[] IdleBlocks, long ExpandBlockCount) = AllocateIdleBlocks(RequiredBlocksCount); // 分配空闲的块和需要扩展的块数量

			if (IdleBlocks is not null)
			{
				Array.Sort(IdleBlocks); // 对空闲块进行升序排序，优先使用前面的块
			}

			// 如果需要拓展块，则根据实际情况处理
			if (ExpandBlockCount > 0)
			{
				ExpandBlocks(ExpandBlockCount); // 扩展块
			}

			// 使用空闲块分配存储信息
			long ContinueStartBlock = -1;
			long RegeisterCount = 0; // 注册块的数量
			long LastBlock = -1; // 上一个块索引

			if (IdleBlocks is null)
			{
				goto AllocateExpand; // 如果没有空闲块，则跳转到扩展分配
			}

			foreach (long block in IdleBlocks)
			{
				re:
				// 如果文件大小已经分配完毕
				if (filesize < BlockSize)
				{
					// 如果前面有连续块，则先分配连续块存储信息
					if (RegeisterCount > 1)
					{
						storageInformations.Add(new StorageInformation(StorageInformationType.Continuous, ContinueStartBlock, RegeisterCount)); // 分配连续块存储信息
						RegeisterCount = 0;		// 重置注册块数量
						ContinueStartBlock = -1;// 重置连续块起始索引
					}

					// 如果还有剩余文件大小，则分配单块存储信息
					if (filesize > 0)
					{
						// 分配单块存储信息
						storageInformations.Add(new StorageInformation(StorageInformationType.Single, block, filesize));
						filesize = 0;			// 文件大小分配完毕
					}

					break;						// 跳出循环
				}

				if (ContinueStartBlock < 0)     // 如果还没有设置连续块起始索引
				{
					RegeisterCount++;           // 注册块数量增加
					ContinueStartBlock = block; // 设置连续块起始索引
					LastBlock = block;          // 设置上一个块的地址
					filesize -= BlockSize;      // 减少文件大小
					continue;                   // 起始块跳过下面步骤
				}

				if (block - LastBlock == 1)     // 通过上一个块的地址判断是否为连续块
				{
					RegeisterCount++;           // 注册块数量增加
					LastBlock = block;          // 设置上一个块的地址
					filesize -= BlockSize;      // 减少文件大小
					continue;                   // 如果是连续块，则继续分配
				}

				// 如果不是连续块，则按下述逻辑处理

				// 如果前面有注册的连续块，则先分配连续块存储信息
				if (RegeisterCount > 1)
				{
					// 分配连续块存储信息
					storageInformations.Add(new StorageInformation(StorageInformationType.Continuous, ContinueStartBlock, RegeisterCount));
				}
				else
				{
					// 分配单块存储信息
					storageInformations.Add(new StorageInformation(StorageInformationType.Single, LastBlock, filesize >= BlockSize ? BlockSize : filesize));
				}

				RegeisterCount = 0;         // 重置注册块数量
				ContinueStartBlock = -1;    // 重置连续块起始索引

				goto re; // 重新处理当前块
			}

			if (IdleBlocks is not null && IdleBlocks.Length > 0)
			{
				// 处理最后剩余的注册块
				if (RegeisterCount > 1)
				{
					// 分配连续块存储信息
					storageInformations.Add(new StorageInformation(StorageInformationType.Continuous, ContinueStartBlock, RegeisterCount));
				}
				else
				{
					// 分配单块存储信息
					storageInformations.Add(new StorageInformation(StorageInformationType.Single, LastBlock, filesize >= BlockSize ? BlockSize : filesize));
				}
			}

			AllocateExpand: // 扩展分配

			// 如果分配已经完成
			if (filesize <= 0)
			{
				goto ret; // 跳转到返回
			}

			// 如果还需要分配信息
			long blockcount = filesize / BlockSize;  // 计算还需要分配的块数量
			int remian = (int)(filesize % BlockSize); // 计算还需要分配的剩余数据大小

			// 如果需要分配连续存储信息
			if (blockcount > 0)
			{
				storageInformations.Add(new StorageInformation(StorageInformationType.Continuous, BaseExpandBlockCurrent, blockcount)); // 分配连续块存储信息
				BaseExpandBlockCurrent += blockcount; // 增加当前扩展块索引
			}

			// 如果需要分配单块存储信息
			if (remian > 0)
			{
				storageInformations.Add(new StorageInformation(StorageInformationType.Single, BaseExpandBlockCurrent, remian)); // 分配单块存储信息
			}

			ret:
			return [.. storageInformations];
		}

		public void ExpandBlocks(long ExpandBlockCount, bool add = true)
		{
			if (Stream is FileStream)
			{
				long expandLength = ExpandBlockCount * BlockSize;

				// 如果是文件流，则扩展文件长度
				Stream.SetLength(Stream.Length + expandLength); // 扩展文件长度
				Stream.Flush(); // 刷新文件流
			}
			else if (Stream is DiskStream diskStream)
			{
				// 如果是物理硬盘流，则不支持扩展，计算还未使用过的块数量决定是否可以存储数据
				long UnusedBlocksCount = (diskStream.Length / BlockSize) - NFATInformation.ExpandBlockLength; // 获取未使用的块数量

				if (UnusedBlocksCount < ExpandBlockCount)
				{
					throw new NotSupportedException($"磁盘\"{diskStream.DiskInfo.Name}\"的剩余空间不足以拓展");
				}
			}

			if (add)
			{
				NFATInformation.ExpandBlockLength += ExpandBlockCount;
				NFATInformation.ToStream(this);
			}
		}

		public void SetBlock(long BlockAddress)
		{
			long position = BlockAddress * BlockSize;

			if (position < 0 || (position > 0 && position + BlockSize > Stream.Length))
			{
				throw new ArgumentOutOfRangeException(nameof(BlockAddress), "块索引超出范围");
			}

			Stream.Position = position;
		}

		public byte[] ReadBlock(long BlockAddress)
		{
			SetBlock(BlockAddress);

			return DataReader.ReadBytes(BlockSize);
		}

		public void WriteBlock(long BlockAddress, byte[] data)
		{
			if (data.Length < BlockSize)
			{
				Array.Resize(ref data, BlockSize);
			}
			if (data.Length > BlockSize)
			{
				throw new ArgumentOutOfRangeException(nameof(data), "写入数据长度超出块大小");
			}

			SetBlock(BlockAddress);
			DataWriter.Write(data);

			Flush(); // 刷新流
		}
	}
}
