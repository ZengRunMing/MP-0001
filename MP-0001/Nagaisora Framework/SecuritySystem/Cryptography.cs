using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace NagaisoraFramework.SecuritySystem
{
    public static class MD5Cryptography
	{
		public static readonly MD5 MD5;
		
		static MD5Cryptography()
		{
			MD5 = MD5.Create();
		}

        public static byte[] MD5Encrypt16Byte(string IN)
        {
			return MD5.ComputeHash(Encoding.UTF8.GetBytes(IN));
        }

        public static byte[] MD5Encrypt16Byte(byte[] IN)
        {
            return MD5.ComputeHash(IN);
        }
    }

	public class RSACryptography : IDisposable
	{
		public RSACryptoServiceProvider RSA;

		public string PublicKey;
		public string PrivateKey;

		public RSACryptography(int keylength)
		{
			RSA = new();
			CreateKey(keylength);
		}

		public void CreateKey(int keyLength)
		{
			RSA.KeySize = keyLength;

			PublicKey = RSA.ToXmlString(false);
			PrivateKey = RSA.ToXmlString(true);
		}

		public byte[] Encrypt(byte[] data, string publickey = null)
		{
			if (publickey == null || publickey == "")
			{
				publickey = PublicKey;
			}

			RSA.FromXmlString(publickey);
			byte[] cipherBytes = RSA.Encrypt(data, false);

			return cipherBytes;
		}

		public byte[] Decrypt(byte[] data, string privatekey = null)
		{
			if (privatekey == null || privatekey == "")
			{
				privatekey = PrivateKey;
			}

			RSA.FromXmlString(privatekey);
			byte[] cipherBytes = RSA.Decrypt(data, false);

			return cipherBytes;
		}

		public byte[] EncryptExpand(byte[] data, string publickey = null)
		{
			if (data == null)
			{
				return null;
			}

			int length = data.Length / 117;
			int remain = data.Length % 117;

			MemoryStream memory0 = new(data);
			MemoryStream memory1 = new();

			for (int i = 0; i < length; i++)
			{
				byte[] d = new byte[117];

				memory0.Read(d, 0, d.Length);

				byte[] Out = Encrypt(d, publickey);

				memory1.Write(Out, 0, Out.Length);
			}

			if (remain > 0)
			{
				byte[] rd = new byte[remain];

				memory0.Read(rd, 0, rd.Length);

				byte[] ROut = Encrypt(rd, publickey);

				memory1.Write(ROut, 0, ROut.Length);
			}

			memory0.Close();

			return memory1.ToArray();
		}

		public byte[] DecryptExpand(byte[] data, string privatekey = null)
		{
			if (data == null)
			{
				return null;
			}

			int length = data.Length / (117 + 11);
			int remain = data.Length % (117 + 11);

			MemoryStream memory0 = new(data);
			MemoryStream memory1 = new();

			for (int i = 0; i < length; i++)
			{
				byte[] d = new byte[117 + 11];

				memory0.Read(d, 0, d.Length);

				byte[] Out = Decrypt(d, privatekey);

				memory1.Write(Out, 0, Out.Length);
			}

			if (remain > 0)
			{
				byte[] rd = new byte[remain];

				memory0.Read(rd, 0, rd.Length);

				byte[] ROut = Decrypt(rd, privatekey);

				memory1.Write(ROut, 0, ROut.Length);
			}

			memory0.Close();

			return memory1.ToArray();
		}

		public void Dispose()
		{
			RSA.Dispose();
		}
	}

	public static class DESCryptography
	{
		public static readonly TripleDES DES;

		static DESCryptography()
		{
			DES = TripleDES.Create();
		}

		public static string Encrypt(string str, byte[] key)
		{
			return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(str), key));
		}

		public static byte[] Encrypt(byte[] data, byte[] key)
		{
			MemoryStream memory = new();
			CryptoStream cryptoStream = new(memory, DES.CreateEncryptor(key, key), CryptoStreamMode.Write);
			
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
			
			return memory.ToArray();
		}

		public static string DESDecrypt(string str, byte[] key)
		{
			return Encoding.UTF8.GetString(DESDecrypt(Convert.FromBase64String(str), key));
		}

		public static byte[] DESDecrypt(byte[] data, byte[] key)
		{
			MemoryStream memory = new();
			
			CryptoStream cryptoStream = new(memory, DES.CreateDecryptor(key, key), CryptoStreamMode.Write);
			
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();

			return memory.ToArray();
		}
	}

	//----------------------------------------------------
	//恩尼格玛随机步进加密系统
	//未完工
	//----------------------------------------------------
	public class EnigmaRandomStepCryptography
	{
		public class EnigmaTurntable
		{
			public List<byte> List0;
			public List<byte> List1;

			public EnigmaTurntable(int seed)
			{
				Random random = new(seed);

				List0 = [];
				List1 = [];

				for (int i = 0; i < 255; i++)
				{
					bool GenerateOut = false;

					Generate:
					byte a = (byte)random.Next(byte.MinValue, byte.MaxValue);

					if (GenerateOut)
					{
						goto GenerateO;
					}

					if (List0.Contains(a))
					{
						goto Generate;
					}
					List0.Add(a);
					GenerateOut = true;
					goto Generate;

					GenerateO:
					if (List1.Contains(a))
					{
						goto Generate;
					}
					List1.Add(a);
				}
			}

			public byte Encrypt(byte input)
			{
				int index = List0.IndexOf(input);
				return List1[index];
			}

			public byte Decrypt(byte input)
			{
				int index = List1.IndexOf(input);
				return List0[index];
			}
		}

		public static List<List<EnigmaTurntable>> lists;

		public Random Random;

		/// <summary>
		/// 初始化函数
		/// </summary>
		/// <param name="digit">转盘的位数, 最大255位转盘, 默认8位</param>
		public EnigmaRandomStepCryptography(byte digit = 8)
		{
			Random = new();

			for (int i = 0; i < digit; i++)
			{

			}
		}

		public int GetRandomKey()
		{
			int key = Random.Next();

			SetKey(key);

			return key;
		}

		public void SetKey(int key)
		{
			Random = new(key);
		}

		public byte[] Encrypt(byte[] binary)
		{
			return null;
		}

		public byte[] Decrypt(byte[] bianry)
		{
			return null;
		}
	}
}