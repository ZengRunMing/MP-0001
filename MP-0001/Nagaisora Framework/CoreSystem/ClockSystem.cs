using Godot;
using Godot.Collections;

using System;
using System.Diagnostics;

namespace NagaisoraFramework
{
	[Tool, GlobalClass]
	public partial class ClockSystem : Node
	{
		public bool IsRunning { get; private set; }

		public Stopwatch RunTimeStopwatch;

		public double TimeSeconds = 0;
		
		public int days;
		public int hours;
		public int minutes;
		public int seconds;
		public int milliseconds;

		public static DateTime DateTime => DateTime.Now;

		public TimeSpan SystemTime;
		public TimeSpan LastSystemTime;

		public TimeSpan RunTime;
		public TimeSpan TotalRunTime;
		
		public TimeSpan PlayerTime;

		public ulong UpdateTick { get; private set; }
		public ulong FixedUpdateTick { get; private set; }

		public delegate void Clock(ulong tick);

		public event Clock FixedUpdate;
		public event Clock Update;

		public override void _Ready()
		{
			if (Engine.IsEditorHint())
			{
				return;
			}

			RunTimeStopwatch = Stopwatch.StartNew();
		}

		public override void _PhysicsProcess(double delta)
		{
			if (Engine.IsEditorHint())
			{
				return;
			}

			if (!IsRunning) return;

			FixedUpdate?.Invoke(FixedUpdateTick);
			FixedUpdateTick++;
		}

		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint())
			{
				return;
			}

			RunTime = RunTimeStopwatch.Elapsed;
			TotalRunTime = GlobalData.ScoreData.TotalRunTime + RunTime;

			TimeSeconds += delta;

			days = (int)TimeSeconds / 86400;
			hours = (int)TimeSeconds / 3600;
			minutes = ((int)TimeSeconds - hours * 3600) / 60;
			seconds = (int)TimeSeconds - hours * 3600 - minutes * 60;
			milliseconds = (int)((TimeSeconds - (int)TimeSeconds) * 1000);

			SystemTime = new TimeSpan(days, hours, minutes, seconds, milliseconds);

			if (SystemTime.Ticks != LastSystemTime.Ticks)
			{
				LastSystemTime = SystemTime;
			}

			if (!IsRunning) return;

			Update?.Invoke(UpdateTick);
			UpdateTick++;
		}

		public void Start()
		{
			IsRunning = true;
		}

		public void Stop()
		{
			IsRunning = false;
		}

		public void Reset()
		{
			UpdateTick = 0;
			FixedUpdateTick = 0;
		}

		public override Variant _Get(StringName property)
		{
			if (property == nameof(SystemTime))
			{
				if (Engine.IsEditorHint())
				{
					return "00 -> 00:00:00.000";
				}
				return $"{SystemTime.Days:00} -> {SystemTime.Hours:00}:{SystemTime.Minutes:00}:{SystemTime.Seconds:00}:{SystemTime.Milliseconds:000}";
			}
			if (property == nameof(RunTime))
			{
				if (Engine.IsEditorHint())
				{
					return "00 -> 00:00:00.000";
				}
				return $"{RunTime.Days:00} -> {RunTime.Hours:00}:{RunTime.Minutes:00}:{RunTime.Seconds:00}:{RunTime.Milliseconds:000}";
			}
			if (property == nameof(TotalRunTime))
			{
				if (Engine.IsEditorHint())
				{
					return "00 -> 00:00:00.000";
				}
				return $"{TotalRunTime.Days:00} -> {TotalRunTime.Hours:00}:{TotalRunTime.Minutes:00}:{TotalRunTime.Seconds:00}:{TotalRunTime.Milliseconds:000}";
			}

			return base._Get(property);
		}

		public override bool _Set(StringName property, Variant value)
		{
			if (property == nameof(SystemTime) || property == nameof(RunTime) || property == nameof(TotalRunTime))
			{
				return true;
			}

			return base._Set(property, value);
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			return
			[
				new ()
				{
					{ "name", nameof(SystemTime) },
					{ "type", (int)Variant.Type.String },
				},
				new ()
				{
					{ "name", nameof(RunTime) },
					{ "type", (int)Variant.Type.String },
				},
				new ()
				{
					{ "name", nameof(TotalRunTime) },
					{ "type", (int)Variant.Type.String },
				},
			];
		}
	}
}
