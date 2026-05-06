using System;
using System.Collections.Generic;
using System.Reflection;

using Godot;

namespace NagaisoraFramework
{
	using DataSystem;
	using NagaisoraFramework.MediaSystem;
	using System.IO;
	using System.Management;

	[GlobalClass]
	public partial class BootLoader : Node
	{
		public string[] arguments;

		//public GameObject TerminalGroup;

		public override void _Ready()
		{
			MainSystem.InitLogSystem(Path.Combine(DataPathHelper.LocalPath, "engine.log"));

			PrintFrameworkInformation();
			PrintOperatingEnvironmentInformation();
			PrintPhysicalDiskInformation();

			GlobalData.ConfigData = ConfigData.Default;
			MainSystem.SetResolution();

			GD.Print($"[Framework Kernel] Initialization random nubmber system");
			RandomSystem.Initialize();

			GD.Print($"[Framework Kernel] Apply key config");
			InputSystem.SetGUIKeyConfig(GlobalData.ConfigData.KeyConfigs["default"]);

			Boot();
		}

		public static void PrintFrameworkInformation()
		{
			Assembly FrameworkBaseAssembly = typeof(MainSystem).Assembly;
			AssemblyName FrameworkAssemblyName = FrameworkBaseAssembly.GetName();

			GD.Print($"{FrameworkAssemblyName.Name} [Version {FrameworkAssemblyName.Version}]");
		}

		public static void PrintOperatingEnvironmentInformation()
		{
			GD.Print($"[Framework Kernel] [{OS.GetModelName}] {OS.GetName}");

			ManagementObject[] cpus = [.. WindowsAPI.GetManagementObjects(WMIPath.Win32_Processor)];
			GD.Print($"[Framework Kernel] Finded {cpus.Length} CPUS");

			int i = 0;
			foreach (ManagementObject cpu in cpus)
			{
				GD.Print($"[Framework Kernel] CPU{i} -> {cpu["Name"]} CPUID:{cpu["ProcessorId"]}");
				i++;
			}

			MEMORYSTATUSEX MEMORYSTATUSEX = WindowsAPI.GetMEMORYSTATUSEX();

			GD.Print($"[Framework Kernel] {MEMORYSTATUSEX.DWMemoryLoad} percent of memory in use");
			GD.Print($"[Framework Kernel]     Total physics memory: {StringHelper.GetFileSizeString(MEMORYSTATUSEX.ullTotalPhys)}");
			GD.Print($"[Framework Kernel]      Free physics memory: {StringHelper.GetFileSizeString(MEMORYSTATUSEX.ullAvailPhys)}");
			GD.Print($"[Framework Kernel] Total paging file memory: {StringHelper.GetFileSizeString(MEMORYSTATUSEX.ullTotalPagefile)}");
			GD.Print($"[Framework Kernel]  Free paging file memory: {StringHelper.GetFileSizeString(MEMORYSTATUSEX.ullAvailPagefile)}");
			GD.Print($"[Framework Kernel]     Total virtual memory: {StringHelper.GetFileSizeString(MEMORYSTATUSEX.ullTotalVirtual)}");
			GD.Print($"[Framework Kernel]      Free virtual memory: {StringHelper.GetFileSizeString(MEMORYSTATUSEX.ullAvailVirtual)}");
			GD.Print($"[Framework Kernel]       Free Extend memory: {StringHelper.GetFileSizeString(MEMORYSTATUSEX.ullAvailExtendedVirtual)}");

			GD.Print($"[Framework Kernel] MainWindowHWND: 0x{MainSystem.MainWindow.NativeInstance:X16}");
		}

		public static void PrintPhysicalDiskInformation()
		{
			ManagementObject[] objects = [.. WindowsAPI.GetManagementObjects(WMIPath.Win32_DiskDrive)];

			GD.Print($"[Framework Kernel] Finded {objects.Length} disks");

			foreach (ManagementObject drive in objects)
			{
				string interfaceType = (string)drive["InterfaceType"] ?? "NULL";
				string size = StringHelper.GetFileSizeString(Convert.ToUInt64(drive["Size"]));

				GD.Print($"[Framework Kernel] {drive["Name"]} -> Model:{drive["Model"]} SID:{drive["SerialNumber"].ToString().TrimEnd('.')} InterfaceType:{interfaceType} Size:{size}");
			}
		}

		public void Boot()
		{
			arguments = OS.GetCmdlineArgs();

			if (arguments is null || arguments.Length <= 0)
			{
				JumpTerminal();
				return;
			}

			Queue<string> argumentqueue = new Queue<string>(arguments);

			while (argumentqueue.Count > 0)
			{
				string command = argumentqueue.Dequeue();

				switch (command)
				{
					default:
						JumpTerminal();
						return;
				}
			}
		}

		public void JumpTerminal()
		{
			//TerminalGroup.SetActive(true);
		}
	}
}

