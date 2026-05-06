using System;
using Godot;

namespace NagaisoraFramework.MediaSystem
{
	using DataSystem;

	[GlobalClass]
	public partial class AudioControler : AudioStreamPlayer
	{
		[Export]
		public float Volume;

		[Export]
		public float ExitVolumeOffset;

		public TimeSpan PlayStartTime;
		public TimeSpan LoopStartTime;
		public TimeSpan LoopEndTime;

		[Export]
		public bool AudioExit;

		public float PlayStartSeconds => (float)PlayStartTime.TotalSeconds;
		public float LoopStartSeconds => (float)LoopStartTime.TotalSeconds;
		public float LoopEndSeconds => (float)LoopEndTime.TotalSeconds;

		public float Time => GetPlaybackPosition();

		public override void _Process(double delta)
		{
			if (LoopEndTime.Ticks < LoopStartTime.Ticks)
			{
				LoopStartTime = TimeSpan.Zero;
				LoopEndTime = TimeSpan.FromSeconds(Stream.GetLength());
			}
			else
			{
				if (LoopEndTime.Ticks != 0)
				{
					if (Time >= LoopEndSeconds)
					{
						Seek(LoopStartSeconds);
					}
				}
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			if (AudioExit)
			{
				if (VolumeLinear > 0f)
				{
					VolumeLinear -= ExitVolumeOffset;
				}
				else
				{
					Stop();
					AudioExit = false;
				}
			}
			else
			{
				VolumeLinear = Volume;
			}
		}

		public void SetAudioData(WaveBGMData Data)
		{
			Stream = Data.AudioStreamWav;
			PlayStartTime = Data.StartTime;
			LoopStartTime = Data.LoopStartTime;
			LoopEndTime = Data.LoopEndTime;
		}

		public void Play()
		{
			Seek(PlayStartSeconds);
			StreamPaused = false;
			base.Play();
		}

		public void Pause()
		{
			StreamPaused = true;
		}

		public new void Stop()
		{
			StreamPaused = false;
			base.Stop();
		}

		public void Exit()
		{
			AudioExit = true;
		}
	}
}
