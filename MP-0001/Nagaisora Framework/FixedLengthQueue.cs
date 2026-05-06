using System.Collections.Generic;

namespace NagaisoraFramework
{
	public class FixedLengthQueue<T> : Queue<T>
	{
		public int Length = 1;

		public FixedLengthQueue(int length) : base(length)
		{
			Length = length;
		}

		public new void Enqueue(T item)
		{
			if (Count >= Length)
			{
				base.Dequeue();
			}

			base.Enqueue(item);
		}

		public new void Dequeue() => base.Dequeue();
	}
}
