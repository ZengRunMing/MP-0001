using Godot;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace NagaisoraFramework.DataSystem
{
	public class ImagePackData(string name)
	{
		public const string DataHead = "NIPD";
		public const uint Version = 0X00000001;

		public string Name = name;

		public ImageData this[string name] => ImageDatas[name];

		public Dictionary<string, ImageData> ImageDatas = [];

		public void Clear() => ImageDatas.Clear();

		public void Add(ImageData data) => ImageDatas.Add(data.Name, data);

		public void Add(string name, Image image) => ImageDatas.Add(name, new ImageData(name, image));

		public void Remove(string name) => ImageDatas.Remove(name);

		public byte[] ToBinary()
		{
			MemoryStream memory = new();
			BinaryWriter writer = new(memory, Encoding.ASCII, true);

			memory.Position = 0;

			writer.Write(DataHead.ToCharArray());
			writer.Write(Version);

			memory.Position = 16;

			writer.Write(Name.ToCharArray());

			memory.Position = 80;

			writer.Write(ImageDatas.Count);

			memory.Position = 512;

			foreach (var item in ImageDatas)
			{
				writer.Write(item.Value.ToBinary());
			}

			writer.Close();

			return memory.ToArray();
		}

		public static ImagePackData FromBinary(byte[] binary)
		{
			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.ASCII, false);

			memory.Position = 0;

			string ReadHead = new(reader.ReadChars(DataHead.Length));
			if (ReadHead != DataHead)
			{
				throw new InvalidDataException($"数据标识头不匹配");
			}

			uint ReadVersion = reader.ReadUInt32();
			if (ReadVersion != Version)
			{
				throw new InvalidDataException($"数据发行版本不匹配");
			}

			memory.Position = 16;

			byte[] hwData = reader.ReadBytes(64);
			string name = StringHelper.GetStringFromBinary(hwData, Encoding.ASCII);

			int count = reader.ReadInt32();

			memory.Position = 512;

			ImagePackData output = new(name);

			int i = 0;
			while (i < count)
			{
				ImageData imageData = ImageData.FromBinary(reader);
				output.Add(imageData);
				i++;
			}

			reader.Close();

			return output;
		}
	}

	public class ImageData
	{
		public string Name;

		public ImageTexture Texture = new();

		public SliceData SliceData;

		public ImageData(string name, Image image)
		{
			Name = name;
			Texture = ImageTexture.CreateFromImage(image);
			SliceData = new();
		}

		public ImageData(string name, Image image, SliceData sliceData)
		{
			Name = name;
			Texture = ImageTexture.CreateFromImage(image);
			SliceData = sliceData;
		}

		public byte[] ToBinary()
		{
			MemoryStream memory = new();
			BinaryWriter writer = new(memory, Encoding.ASCII, true);

			memory.Position = 0;

			writer.Write(Name.ToCharArray());

			memory.Position = 128;

		 	Image image = Texture.GetImage();

			byte[] imageData = image.GetData();
			byte[] sliceData = SliceData.ToBinary();

			int imageDataSectorCount = imageData.Length / 512;
			if (imageData.Length % 512 > 0)
			{
				imageDataSectorCount++;
			}

			int sliceDataSectorCount = sliceData.Length / 512;
			if (sliceData.Length % 512 > 0)
			{
				sliceDataSectorCount++;
			}

			writer.Write(imageDataSectorCount);
			writer.Write(sliceDataSectorCount);

			writer.Write(imageData.Length);
			writer.Write((int)image.GetFormat());
			
			writer.Write(image.GetWidth());
			writer.Write(image.GetHeight());

			memory.Position = 512;

			byte[] imageWriteData = new byte[imageDataSectorCount * 512];
			Array.Copy(imageData, imageWriteData, imageData.Length);
			writer.Write(imageWriteData);

			byte[] sliceWriteData = new byte[sliceDataSectorCount * 512];
			Array.Copy(sliceData, sliceWriteData, sliceData.Length);
			writer.Write(sliceWriteData);

			writer.Close();

			return memory.ToArray();
		}

		public static ImageData FromBinary(BinaryReader reader)
		{
			byte[] hwData = reader.ReadBytes(512);

			MemoryStream hmemory = new(hwData);
			BinaryReader hreader = new(hmemory, Encoding.ASCII, false);

			hmemory.Position = 0;

			byte[] nameData = hreader.ReadBytes(128);
			string name = StringHelper.GetStringFromBinary(nameData, Encoding.ASCII);

			int imageDataSectorCount = hreader.ReadInt32();
			int sliceDataSectorCount = hreader.ReadInt32();

			int imageDataSize = hreader.ReadInt32();
			Image.Format imageFormat = (Image.Format)hreader.ReadInt32();

			int imagewidth = hreader.ReadInt32();
			int imageheight = hreader.ReadInt32();

			hreader.Close();
			hmemory.Close();

			byte[] imageSourceData = reader.ReadBytes(imageDataSectorCount * 512);
			byte[] sliceSourceData = reader.ReadBytes(sliceDataSectorCount * 512);

			byte[] imageData = new byte[imageDataSize];
			Array.Copy(imageSourceData, imageData, imageData.Length);

			Image image = Image.CreateFromData(imagewidth, imageheight, false, imageFormat, imageData);

			SliceData sliceData = SliceData.FromBinary(sliceSourceData);

			return new(name, image, sliceData);
		}
	}

	public class SliceData()
	{
		public Rect2 this[string name] => SliceDatas[name];

		public int Count => SliceDatas.Count;

		public Dictionary<string, Rect2> SliceDatas = [];

		public void Clear() => SliceDatas.Clear();

		public void Add(string name, Rect2 rect) => SliceDatas.Add(name, rect);

		public void Remove(string name) => SliceDatas.Remove(name);

		public byte[] ToBinary()
		{
			MemoryStream memory = new();
			BinaryWriter writer = new(memory, Encoding.ASCII, true);

			memory.Position = 0;

			foreach (var item in SliceDatas)
			{
				string name = item.Key;

				writer.Write(name.ToCharArray());

				memory.Position = 16;

				writer.Write(item.Value.Position.X);
				writer.Write(item.Value.Position.Y);

				writer.Write(item.Value.Size.X);
				writer.Write(item.Value.Size.Y);
			}

			writer.Flush();

			if (memory.Length < 512 || memory.Length % 512 > 0)
			{
				byte[] endFlag = new byte[32];
				Array.Fill(endFlag, byte.MaxValue);
				writer.Write(endFlag);
			}

			writer.Close();

			return memory.ToArray();
		}

		public static SliceData FromBinary(byte[] binary)
		{
			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.ASCII, false);

			SliceData sliceData = new();

			while (memory.Position < memory.Length)
			{
				byte[] hwData = reader.ReadBytes(16);

				if (hwData.All(e => e == byte.MaxValue))
				{
					break;
				}

				string name = StringHelper.GetStringFromBinary(hwData, Encoding.ASCII);

				float positionX = reader.ReadSingle();
				float positionY = reader.ReadSingle();

				float width = reader.ReadSingle();
				float height = reader.ReadSingle();

				sliceData.Add(name, new Rect2(positionX, positionY, width, height));
			}

			reader.Close();

			return sliceData;
		}
	}
}
