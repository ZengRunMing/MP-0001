using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Interaction;

using Godot;

namespace NagaisoraFramework.MediaSystem
{
	using DataSystem;

	[GlobalClass]
	public partial class MidiControler : Node
	{
		[Export]
		public int OutputDeviceID = 0;
		[Export]
		public string OutputDeviceName = "";

		public MidiFile MidiFile;
		public Playback Playback;
		public OutputDevice OutputDevice;

		public TimeSpan PlayStartTime;
		public TimeSpan LoopStartTime;
		public TimeSpan LoopEndTime;

		public ICollection<Note> MidiNotes;

		public TimeSpan CurrentTime(TimeSpanType type)
		{
			if (Playback is null)
			{
				return TimeSpan.Zero;
			}

			return Playback.GetCurrentTime(type) as MetricTimeSpan;
		}

		public TimeSpan DurationTime(TimeSpanType type)
		{
			if (Playback is null)
			{
				return TimeSpan.Zero;
			}

			return Playback.GetDuration(type) as MetricTimeSpan;
		}

		public override void _Process(double delta)
		{
			if (Playback is not null)
			{
				TimeSpan Time = CurrentTime(TimeSpanType.Metric);

				if (LoopEndTime.Ticks < LoopStartTime.Ticks)
				{
					Playback.Loop = true;
				}
				else
				{
					Playback.Loop = false;
					if (LoopEndTime.Ticks != 0)
					{
						if (Time.Ticks >= LoopEndTime.Ticks)
						{
							Playback.Stop();
							Playback.MoveToTime((MetricTimeSpan)LoopStartTime);
							Playback.Start();
						}
					}
				}
			}
		}

		public static InputDevice GetMidiInputDevice(int DeviceID)
		{
			InputDevice inputDevice = InputDevice.GetByIndex(DeviceID);
			return inputDevice;
		}

		public static OutputDevice GetMidiOutputDevice(int DeviceID)
		{
			if (DeviceID < 0)
			{
				return null;
			}

			OutputDevice outputDevice = OutputDevice.GetByIndex(DeviceID);
			return outputDevice;
		}

		public void SetOutputDevice(int DeviceID)
		{
			OutputDevice?.Dispose();
			OutputDeviceID = DeviceID;
			if (DeviceID < 0)
			{
				OutputDevice = null;
				return;
			}

			OutputDevice = GetMidiOutputDevice(DeviceID);
		}

		public void SetOutputDevice(string name)
		{
			OutputDevice?.Dispose();
			OutputDeviceName = name;
			if (name == "")
			{
				OutputDevice = null;
				return;
			}

			OutputDevice = OutputDevice.GetByName(name);
		}

		public static void DisposeOutputDevice(OutputDevice device)
		{
			device?.Dispose();
		}

		public static void DisposeInputDevice(InputDevice device)
		{
			device?.Dispose();
		}

		public static string[] ListMidiInDevices()
		{
			List<string> MIN = [];
			foreach (InputDevice inputDevice in InputDevice.GetAll())
			{
				MIN.Add(inputDevice.Name);
			}
			return [.. MIN];
		}

		public static OutputDevice[] ListMidiOutDevices()
		{
			List<OutputDevice> MOT = [.. OutputDevice.GetAll()];
			return [.. MOT];
		}

		public static string[] ListMidiOutDeviceNames()
		{
			List<string> MOT = [];
			foreach (OutputDevice outputDevice in OutputDevice.GetAll())
			{
				MOT.Add(outputDevice.Name);
			}
			return [.. MOT];
		}

		public void SetMidiFile(string Name)
		{
			MidiFile = null;
			MidiFile = MidiFile.Read(Name);
			MidiNotes = MidiFile.GetNotes();
		}

		public void SetMidiData(MidiBGMData data)
		{
			PlayStartTime = data.StartTime;
			LoopStartTime = data.LoopStartTime;
			LoopEndTime = data.LoopEndTime;
			SetMidiData(data.MidiData);
		}

		public void SetMidiData(MemoryStream Stream)
		{
			MidiFile = null;

			ReadingSettings readingSettings = new()
			{
				InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits,
				TextEncoding = Encoding.GetEncoding("Shift_JIS"),
			};

			MemoryStream memory = new(Stream.ToArray());
			MidiFile = MidiFile.Read(memory, readingSettings);

			MidiNotes = MidiFile.GetNotes();
		}

		public void Play()
		{
			CreatePlayback();

			Playback?.Start();
		}

		public void CreatePlayback()
		{
			if (Playback is null)
			{
				Playback = MidiFile.GetPlayback();
				Playback.OutputDevice = OutputDevice;
				Playback.Speed = 1f;
				Playback.Loop = false;
			}
		}

		public void Stop()
		{
			Playback?.Stop();
			Playback?.Dispose();
			Playback = null;

			OutputDevice?.Dispose();
		}

		public void Pause()
		{
			Playback?.Stop();
		}

		public new void Dispose()
		{
			Playback?.Stop();
			Playback?.Dispose();
			Playback = null;
			OutputDevice?.Dispose();
			MidiFile = null;

			base.Dispose();
		}
	}
}
