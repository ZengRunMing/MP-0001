using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Win32.SafeHandles;

/*
 * 长空STG架构
 * 底层系统
 * By 万影 星梦
 * 魔晶自动化机电工程社团
 * 保留基本的著作权利
*/

namespace NagaisoraFramework.Underlyingsystem
{
	using static WindowsAPI;

	public class DiskInfo
	{
		public string Name = "Not Applicable";
		public string Model = "Not Applicable";
		public string SerialNumber = "Not Applicable";
		public string InterfaceType = "Not Applicable";
		public string Description = "Not Applicable";
		public long Size;
		public string Status = "Not Applicable";

		public DiskInfo(string name, string description, long size, string status)
		{
			Name = name;
			Description = description;
			Size = size;
			Status = status;
		}

		public DiskInfo(string name, string model, string serialnumber, string interfacetype, string description, long size, string status)
		{
			Name = name;
			Model = model;
			SerialNumber = serialnumber;
			InterfaceType = interfacetype;
			Description = description;
			Size = size;
			Status = status;
		}

		public override string ToString()
		{
			return $"{Name.TrimStart(new char[] { '\\', '.' }),-15} {Model,-35} {InterfaceType,-20} ({Size / 1024f / 1024f / 1024f,-5} GB)";
		}
	}

	/// <summary>
	/// 物理磁盘操作类
	/// <para>警告 :</para>
	/// <para>该方法通过Windows API直接操作底层，具有一定的危险性</para>
	/// <para>需要程序以管理员权限运行</para>
	/// </summary>
	public class DiskStream : Stream
	{
		public DiskInfo DiskInfo;

		public static int WriteLength = 0;

		public int BufferSize;

		public SafeFileHandle DiskHandle;
		public FileStream BaseFileStream;

		public override bool CanRead => BaseFileStream.CanRead;

		public override bool CanSeek => BaseFileStream.CanSeek;

		public override bool CanWrite => BaseFileStream.CanWrite;

		public override long Length => _Length;

		public override long Position { get => BaseFileStream.Position; set => BaseFileStream.Position = value; }

		public long _Length;

		public DiskStream(FileStream fileStream, int bufferSize = 512)
		{
			BaseFileStream = fileStream;
			BufferSize = bufferSize;
			_Length = fileStream.Length;
			return;
		}

		public DiskStream(string info, int bufferSize = 512, FileAccess fileAccess = FileAccess.ReadWrite, bool IsAsync = false)
		{
			DiskHandle = CreateFile($"\\\\.\\{info}", AccessFlag.GenericRead | AccessFlag.GenericWrite, ShareMode.FileShareRead | ShareMode.FileShareWrite, IntPtr.Zero, CreateMode.OpenExisting, 0, IntPtr.Zero);

			if (DiskHandle.IsInvalid)
			{
				throw new SystemException($"打开磁盘{info}的过程中发生未知的错误\n请检查程序是否以管理员权限运行，以及对应磁盘处于联机状态");
			}

			BaseFileStream = new FileStream(DiskHandle, fileAccess, bufferSize, IsAsync);

			BufferSize = bufferSize;

			return;
		}

		public DiskStream(DiskInfo info, int bufferSize = 512, FileAccess fileAccess = FileAccess.ReadWrite, bool IsAsync = false) : this(info.Name.TrimStart(new char[] { '\\', '.' }), bufferSize, fileAccess, IsAsync)
		{
			DiskInfo = info;
			SetLength(info.Size);
			return;
		}

		public override void SetLength(long value)
		{
			_Length = value;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			EMoveMethod moveMethod = EMoveMethod.Begin;

			switch (origin)
			{
				case SeekOrigin.Begin:
					moveMethod = EMoveMethod.Begin;
					break;
				case SeekOrigin.Current:
					moveMethod = EMoveMethod.Current;
					break;
				case SeekOrigin.End:
					moveMethod = EMoveMethod.End;
					break;
			}

			SetFilePointer(BaseFileStream.SafeFileHandle, offset, IntPtr.Zero, moveMethod);

			return BaseFileStream.Position;
		}

		public override void Flush()
		{
			BaseFileStream.Flush();
		}

		[SecuritySafeCritical]
		public override int Read(byte[] array, int offset, int count)
		{
			return BaseFileStream.Read(array, offset, count);
		}

		public override Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
		{
			AsyncRead(array, offset, count);

			return Task.FromResult(0);
		}

		public async void AsyncRead(byte[] array, int offset, int count)
		{
			await Task.Run(() =>
			{
				BaseFileStream.Read(array, offset, count);
			});
		}

		[SecuritySafeCritical]
		public override void Write(byte[] array, int offset, int count)
		{
			Task task = BaseFileStream.WriteAsync(array, offset, count);

			Task.WaitAll(task);
			return;
		}

