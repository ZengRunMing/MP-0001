using System;
using System.Net;
using System.Net.Sockets;

namespace NagaisoraFramework.NetworkSystem
{
	public enum NetWorkType
	{
		Server,
		Client,
	}

	public delegate void ReceiveCallBackEvent(Socket socket, byte[] bytes);
	public delegate void AcceptCallBackEvent(Socket socket);
	public delegate void ConnectCallBackEvent(Socket socket);
	public delegate void ExceptionCallBackEvent(Exception exception);

	public class NetworkSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
	{
		public AddressFamily AddressFamily { get; set; } = addressFamily;
		public SocketType SocketType { get; set; } = socketType;
		public ProtocolType ProtocolType { get; set; } = protocolType;

		public NetWorkType NetWorkType { get; private set; }

		public Socket Socket { get; private set; }

		public void NewSocket()
		{
			Socket = new Socket(AddressFamily, SocketType, ProtocolType);
		}

		public Exception OpenServerSocket(IPAddress host, int port, int ListenCount)
		{
			NewSocket();

			NetWorkType = NetWorkType.Server;

			try
			{
				IPEndPoint EndPoint = new(host, port);
				Socket.Bind(EndPoint);

				Socket.Listen(ListenCount);
			}
			catch (Exception ex)
			{
				Close();
				return ex;
			}

			return null;
		}

		public Exception OpenClientSocket(IPAddress host, int port, int localport, int timeout)
		{
			NewSocket();

			NetWorkType = NetWorkType.Client;

			try
			{
				IPEndPoint EndPoint = new(IPAddress.Parse("0.0.0.0"), localport);
				Socket.Bind(EndPoint);
				Socket.Connect(host, port);
			}
			catch (Exception ex)
			{
				Close();
				return ex;
			}

			return null;
		}

		public void Close()
		{
			Socket?.Close();
			Socket?.Dispose();
			Socket = null;
		}
	}
}