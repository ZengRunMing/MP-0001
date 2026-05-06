using System.Collections;
using System.Collections.Generic;

namespace NagaisoraFramework
{
	public static class BitArrayBool
	{
		public static byte[] ToBinary(bool[] Bools)
		{
			int size = Bools.Length / 8;
			int csize = Bools.Length % 8;

			BitArray array = new(Bools.Length);
			for (int i = 0; i < Bools.Length; i++)
			{
				array[i] = Bools[i];
			}

			int size1 = size;
			if (csize != 0)
			{
				size1++;
			}

			byte[] bytes = new byte[size1];
			array.CopyTo(bytes, 0);

			return bytes;
		}

		public static bool[] FromBinary(byte[] bytes, int length)
		{
			BitArray array = new(bytes);

			List<bool> bools = [];

			int index = 0;

			foreach (bool bl in array)
			{
				if (index < length)
				{
					bools.Add(bl);
				}

				index++;
			}

			return [.. bools];
		}
	}
}