		public override void Close()
		{
			BaseFileStream.Close();
			DiskHandle?.Close();

			return;
		}

		/// <summary>
		/// 从物理磁盘扇区读取数据
		/// <para>警告 :</para>
		/// <para>该方法通过Windows API直接操作底层，具有一定的危险性</para>
		/// <para>需要程序以管理员权限运行</para>
		/// </summary>
		/// <param name="SectorIndex">物理磁盘扇区号，从0开始</param>
		/// <returns>读取出的数据</returns>
		public byte[] ReadSector(long SectorIndex)
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			BaseFileStream.Position = SectorIndex * BufferSize;
			byte[] ReturnByte = new byte[BufferSize];
			Task task = BaseFileStream.ReadAsync(ReturnByte, 0, BufferSize); //获取扇区

			Task.WaitAll(task);

			return ReturnByte;
		}

		public byte[][] ReadSectorsArray(long SectorIndex, long length)
		{
			byte[][] buffer = new byte[length][];

			for (long i = SectorIndex; i < SectorIndex + length; i++)
			{
				buffer[i] = ReadSector(i);
			}

			return buffer;
		}

		public byte[] ReadSectors(long SectorIndex, long length)
		{
			MemoryStream memoryStream = new MemoryStream();

			for (long i = SectorIndex; i <= SectorIndex + length - 1; i++)
			{
				byte[] bytes = ReadSector(i);
				memoryStream.Write(bytes, 0, bytes.Length);
			}

			return memoryStream.ToArray();
		}

		public byte[] ReadSectors(long SectorIndex, long Size, out long NowPosition)
		{
			NowPosition = 0;

			MemoryStream memoryStream = new MemoryStream();

			for (long i = SectorIndex; i <= SectorIndex + Size - 1; i++)
			{
				byte[] bytes = ReadSector(i);
				memoryStream.Write(bytes, 0, bytes.Length);

				NowPosition += BufferSize;
			}

			return memoryStream.ToArray();
		}

		public byte[] ReadSector()
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			byte[] ReturnByte = new byte[BufferSize];
			BaseFileStream.Read(ReturnByte, 0, BufferSize); //获取扇区

