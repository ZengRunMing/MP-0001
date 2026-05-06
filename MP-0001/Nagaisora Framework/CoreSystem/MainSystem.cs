using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Godot;

namespace NagaisoraFramework
{
	using LogSystem;
	using MediaSystem;
	using STGSystem;

	public static class MainSystem
	{
		public static Window MainWindow => Window.GetFocusedWindow();

		public static BGMControler BGMControler;
		public static SoundEffectControler SoundEffectControler;

		public static InputSystem InputSystem;
		public static ClockSystem ClockSystem;
		public static SystemControler SystemControler;

		public delegate void KeyDownEvent(InputKey inputKey);
		public static event KeyDownEvent KeyDown;

		public delegate void TimeUpdateEvent(TimeSpan timeSpan);
		public static event TimeUpdateEvent GameTimeUpdate;

		public static event EventHandler OnApplicationQuit;

		public static Log Log;

		public static Dictionary<string, STGControler> STGControlers;

		public static int BGMCount
		{
			get
			{
				int WaveBGMCount = MainSystem.BGMControler.BGMPackData.WaveBGMDatas.Count;
				int MidiBGMCount = MainSystem.BGMControler.BGMPackData.MidiBGMDatas.Count;

				return WaveBGMCount >= MidiBGMCount ? WaveBGMCount : MidiBGMCount;
			}
		}

		public static int PlayerCount;
		public static int RankCount;

		static MainSystem()
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			STGControlers = [];
		}

		public static void InitLogSystem(string path)
		{
			OutLivel outLivel = OutLivel.Information;

			if (OS.IsDebugBuild())
			{
				outLivel = OutLivel.Debug;
			}

			Log = new(outLivel, path);

			OS.AddLogger(Log);
		}

		public static void CallKeyDown(ushort keys)
		{
			InputKey inputKey = new InputKey();
			inputKey.FromBinary(keys);

			CallKeyDown(inputKey);
		}

		public static void CallKeyDown(InputKey inputKey)
		{
			KeyDown?.Invoke(inputKey);
		}

		public static void CallGameTimeUpdate(TimeSpan timeSpan)
		{
			GameTimeUpdate?.Invoke(timeSpan);
		}

		public static void SetTimeScale(float i)
		{
			Engine.PhysicsTicksPerSecond = (int)(60 / i);
		}

		public static void SetResolution()
		{
			Window.ModeEnum mode = Window.ModeEnum.Windowed;

			if (GlobalData.ConfigData.DrawMode == 1)
			{
				mode = Window.ModeEnum.Maximized;
			}
			else if (GlobalData.ConfigData.DrawMode == 2)
			{
				mode = Window.ModeEnum.Fullscreen;
			}
			else if (GlobalData.ConfigData.DrawMode == 3)
			{
				mode = Window.ModeEnum.ExclusiveFullscreen;
			}

			GD.Print($"[Framework Kernel] Set display resolution target [{(int)GlobalData.ConfigData.ResolutionX}, {(int)GlobalData.ConfigData.ResolutionY}] {mode}");

			MainWindow.Size = new Vector2I((int)GlobalData.ConfigData.ResolutionX, (int)GlobalData.ConfigData.ResolutionY);
			MainWindow.Mode = mode;
		}

		public static STGControler RegeisterSTGControler(string name, KeyConfig keyConfig, Vector2I size, float boundaryMargin)
		{
			InputSystem inputSystem = new(keyConfig);
			ClockSystem clockSystem = new();

			STGControler controler = new(name, inputSystem, clockSystem, size, boundaryMargin);
			STGControlers.Add(name, controler);

			return controler;
		}

		public static Assembly LoadAssembly(byte[] binary)
		{
			return AppDomain.CurrentDomain.Load(binary);
		}

		public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName.Split(',')[0] == args.Name.Split(',')[0]);
		}

		public static void Quit()
		{
			OnApplicationQuit?.Invoke(null, null);

			Node node = new();
			node.GetTree().Quit();
		}
	}
}