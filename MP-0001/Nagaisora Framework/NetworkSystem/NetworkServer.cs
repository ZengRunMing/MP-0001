using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NagaisoraFramework.NetworkSystem
{
	public class NetworkServer : NetworkSocket
	{
		public IPAddress IPAddress;
		public int Port;

		public int ListenCount;

		public int BufferSize = 1024;

		public bool IsListening { get; private set; } = false;

		public event AcceptCallBackEvent Accept;
		public event ReceiveCallBackEvent Receive;

		public event ExceptionCallBackEvent Exception;

		public byte[] dataBuffer;

		public Dictionary<Socket, int> Clients;

		public NetworkServer() : this(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
		{

		}

		public NetworkServer(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
		{
			Clients = [];
			dataBuffer = new byte[BufferSize];
		}

		public Exception StartSocket()
		{
			Clients.Clear();

			Exception exception = OpenServerSocket(IPAddress, Port, ListenCount);

			if (exception != null)
			{
				IsListening = false;
				return exception;
			}

			Socket.BeginAccept(AcceptCallBack, null);

			IsListening = true;
			return null;
		}

		public void CloseSocket()
		{
			Close();
			IsListening = false;
		}

		public virtual void AcceptCallBack(IAsyncResult result)
		{
			try
			{
				if (Socket == null)
				{
					return;
				}

				Socket clientSocket = Socket.EndAccept(result);
				Socket.BeginAccept(AcceptCallBack, null);

				Accept?.Invoke(clientSocket);
				BaseAccept(clientSocket);

				if (clientSocket == null || !clientSocket.Connected)
				{
					return;
				}

				Clients.Add(clientSocket, 0);

				clientSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, ReceiveCallBack, clientSocket);
			}
			catch (Exception ex)
			{
				Exception?.Invoke(ex);
			}
		}

		public virtual void ReceiveCallBack(IAsyncResult result)
		{
			try
			{
				Socket clientSocket = result.AsyncState as Socket;

				int count = clientSocket.EndReceive(result);

				if (count == 0)
				{
					clientSocket.Close();
					return;
				}

				Receive?.Invoke(clientSocket, [.. dataBuffer.Take(count)]);
				BaseReceive(clientSocket, [.. dataBuffer.Take(count)]);

				clientSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, ReceiveCallBack, clientSocket);
			}
			catch (Exception ex)
			{
				Exception?.Invoke(ex);
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
