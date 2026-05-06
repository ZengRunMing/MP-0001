using System.IO;
using System.Linq;
using System.Text;

namespace NagaisoraFramework
{
	public static class StringHelper
	{
		public static string HexTable(byte[] data)
		{
			MemoryStream memory = new MemoryStream(data);
			BinaryReader reader = new BinaryReader(memory);

			StringBuilder StringBuilder = new StringBuilder();

			StringBuilder.AppendLine("+--------------------+------------------------------------------------------------------+--------------------+");
			StringBuilder.AppendLine("|  ADDRESS           |  00  01  02  03  04  05  06  07  08  09  10  11  12  13  14  15  |  ASCII             |");
			StringBuilder.AppendLine("+--------------------+------------------------------------------------------------------+--------------------+");

			int lineindex = 0;

			while (memory.Position < memory.Length)
			{
				StringBuilder.Append($"|  0x{lineindex * 16:X14}  |  ");

				StringBuilder ASCII = new StringBuilder();

				for (int i = 0; i < 16; i++)
				{
					if (memory.Position >= memory.Length)
					{
						StringBuilder.Append("    ");
						ASCII.Append(' ');
						continue;
					}

					byte Byte = reader.ReadByte();

					StringBuilder.Append($"{Byte:X2}  ");

					if (Byte >= 0x20 && Byte <= 0x7E)
					{
						ASCII.Append((char)Byte);
					}
					else
					{
						ASCII.Append('.');
					}
				}

				StringBuilder.Append("|  ");
				StringBuilder.Append($"{ASCII}");
				StringBuilder.AppendLine("  |");

				lineindex++;
			}

			StringBuilder.AppendLine("+--------------------+------------------------------------------------------------------+--------------------+");

			return StringBuilder.ToString();
		}

		public static string GetFileSizeString(ulong Size)
		{
			string s = string.Empty;

			if (Size < 1024ul)
			{
				s = $"{Size}B";
			}
			else if (Size < 1024ul * 1024ul)
			{
				s = $"{(Size / 1024f):0.00}KiB";
			}
			else if (Size < 1024ul * 1024ul * 1024ul)
			{
				s = $"{(Size / 1024f / 1024f):0.00}MiB";
			}
			else if (Size < 1024ul * 1024ul * 1024ul * 1024ul)
			{
				s = $"{(Size / 1024f / 1024f / 1024f):0.00}GiB";
			}
			else
			{
				s = $"{(Size / 1024f / 1024f / 1024f / 1024f):0.00}TiB";
			}

			return s;
		}

		public static string GetStringFromBinary(byte[] binary, Encoding encoding)
		{
			byte[] StringBinary = binary.TakeWhile(b => b != 0).ToArray();
			return encoding.GetString(StringBinary);
		}

		public static string Readtxt(string[] strs, int linenumber)
		{
			if (linenumber == 0 || linenumber > strs.Length)
			{
				return string.Empty;
			}
			return strs[linenumber].Trim();
		}
	}
}
