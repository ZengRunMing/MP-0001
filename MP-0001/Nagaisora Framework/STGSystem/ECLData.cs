//using System.IO;
//using System.Text;
//using System.Collections.Generic;
//using System;
//using System.Reflection;

//namespace NagaisoraFramework.STGSystem
//{
//	public class ECLData
//	{
//		public static readonly string EditorHeader = "EDIECL";
//		public static readonly string ExecutableHeader = "EXTECL";

//		public uint SystemVersion;
//		public string Name;
//		public string Description;
//		public uint FileVersion;
//		public string Author;

//		public List<INFEItem> ECLItems;

//		public Assembly ECLAssembly;

//		public bool IsRelease = false;
//		public bool IsExecutable = false;

//		public ECLData()
//		{
//			SystemVersion = BitConverter.ToUInt32(new byte[] { 1, 0, 0, 0 }, 0);
//			FileVersion = BitConverter.ToUInt32(new byte[] { 1, 0, 0, 0 }, 0);
//			Author = "Unknown";
//			ECLItems = new List<INFEItem>();
//		}

//		public static ECLData Create(string name = "Default", string description = "DefaultECL", string author = "Unknown")
//		{
//			ECLData data = new ECLData
//			{
//				Name = name,
//				Description = description,
//				Author = author,
//				ECLItems = new List<INFEItem>(),
//			};

//			INFEItem item = new ECLMain
//			{
//				Name = "Main",
//				Description = "ECL的程序入口",
//			};
//			item.Init();

//			data.ECLItems.Add(item);

//			return data;
//		}

//		public byte[] ToBinary()
//		{
//			MemoryStream memoryStream = new MemoryStream();
//			BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true);

//			binaryWriter.Write(EditorHeader.ToCharArray());		// 文件头标识
//			binaryWriter.Write(SystemVersion);					// 系统版本

//			binaryWriter.Write(Name ?? string.Empty);			// 名称
//			binaryWriter.Write(Description ?? string.Empty);	// 描述
//			binaryWriter.Write(FileVersion);					// 文件版本
//			binaryWriter.Write(Author ?? string.Empty);			// 作者署名

//			binaryWriter.Write(ECLItems.Count); // 写入 ECLItems 的数量

//			foreach (var item in ECLItems)
//			{
//				if (item is ECLMain)
//				{
//					binaryWriter.Write((byte)0); // 写入 ECLMain 类型标识
//				}
//				else if (item is ECLClass)
//				{
//					binaryWriter.Write((byte)1); // 写入 ECLClass 类型标识
//				}
//				else if (item is ECLInterrupt)
//				{
//					binaryWriter.Write((byte)2); // 写入 ECLInterrupt 类型标识
//				}
//				else if (item is ECLCondition)
//				{
//					binaryWriter.Write((byte)3); // 写入 ECLCondition 类型标识
//				}
//				else if (item is ECLInterface)
//				{
//					binaryWriter.Write((byte)4); // 写入 ECLFunction 类型标识
//				}
//				else
//				{
//					throw new InvalidDataException($"Unknown ECLItem type: {item.GetType().Name}");
//				}

//				binaryWriter.Write(item.Name ?? string.Empty); // 写入每个 ECLItem 的名称
//				binaryWriter.Write(item.Description ?? string.Empty); // 写入每个 ECLItem 的描述
//				binaryWriter.Write(item.ExecuteCode ?? string.Empty); // 写入每个 ECLItem 的执行代码
//			}

//			binaryWriter.Close(); // 关闭二进制写入器

//			return memoryStream.ToArray(); // 返回完整的二进制数据
//		}

//		public static ECLData FromBinary(byte[] binary)
//		{
//			MemoryStream memoryStream = new MemoryStream(binary); // 创建内存流
//			BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8, true); // 创建二进制读取器

//			char[] header = binaryReader.ReadChars(EditorHeader.Length); // 读取文件头标识

//			string headerString = new string(header);

//			if (headerString != EditorHeader && headerString != ExecutableHeader)
//			{
//				throw new InvalidDataException($"Invalid file header: {headerString}. Expected: {EditorHeader} or {ExecutableHeader}"); // 检查文件头标识是否正确
//			}

//			ECLData data = new ECLData // 创建 ECLData 实例
//			{
//				SystemVersion = binaryReader.ReadUInt32(), // 读取系统版本
//				Name = binaryReader.ReadString(), // 读取名称
//				Description = binaryReader.ReadString(), // 读取描述
//				FileVersion = binaryReader.ReadUInt32(), // 读取版本
//				Author = binaryReader.ReadString(), // 读取作者署名
//			};

//			if (headerString == EditorHeader)
//			{
//				int itemCount = binaryReader.ReadInt32(); // 读取 ECLItems 的数量

//				data.ECLItems = new List<INFEItem>(itemCount); // 初始化 ECLItems 列表

//				for (int i = 0; i < itemCount; i++)
//				{
//					byte itemType = binaryReader.ReadByte(); // 读取每个 ECLItem 的类型标识

//					string itemName = binaryReader.ReadString(); // 读取每个 ECLItem 的名称
//					string itemDescription = binaryReader.ReadString(); // 读取每个 ECLItem 的描述
//					string itemExecuteCode = binaryReader.ReadString(); // 读取每个 ECLItem 的执行代码
//					INFEItem item;

//					switch (itemType)
//					{
//						case 0: // ECLMain
//							item = new ECLMain
//							{
//								Name = itemName,
//								Description = itemDescription,
//								ExecuteCode = itemExecuteCode
//							};
//							break;
//						case 1: // ECLFunction
//							item = new ECLClass
//							{
//								Name = itemName,
//								Description = itemDescription,
//								ExecuteCode = itemExecuteCode
//							};
//							break;
//						case 2: // ECLInterrupt
//							item = new ECLInterrupt
//							{
//								Name = itemName,
//								Description = itemDescription,
//								ExecuteCode = itemExecuteCode
//							};
//							break;
//						case 3: // ECLCondition
//							item = new ECLCondition
//							{
//								Name = itemName,
//								Description = itemDescription,
//								ExecuteCode = itemExecuteCode
//							};
//							break;
//						case 4: // ECLInterface
//							item = new ECLInterface
//							{
//								Name = itemName,
//								Description = itemDescription,
//								ExecuteCode = itemExecuteCode
//							};
//							break;
//						default:
//							throw new InvalidDataException($"Unknown ECLItem type: {itemType}");
//					}

//					data.ECLItems.Add(item); // 添加到 ECLItems 列表中
//				}
//				goto close;
//			}

//			data.IsExecutable = true; // 设置为可执行文件

//			data.IsRelease = !binaryReader.ReadBoolean(); // 读取调试标志
//			int assemblyLength = binaryReader.ReadInt32(); // 读取程序集长度
//			int pdbLength = binaryReader.ReadInt32(); // 读取 PDB 长度
//			byte[] assemblyBinary = binaryReader.ReadBytes(assemblyLength); // 读取程序集二进制数据
//			byte[] pdbBinary = binaryReader.ReadBytes(pdbLength); // 读取 PDB 二进制数据

//			data.ECLAssembly = MainSystem.LoadAssembly(assemblyBinary); // 加载程序集

//			close:
//			binaryReader.Close();
//			memoryStream.Close();

//			return data;
//		}
//	}
//}
