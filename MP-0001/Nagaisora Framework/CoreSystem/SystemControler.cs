using Godot;

using System;
using System.Diagnostics;

namespace NagaisoraFramework
{
	[GlobalClass]
	public partial class SystemControler : Node
	{
		[Export]
		public float FrameRate;

		public override void _Process(double delta)
		{
			FrameRate = (float)Performance.GetMonitor(Performance.Monitor.TimeFps);
		}
	}
}