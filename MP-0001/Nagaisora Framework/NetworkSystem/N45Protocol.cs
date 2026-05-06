using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace NagaisoraFramework
{
	using SecuritySystem;
	using NetworkSystem;

	using static N45Server;

	public enum N45Command
	{
		None = 0,
		SendInfo = 1,
		KeyExchange = 2,
		DataHeader = 3,
		Data = 4,
		Ping = 5,
	}

	public class N45Object
	{
		public N45Command Command;
		public byte[] Data;

		public N45Object()
		{
			Command = N45Command.None;
			Data = [];
		}

		public N45Object(N45Command command, byte[] data)
		{
			Command = command;
			Data = data;
		}

		public byte[] GetSendData()
		{
			MemoryStream memory = new();
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			writer.Write((short)Command);
			if (Data != null)
			{
				writer.Write(Data.Length);
				writer.Write(Data);
			}
			else
			{
				writer.Write(0);
			}

			return memory.ToArray();
		}

		public static N45Object GetObjectFormData(byte[] data)
		{
			MemoryStream memory = new(data);
			BinaryReader reader = new(memory, Encoding.UTF8, false);

			N45Command command = (N45Command)reader.ReadInt16();
			int length = reader.ReadInt32();
			byte[] tdata = reader.ReadBytes(length);

			N45Object n45Object = new()
			{
				Command = command,
				Data = tdata,
			};

			reader.Close();

			return n45Object;
		}
	}

	public class N45Server : NetworkServer
	{
		public string OldLocalKey;

		public int KeyLength;

		public Dictionary<Socket, N45ClientInfo> N45Clients;

		public delegate void Messsage(object Message);

		public new event Messsage Accept;
		public new event Messsage Receive;

		public Thread MessagingThread;

		public System.Threading.Timer Timer1S;
		public N45Server() : this(4096)
		{

		}

		public N45Server(int keylength)
		{
			KeyLength = keylength;
			N45Clients = [];

			Timer1S = new System.Threading.Timer(Timer1SMethod, null, 0, 1000);

			//MessagingThreads = new Thread[5];

			//for (int i = 0; i < MessagingThreads.Length; i++)
			//{
				MessagingThread = new(MessagingThreadMethod);
				MessagingThread.Start();
			//}
		}

		public void Timer1SMethod(object state)
		{
			try
			{
				if (N45Clients.Count == 0)
				{
					return;
				}

				foreach (var info in N45Clients)
				{
					info.Value.PingThreadMethod();
				}
			}
			catch
			{ }
		}

		public void MessagingThreadMethod()
		{
			while (true)
			{
				if (N45Clients.Count == 0)
				{
					continue;
				}

				Parallel.ForEach(N45Clients.ToArray(), (info) =>
				{
					lock (N45Clients)
					{
						info.Value.MessagingThreadMethod();
					}
				});
			}
		}

		public void Start()
		{
			N45Clients.Clear();

			Exception exception = StartSocket();

			if (exception != null)
			{
				throw exception;
			}
		}

		public new void Close()
		{
			CloseSocket();

			if (N45Clients.Count == 0)
			{
				return;
			}

			foreach (var info in N45Clients.ToArray())
			{
				info.Value.Dispose();
			}

			//foreach (Thread thread in MessagingThreads)
			//{
				MessagingThread.Abort();
			//}
		}

		public override void AcceptCallBack(IAsyncResult result)
		{
			if (Socket == null)
			{
				return;
			}

			Socket socket = Socket.EndAccept(result);
			Socket.BeginAccept(AcceptCallBack, null);

			Accept?.Invoke($"{socket.RemoteEndPoint} 连接至主站");

			N45ClientInfo info = new(KeyLength, BufferSize)
			{
				Server = this,
				BaseSocket = socket,
				EndPoint = socket.RemoteEndPoint,
			};


			Task task = new(
				() =>
				{
					Thread.Sleep(500);
					if (!N45Clients.ContainsKey(socket))
					{
						Accept?.Invoke($"{socket.RemoteEndPoint} 超时未登记信息, 拒绝连接");
						socket.Close();
						info.Dispose();
					}
				});
			task.Start();

			byte[] AcceptDataBuffer = new byte[BufferSize];

			int count = socket.Receive(AcceptDataBuffer);
			byte[] N45ObjData = AcceptDataBuffer.Take(count).ToArray();

			N45Object N45Obj = N45Object.GetObjectFormData(N45ObjData);

			if (N45Obj.Command != N45Command.KeyExchange)
			{
				socket.Close();
				return;
			}

			info.RSAKey = Encoding.UTF8.GetString(N45Obj.Data);

			N45Obj = new()
			{
				Command = N45Command.KeyExchange,
				Data = Encoding.UTF8.GetBytes(info.RSACryptography.PublicKey),
			};

			socket.Send(info.RSACryptography.EncryptExpand(N45Obj.GetSendData(), info.RSAKey));

			count = socket.Receive(AcceptDataBuffer);
			N45ObjData = [.. AcceptDataBuffer.Take(count)];

			N45ObjData = info.RSACryptography.DecryptExpand(N45ObjData);

			N45Obj = N45Object.GetObjectFormData(N45ObjData);

			if (N45Obj.Command != N45Command.SendInfo)
			{
				socket.Close();
				return;
			}

			MemoryStream memory = new(N45Obj.Data);
			BinaryReader reader = new(memory, Encoding.UTF8);

			info.HostName = reader.ReadString();

			reader.Close();
			memory.Close();

			N45Clients.Add(socket, info);

			Accept?.Invoke($"{socket.RemoteEndPoint} 信息登记完成");

			socket.BeginReceive(info.DataBuffer, 0, info.DataBuffer.Length, SocketFlags.None, ReceiveCallBack, socket);
		}

		public override void ReceiveCallBack(IAsyncResult result)
		{
			Socket clientSocket = result.AsyncState as Socket;

			int count = clientSocket.EndReceive(result);

			if (count == 0)
			{
				clientSocket.Close();
				return;
			}

			N45Clients[clientSocket].ReceiveMessagingQueue.Enqueue(N45Clients[clientSocket].DataBuffer.Take(count).ToArray());

			clientSocket.BeginReceive(N45Clients[clientSocket].DataBuffer, 0, N45Clients[clientSocket].DataBuffer.Length, SocketFlags.None, ReceiveCallBack, clientSocket);
		}

		public void ReceiveEvent(object obj)
		{
			Receive?.Invoke(obj);
		}
	}

	public class N45Client : NetworkClient, IDisposable
	{
		public RSACryptography RSACryptography;
		public RSACryptography NewRSACryptography;

		public int KeyLength;

		public string ServerKey;
		public string OldLocalKey;

		public Queue<byte[]> ReceiveMessagingQueue;
		public Queue<N45Object> SendMessagingQueue;
		public Thread MessagingThread;

		public event Messsage Accept;
		public new event Messsage Receive;

		public N45Client() : this(4096)
		{

		}

		public N45Client(int keylength)
		{
			KeyLength = keylength;

			ReceiveMessagingQueue = new Queue<byte[]>();
			SendMessagingQueue = new Queue<N45Object>();

			RSACryptography = new(KeyLength);

			MessagingThread = new Thread(MessagingThreadMethod);
			MessagingThread.Start();
		}

		public void Start()
		{
			Exception exception = StartSocket();

			if (exception != null)
			{
				Close();
				throw exception;
			}
		}

		public new void Close()
		{
			MessagingThread.Abort();
			CloseSocket();
		}

		public override void BaseAccept(Socket socket)
		{
			Accept?.Invoke("连接成功");

			base.BaseAccept(socket);

			N45Object keyobj = new()
			{
				Command = N45Command.KeyExchange,
				Data = Encoding.UTF8.GetBytes(RSACryptography.PublicKey),
			};

			socket.Send(keyobj.GetSendData());

			int count = socket.Receive(dataBuffer);
			byte[] N45ObjData = dataBuffer.Take(count).ToArray();

			N45ObjData = RSACryptography.DecryptExpand(N45ObjData);

			N45Object N45Obj = N45Object.GetObjectFormData(N45ObjData);

			if (N45Obj.Command != N45Command.KeyExchange)
			{
				socket.Close();
				return;
			}

			ServerKey = Encoding.UTF8.GetString(N45Obj.Data);

			MemoryStream memoryStream = new();
			BinaryWriter writer = new(memoryStream, Encoding.UTF8, true);

			writer.Write(Dns.GetHostName());

			N45Object n45Object = new()
			{
				Command = N45Command.SendInfo,
				Data = memoryStream.ToArray(),
			};

			socket.Send(RSACryptography.EncryptExpand(n45Object.GetSendData(), ServerKey));

			Accept?.Invoke("信息登记完成");
		}

		public override void BaseReceive(Socket socket, byte[] bytes)
		{
			base.BaseReceive(socket, bytes);

			ReceiveMessagingQueue.Enqueue(bytes);
		}


		public void MessagingThreadMethod()
		{
			while (true)
			{
				if (Socket == null || !Socket.Connected)
				{
					Dispose();
					return;
				}

				SendMessaging();
				ReceiveMessaging();
			}
		}

		public void SendMessaging()
		{
			if (SendMessagingQueue == null || SendMessagingQueue.Count == 0)
			{
				return;
			}

			N45Object Object = SendMessagingQueue.Dequeue();
			byte[] edata = RSACryptography.EncryptExpand(Object.GetSendData(), ServerKey);

			Socket.Send(edata);

			if (Object.Command == N45Command.KeyExchange)
			{
				RSACryptography.Dispose();
				RSACryptography = null;
				RSACryptography = NewRSACryptography;
			}
		}

		public void ReceiveMessaging()
		{
			if (ReceiveMessagingQueue == null || ReceiveMessagingQueue.Count == 0)
			{
				return;
			}

			byte[] data = ReceiveMessagingQueue.Dequeue();

			string key = RSACryptography.PrivateKey;

			byte[] RData;

			ReDecrypt:
			try
			{
				RData = RSACryptography.DecryptExpand(data, key);
			}
			catch (Exception ex)
			{
				if (key == OldLocalKey)
				{
					throw new Exception($"{ex} -> 密钥未能更新");
				}

				key = OldLocalKey;
				goto ReDecrypt;
			}

			N45Object Object = N45Object.GetObjectFormData(RData);

			switch (Object.Command)
			{
				case N45Command.KeyExchange:
					ServerKey = Encoding.UTF8.GetString(Object.Data);
					Receive?.Invoke($"主站要求更新密钥 正在处理...");

					NewRSACryptography = new(KeyLength);
					OldLocalKey = RSACryptography.PrivateKey;

					N45Object keyobj = new()
					{
						Command = N45Command.KeyExchange,
						Data = Encoding.UTF8.GetBytes(NewRSACryptography.PublicKey),
					};

					SendMessagingQueue.Enqueue(keyobj);
					Receive?.Invoke($"新密钥发送完毕");
					Receive?.Invoke($"主站密钥已更新为 {BitConverter.ToString(MD5Cryptography.MD5Encrypt16Byte(ServerKey)).Replace('-', ' ')}");
					break;
				case N45Command.Data:
					Receive?.Invoke($" <- {BitConverter.ToString(Object.Data).Replace('-', ' ')}");
					break;
				case N45Command.Ping:
					SendMessagingQueue.Enqueue(Object);
					break;
			}
		}

		public void Dispose()
		{
			MessagingThread.Abort();
			Socket?.Close();
			RSACryptography?.Dispose();
		}
	}

	public class N45ClientInfo : IDisposable
	{
		public N45Server Server;
		public Socket BaseSocket;

		public RSACryptography RSACryptography;
		public RSACryptography NewRSACryptography;

		public int KeyLength;

		public string RSAKey;
		public string OldLocalKey;

		public System.Threading.Timer KeyUpdateTimer;
		public Random KeyUpdateTimeRandom;
		public long NowTime = 0;

		public Queue<byte[]> ReceiveMessagingQueue;
		public Queue<N45Object> SendMessagingQueue;

		public byte[] DataBuffer;

		public Stopwatch PingStopwatch;

		public string HostName;
		public EndPoint EndPoint;

		public long Ping;

		public N45ClientInfo(int keyLength, int bufferSize)
		{
			KeyLength = keyLength;
			DataBuffer = new byte[bufferSize];

			RSACryptography = new(KeyLength);

			ReceiveMessagingQueue = new Queue<byte[]>();
			SendMessagingQueue = new Queue<N45Object>();

			KeyUpdateTimer = new System.Threading.Timer(UpdateKeyClick, null, 0, 1000);

			KeyUpdateTimeRandom = new Random();
			NowTime = KeyUpdateTimeRandom.Next(10, 30);

			PingStopwatch = new Stopwatch();
		}

		public void UpdateKeyClick(object state)
		{
			NowTime--;
			if (NowTime == 0)
			{
				NowTime = KeyUpdateTimeRandom.Next(10, 30);

				NewRSACryptography = new(KeyLength);
				OldLocalKey = RSACryptography.PrivateKey;

				N45Object keyobj = new()
				{
					Command = N45Command.KeyExchange,
					Data = Encoding.UTF8.GetBytes(NewRSACryptography.PublicKey),
				};

				SendMessagingQueue.Enqueue(keyobj);

				Console.WriteLine($"{EndPoint}更新密钥");
			}
		}

		public void BaseReceive(byte[] bytes)
		{
			ReceiveMessagingQueue.Enqueue(bytes);
		}

		public void MessagingThreadMethod()
		{
			if (BaseSocket == null || !BaseSocket.Connected)
			{
				Server.N45Clients.Remove(BaseSocket);
				Dispose();
				return;
			}

			SendMessaging();
			ReceiveMessaging();
		}

		public void PingThreadMethod()
		{
			N45Object obj = new()
			{
				Command = N45Command.Ping,
				Data = BitConverter.GetBytes(Ping),
			};

			SendMessagingQueue.Enqueue(obj);
		}

		public void SendMessaging()
		{
			if (SendMessagingQueue == null || SendMessagingQueue.Count == 0)
			{
				return;
			}

			N45Object Object = SendMessagingQueue.Dequeue();
			byte[] edata = RSACryptography.EncryptExpand(Object.GetSendData(), RSAKey);

			BaseSocket.Send(edata);

			switch (Object.Command)
			{
				case N45Command.KeyExchange:
					RSACryptography.Dispose();
					RSACryptography = null;
					RSACryptography = NewRSACryptography;
					break;
				case N45Command.Ping:
					PingStopwatch.Reset();
					PingStopwatch.Start();
					break;
			}
		}

		public void ReceiveMessaging()
		{
			if (ReceiveMessagingQueue == null || ReceiveMessagingQueue.Count == 0)
			{
				return;
			}

			byte[] data = ReceiveMessagingQueue.Dequeue();

			string key = RSACryptography.PrivateKey;

			byte[] RData;

			ReDecrypt:
			try
			{
				RData = RSACryptography.DecryptExpand(data, key);
			}
			catch (Exception ex)
			{
				if (key == OldLocalKey)
				{
					throw new Exception($"{ex} -> 密钥未能更新");
				}

				key = OldLocalKey;
				goto ReDecrypt;
			}

			N45Object Object = N45Object.GetObjectFormData(RData);

			switch (Object.Command)
			{
				case N45Command.None:

					break;
				case N45Command.KeyExchange:
					RSAKey = Encoding.UTF8.GetString(Object.Data);
					break;
				case N45Command.Data:
					SendMessagingQueue.Enqueue(Object);
					break;
				case N45Command.Ping:
					PingStopwatch.Stop();
					Ping = PingStopwatch.ElapsedMilliseconds;
					break;
				default:

					break;
			}
		}

		public void Dispose()
		{
			BaseSocket?.Close();
			RSACryptography?.Dispose();
			NewRSACryptography?.Dispose();
			KeyUpdateTimer?.Dispose();
			ReceiveMessagingQueue?.Clear();
			SendMessagingQueue?.Clear();
		}
	}
}
