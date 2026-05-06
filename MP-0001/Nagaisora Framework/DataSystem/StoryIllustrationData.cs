//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using UnityEngine;

//namespace NagaisoraFramework.DataSystem
//{
//	public class StoryIllustrationData
//	{
//		public const string DataHead = "NSID";
//		public const uint Version = 0X00000001;

//		public AssetBundleData LinkAssetBundle;
//		public Guid LinkAssetBundleGuid;

//		public Dictionary<uint, IllustrationSpriteData> Images;

//		public StoryIllustrationData(Guid linkAssetBundleGuid, IllustrationSpriteData[] data)
//		{
//			LinkAssetBundleGuid = linkAssetBundleGuid;
//			Images = new Dictionary<uint, IllustrationSpriteData>();

//			LinkAssetBundle = AssetBundleManager.FindAssetBundleData(LinkAssetBundleGuid);

//			foreach (IllustrationSpriteData item in data)
//			{
//				if (!(LinkAssetBundle is null))
//				{
//					item.GetSprite(LinkAssetBundle.AssetBundle);
//				}

//				Images.Add(item.Index, item);
//			}
//		}

//		public byte[] ToBinary()
//		{
//			MemoryStream memory = new MemoryStream();
//			BinaryWriter writer = new BinaryWriter(memory, Encoding.UTF8, true);

//			writer.Write(DataHead.ToCharArray());
//			writer.Write(Version);

//			writer.Write(Images.Count);

//			memory.Position = 16;

//			writer.Write(LinkAssetBundle.DataGuid.ToByteArray());

//			memory.Position = 512;

//			foreach (IllustrationSpriteData item in Images.Values)
//			{
//				writer.Write(item.ToBinary());
//			}

//			writer.Close();

//			return memory.ToArray();
//		}

//		public static StoryIllustrationData FromBinary(byte[] binary)
//		{
//			MemoryStream memory = new MemoryStream(binary);
//			BinaryReader reader = new BinaryReader(memory, Encoding.UTF8);

//			memory.Position = 0;

//			string ReadHead = new string(reader.ReadChars(DataHead.Length));

//			if (ReadHead != DataHead)
//			{
//				throw new InvalidDataException($"数据标识头不匹配");
//			}

//			uint ReadVersion = reader.ReadUInt32();

//			if (ReadVersion != Version)
//			{
//				throw new InvalidDataException($"数据发行版本不匹配");
//			}

//			uint count = reader.ReadUInt32();

//			memory.Position = 16;

//			Guid ReadGUID = new Guid(reader.ReadBytes(16));

//			memory.Position = 512;

//			IllustrationSpriteData[] datas = new IllustrationSpriteData[count];

//			for (uint i = 0; i < count; i++)
//			{
//				byte[] buffer = reader.ReadBytes(128);
//				datas[i] = IllustrationSpriteData.FromBinary(buffer);
//			}

//			StoryIllustrationData storyData = new StoryIllustrationData(ReadGUID, datas);
			
//			return storyData;
//		}
//	}

//	public class IllustrationSpriteData
//	{
//		public uint Index;

//		public string AssetName;

//		public Sprite Sprite;

//		public Vector2 Position;
//		public Vector2 Size;

//
//		public IllustrationSpriteData(uint index, Vector2 position, Vector2 size, string assetName, AssetBundle assetBundle = null)
//		{
//			Index = index;
//			Position = position;
//			Size = size;
//			AssetName = assetName;

//			if (assetBundle is null)
//			{
//				return;
//			}

//			GetSprite(assetBundle);
//		}

//		public void GetSprite(AssetBundle assetBundle)
//		{
//			Sprite = assetBundle.LoadAsset<Sprite>(AssetName);
//		}
//

//		public byte[] ToBinary()
//		{
//			MemoryStream memoryStream = new MemoryStream(128);
//			BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.UTF8, true);

//			writer.Write(Index);

//			memoryStream.Position = 16;

//			writer.Write(Position.x);
//			writer.Write(Position.y);
			
//			writer.Write(Size.x);
//			writer.Write(Size.y);

//			writer.Write(AssetName.ToCharArray());

//			writer.Close();

//			return memoryStream.ToArray();
//		}

//		public static IllustrationSpriteData FromBinary(byte[] binary)
//		{
//			if (binary.Length != 128)
//			{
//				throw new ArgumentException("Binary data length is not correct.");
//			}

//			MemoryStream memoryStream = new MemoryStream(binary);
//			BinaryReader reader = new BinaryReader(memoryStream, Encoding.UTF8);
			
//			uint index = reader.ReadUInt32();

//			memoryStream.Position = 16;

//			Vector2 position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
//			Vector2 size = new Vector2(reader.ReadSingle(), reader.ReadSingle());

//			byte[] namebuffer = reader.ReadBytes(96);
//			string name = MainSystem.GetStringFromBinary(namebuffer, Encoding.UTF8);

//			reader.Close();

//			IllustrationSpriteData data = new IllustrationSpriteData(index, position, size, name);

//			return data;
//		}
//	}
//}
