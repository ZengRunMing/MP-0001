using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace NagaisoraFramework
{
	[Serializable]
	public class MemoryMappedViewAccessorStream
	{
		public MemoryMappedViewAccessor ViewAccessor;

		public long Position = 0;

		public MemoryMappedViewAccessorStream(MemoryMappedViewAccessor viewAccessor)
		{
			ViewAccessor = viewAccessor;
		}

		public void Seek(long value, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					Position = 0 + value;
					break;
				case SeekOrigin.Current:
					Position += value;
					break;
				case SeekOrigin.End:
					Position -= value;
					break;
			}
		}
	}
}