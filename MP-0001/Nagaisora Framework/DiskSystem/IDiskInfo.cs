using System.IO;
using System.Linq;

namespace NagaisoraFramework
{
	public abstract class IDiskInfo
	{
		public abstract ulong StartBlock { get; }
		public abstract ulong EndBlock { get; }
		public abstract ulong BlockSize { get; }
		public abstract ulong DataLength { get; }
		public abstract byte[] SaveID { get; }

		public static IDiskInfo FormBinary(byte[] vs)
		{
			if (vs.Length < 512)
			{
				return null;
			}

			MemoryStream memoryStream = new MemoryStream(vs);
			BinaryReader binaryReader = new BinaryReader(memoryStream);

			binaryReader.BaseStream.Position = 0;

			char[] Hider = binaryReader.ReadChars(8);

			binaryReader.BaseStream.Position = 0;

			if (new string(Hider.Take(4).ToArray()) == "NFAT")
			{
				//return NFATInfo.FormBinary(binaryReader.ReadBytes(1024));
				return null;
			}
			else if (new string(Hider) == "RTA PART")
			{
				return RTA.FromBinary(binaryReader.ReadBytes(1024));
			}
			else
			{
				return null;
			}

			//binaryReader.Close();
			//memoryStream.Close();
		}
	}
}
