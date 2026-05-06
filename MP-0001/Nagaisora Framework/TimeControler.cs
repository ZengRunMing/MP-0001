using System;

namespace NagaisoraFramework
{
	public class TimeControler
	{
		public bool IsRunning;
	
		public TimeSpan StartTime;
		public TimeSpan CacheDuration;

		public TimeSpan Elapsed => IsRunning ? MainSystem.ClockSystem.SystemTime - StartTime + CacheDuration : CacheDuration;

		public void Start()
		{
			StartTime = MainSystem.ClockSystem.SystemTime;
			IsRunning = true;
		}

		public void Stop()
		{
			CacheDuration = Elapsed;
			IsRunning = false;
		}

		public void Reset()
		{
			StartTime = TimeSpan.Zero;
			CacheDuration = TimeSpan.Zero;
			IsRunning = false;
		}
	}
}
