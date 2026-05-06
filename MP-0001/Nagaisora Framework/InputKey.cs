using System;
using System.Collections;

namespace NagaisoraFramework
{
	public class InputKey
	{
		public bool Up;
		public bool Down;
		public bool Left;
		public bool Right;

		public bool Shoot;
		public bool Bomb;

		public bool Slow;

		public ushort ToBinary()
		{
			BitArray bitArray = new BitArray(16);

			WriteBitarray(bitArray);

			byte[] data = new byte[bitArray.Count / 8];
			bitArray.CopyTo(data, 0);

			return BitConverter.ToUInt16(data, 0);
		}

		public void FromBinary(ushort i)
		{
			BitArray bitArray = new BitArray(BitConverter.GetBytes(i));

			ReadBitarray(bitArray);
		}

		public virtual void WriteBitarray(BitArray bitArray)
		{
			Up = bitArray[0];
			Down = bitArray[1];
			Left = bitArray[2];
			Right = bitArray[3];
			Slow = bitArray[4];
			Shoot = bitArray[5];
			Bomb = bitArray[6];
		}

		public virtual void ReadBitarray(BitArray bitArray)
		{
			bitArray[0] = Up;
			bitArray[1] = Down;
			bitArray[2] = Left;
			bitArray[3] = Right;
			bitArray[4] = Slow;
			bitArray[5] = Shoot;
			bitArray[6] = Bomb;
		}
	}
}