			return ReturnByte;
		}

		/// <summary>
		/// 将扇区数据写入到物理磁盘扇区中
		/// <para>警告 :</para>
		/// <para>该方法通过Windows API直接操作底层，具有一定的危险性</para>
		/// <para>需要程序以管理员权限运行</para>
		/// </summary>
		/// <param name="SectorIndex">物理磁盘扇区参数</param>
		/// <param name="SectorBytes">要写入的数据，不满512字节则用0xFF补全</param>
		public void WriteSector(long SectorIndex, byte[] SectorBytes)
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			if (SectorBytes.Length > BufferSize)
			{
				throw new Exception("字节数组长度大于扇区最大支持的长度，请拆分后再写入");
			}

			List<byte> vs;
			if (SectorBytes.Length < BufferSize)
			{
				vs = new List<byte>();

				for (int i = 0; i < BufferSize; i++)
				{
					if (i < SectorBytes.Length)
					{
						vs.Add(SectorBytes[i]);
					}
					else
					{
						vs.Add(0);
					}
				}
				SectorBytes = vs.ToArray();
			}

			BaseFileStream.Position = SectorIndex * BufferSize;
			BaseFileStream.Write(SectorBytes, 0, BufferSize);

			Flush();
			return;
		}

		public void WriteSector(byte[] SectorBytes)
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			if (SectorBytes.Length > BufferSize)
			{
				throw new Exception("字节数组长度大于扇区最大支持的长度，请拆分后再写入");
			}

			List<byte> vs;
			if (SectorBytes.Length < BufferSize)
			{
				vs = new List<byte>();

				for (int i = 0; i < BufferSize; i++)
				{
					if (i < SectorBytes.Length)
					{
						vs.Add(SectorBytes[i]);
					}
					else
					{
						vs.Add(0);
					}
				}
				SectorBytes = vs.ToArray();
			}

			BaseFileStream.Write(SectorBytes, 0, BufferSize);

			Flush();
			return;
		}

		public void WriteData(long SectorIndex, byte[] SectorBytes, out float WritePosition)
		{
			WritePosition = 0f;

			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			List<byte> vs;

			float Length = (float)SectorBytes.Length / (float)BufferSize;

			WriteLength = SectorBytes.Length;

			for (long j = 0; j < Math.Ceiling(Length); j++)
			{
				vs = new List<byte>();
				for (long i = 0; i < BufferSize; i++)
				{
					if (i + (BufferSize * j) <= SectorBytes.Length)
					{
						vs.Add(SectorBytes[i + (BufferSize * j)]);
					}
					else
					{
						break;
					}
				}

				WriteSector(SectorIndex, vs.ToArray());
				WritePosition = (float)j * (float)BufferSize;
				SectorIndex++;
			}
			WritePosition = (float)Length + 1 * (float)BufferSize;
			return;
		}

		public void WriteData(long SectorIndex, byte[] SectorBytes)
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			List<byte> vs;

			float Length = (float)SectorBytes.Length / (float)BufferSize;

			WriteLength = SectorBytes.Length;

			for (long j = 0; j < Math.Ceiling(Length); j++)
			{
				vs = new List<byte>();
				for (long i = 0; i < BufferSize; i++)
				{
					if (i + (BufferSize * j) > SectorBytes.Length)
					{
						break;
					}

					vs.Add(SectorBytes[i + (BufferSize * j)]);
				}

				WriteSector(SectorIndex, vs.ToArray());
				SectorIndex++;
			}

			return;
		}

		public void WriteData(byte[] SectorBytes, out float WritePosition)
		{
			WritePosition = 0f;

			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			List<byte> vs;

			float Length = (float)SectorBytes.Length / (float)BufferSize;

			WriteLength = SectorBytes.Length;

			for (long j = 0; j < Math.Ceiling(Length); j++)
			{
				vs = new List<byte>();
				for (long i = 0; i < BufferSize; i++)
				{
					if (i + (BufferSize * j) >= SectorBytes.Length)
					{
						break;
					}

					vs.Add(SectorBytes[i + (BufferSize * j)]);
				}
				WriteSector(vs.ToArray());

				WritePosition = (float)j * (float)BufferSize;
			}
			WritePosition = SectorBytes.Length;

			return;
		}

		public void WriteData(byte[] SectorBytes)
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			List<byte> vs;

			float Length = (float)SectorBytes.Length / (float)BufferSize;

			WriteLength = SectorBytes.Length;

			for (long j = 0; j < Math.Ceiling(Length); j++)
			{
				vs = new List<byte>();
				for (long i = 0; i < BufferSize; i++)
				{
					if (i + (BufferSize * j) >= SectorBytes.Length)
					{
						break;
					}

					vs.Add(SectorBytes[i + (BufferSize * j)]);
				}
				WriteSector(vs.ToArray());
			}

			return;
		}

		public byte[] ReadData(long SectorIndex, long Size, out float WritePosition)
		{
			WritePosition = 0f;

			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			MemoryStream memoryStream = new MemoryStream();

			long Length = Size / BufferSize;

			long setleng = Size % BufferSize;

			for (long j = 0; j < Length; j++)
			{
				byte[] vs1 = ReadSector(SectorIndex);
				memoryStream.Write(vs1, 0x00, vs1.Length);
				WritePosition = (float)j * (float)BufferSize;
				SectorIndex++;
			}

			byte[] vsr = ReadData(setleng);
			memoryStream.Write(vsr, 0x00, vsr.Length);

			WritePosition = (float)Length + 1 * (float)BufferSize;
			return memoryStream.ToArray();
		}

		public byte[] ReadData(long SectorIndex, long Size)
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			MemoryStream memoryStream = new MemoryStream();

			long Length = Size / BufferSize;

			long setleng = Size % BufferSize;

			for (long j = 0; j < Length; j++)
			{
				byte[] vs1 = ReadSector(SectorIndex);
				memoryStream.Write(vs1, 0x00, vs1.Length);
				SectorIndex++;
			}

			byte[] vsr = ReadData(setleng);
			memoryStream.Write(vsr, 0x00, vsr.Length);

			return memoryStream.ToArray();
		}

		public byte[] ReadData(long Size, out float WritePosition)
		{
			WritePosition = 0f;

			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			MemoryStream memoryStream = new MemoryStream();

			long Length = Size / BufferSize;

			long setleng = Size % BufferSize;

			for (long j = 0; j < Length; j++)
			{
				byte[] vs1 = ReadSector();
				memoryStream.Write(vs1, 0x00, vs1.Length);
				WritePosition = (float)j * (float)BufferSize;
			}

			byte[] vsr = ReadData(setleng);
			memoryStream.Write(vsr, 0x00, vsr.Length);

			WritePosition = Size;

			return memoryStream.ToArray();
		}

		public byte[] ReadData(long Size)
		{
			if (BaseFileStream == null)
			{
				throw new NullReferenceException("磁盘未实例化");
			}

			MemoryStream memoryStream = new MemoryStream();
			byte[] vs1 = new byte[Size];
			BaseFileStream.Read(vs1, 0x00, vs1.Length);
			memoryStream.Write(vs1, 0x00, vs1.Length);

			return memoryStream.ToArray();
		}
	}
}