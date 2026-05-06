using System.IO;

namespace NagaisoraFramework
{
	public class AddressMappedStream : Stream
	{
		protected Stream BaseStream;

		protected long MappedAddress;
		protected long MappedLength;

		public override bool CanRead => BaseStream.CanRead;

		public override bool CanSeek => BaseStream.CanSeek;

		public override bool CanWrite => BaseStream.CanWrite;

		public override long Length => MappedLength;

		public override long Position
		{
			get => BaseStream.Position - MappedAddress;
			set
			{
				if (value >= Length)
				{
					throw new IOException("out of range.");
				}

				BaseStream.Position = MappedAddress + value;
			}
		}

		public AddressMappedStream(Stream stream, long address, long length)
		{
			BaseStream = stream;
			MappedAddress = address;
			MappedLength = length;
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (Position + count >= Length)
			{
				throw new IOException("Out of Range");
			}

			return BaseStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			BaseStream.Write(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin is SeekOrigin.Begin)
			{
				if (offset < 0 || offset >= Length)
				{
					throw new IOException("Out of Range.");
				}
			}
			else if (origin is SeekOrigin.Current)
			{
				if (Position + offset < 0 || Position + offset >= Length)
				{
					throw new IOException("Out of Range.");
				}
			}
			else
			{
				if (Length + offset < 0 || offset > 0)
				{
					throw new IOException("Out of Range");
				}
			}

			BaseStream.Seek(MappedAddress + offset, origin);

			return Position;
		}

		public void SetAddress(long value)
		{
			MappedAddress = value;
		}

		public override void SetLength(long value)
		{
			MappedLength = value;
		}
	}
}
