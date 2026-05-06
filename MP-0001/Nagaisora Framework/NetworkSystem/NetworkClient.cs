using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NagaisoraFramework.NetworkSystem
{
	public class NetworkClient : NetworkSocket
	{
		public IPAddress IPAddress;
		public int Port;
		public int LocalPort;

		public int Timeout;

		public int BufferSize = 1024;

		public bool IsConnected { get; private set; } = false;

		public event ReceiveCallBackEvent Receive;

		public event ExceptionCallBackEvent Exception;

		public byte[] dataBuffer;

		public NetworkClient() : this(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
		{

		}

		public NetworkClient(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
		{
			dataBuffer = new byte[BufferSize];
		}

		public Exception StartSocket()
		{
			Exception exception = OpenClientSocket(IPAddress, Port, LocalPort, Timeout);

			if (exception != null)
			{
				IsConnected = false;
				return exception;
			}

			IsConnected = true;

			BaseAccept(Socket);

			if (Socket == null || !Socket.Connected)
			{
				return null;
			}

			Socket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, ReceiveCallBack, Socket);
			return null;
		}

		public void CloseSocket()
		{
			Close();
			IsConnected = false;
		}

		public void ReceiveCallBack(IAsyncResult result)
		{
			try
			{
				Socket socket = result.AsyncState as Socket;
				int count = socket.EndReceive(result);

				if (count == 0)
				{
					socket.Close();
					return;
				}

				BaseReceive(socket, [.. dataBuffer.Take(count)]);
				Receive?.Invoke(socket, [.. dataBuffer.Take(count)]);

				socket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, ReceiveCallBack, socket);
			}
			catch (Exception e)
			{
				Exception?.Invoke(e);
			}
		}

		public virtual void BaseAccept(Socket socket)
		{

		}

		public virtual void BaseReceive(Socket socket, byte[] bytes)
		{

		}
	}
}
