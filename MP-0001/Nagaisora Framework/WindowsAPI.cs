using Microsoft.Win32.SafeHandles;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace NagaisoraFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MEMORYSTATUSEX
	{
		public uint DWLength; //当前结构体大小
		public uint DWMemoryLoad; //当前内存使用率
		public ulong ullTotalPhys; //总计物理内存大小
		public ulong ullAvailPhys; //可用物理内存大小
		public ulong ullTotalPagefile; //总计交换文件大小
		public ulong ullAvailPagefile; //总计交换文件大小
		public ulong ullTotalVirtual; //总计虚拟内存大小
		public ulong ullAvailVirtual; //可用虚拟内存大小
		public ulong ullAvailExtendedVirtual; //保留 这个值始终为0
	}

	public static class WindowsAPI
	{
		[DllImport("kernel32.dll")]
		public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string FileName, AccessFlag accessFlag, ShareMode shareMode, IntPtr securityAttr, CreateMode createMode, uint flagsAndAttributes, IntPtr templateFile);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern uint SetFilePointer([In] SafeFileHandle hFile, [In] long lDistanceToMove, IntPtr lpDistanceToMoveHigh, [In] EMoveMethod dwMoveMethod);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetFilePointerEx([In] SafeFileHandle hFile, [In] long lDistanceToMove, IntPtr lpDistanceToMoveHigh, [In] EMoveMethod dwMoveMethod);
		[DllImport("kernel32.dll")]
		public static extern SafeFileHandle OpenProcess(uint flag, bool ihh, int processid);
		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(SafeFileHandle handle, int address, int[] buffer, int size, int[] nor);
		[DllImport("kernel32.dll")]
		public static extern SafeFileHandle WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, int size, out IntPtr lpNumberOfBytesWritten);
		[DllImport("kernel32", EntryPoint = "CreateRemoteThread")]
		public static extern int CreateRemoteThread(int hProcess, int lpThreadAttributes, int dwStackSize, int lpStartAddress, int lpParameter, int dwCreationFlags, ref int lpThreadId);
		[DllImport("Kernel32.dll")]
		public static extern SafeFileHandle VirtualAllocEx(IntPtr hProcess, int lpAddress, int dwSize, short flAllocationType, short flProtect);
		[DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
		public static extern int CloseHandle(int hObject);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hObject);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool SetWindowTextW(IntPtr hwnd, string title);
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string className, string windowName);
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("shell32.dll")]
		public static extern IntPtr ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);
		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

		public static void ChangeIcon(IntPtr hWnd, Icon icon)
		{
			SendMessage(hWnd, 0x80, 0, icon.Handle);
			SendMessage(hWnd, 0x80, 1, icon.Handle);
		}

		public const uint GENERIC_READ = 0x80000000;
		public const uint GENERIC_WRITE = 0x40000000;
		public const uint FILE_SHARE_READ = 0x00000001;
		public const uint FILE_SHARE_WRITE = 0x00000002;
		public const uint OPEN_EXISTING = 3;
		public const uint FILE_ATNFATIBUTE_NORMAL = 0x00000080;

		public static List<ManagementObject> GetManagementObjects(WMIPath path)
		{
			ManagementObjectSearcher searcher = new($"SELECT * FROM {path}");

			List<ManagementObject> list = [];

			foreach (ManagementObject @object in searcher.Get().Cast<ManagementObject>())
			{
				list.Add(@object);
			}

			return list;
		}

		public static MEMORYSTATUSEX GetMEMORYSTATUSEX()
		{
			MEMORYSTATUSEX MEMORYSTATUSEX = new();
			MEMORYSTATUSEX.DWLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
			bool status = GlobalMemoryStatusEx(ref MEMORYSTATUSEX);
			if (status)
			{
				return MEMORYSTATUSEX;
			}
			else
			{
				throw new Exception("Get MEMORYSTATUSEX failed");
			}
		}
	}
}
