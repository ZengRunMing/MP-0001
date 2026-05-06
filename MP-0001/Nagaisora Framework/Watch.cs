using System.Diagnostics;
using System.Threading;

namespace NagaisoraFramework
{
	public class Watch
	{
		private long startTime;

		public int FPS = 60;

		public void Start()
		{
			startTime = Stopwatch.GetTimestamp();
		}

		public float GetDuration()
		{
			if (startTime == 0)
			{
				return 0f;
			}
			return 1000f * (Stopwatch.GetTimestamp() - startTime) / Stopwatch.Frequency;
		}

		public void IncreaseOneFrame()
		{
			startTime += Stopwatch.Frequency / FPS;
		}

		public void SleepTo(float Time)
		{
			Time -= GetDuration();
			if (Time > 0f)
			{
				Thread.Sleep((int)Time);
			}
		}
	}
}
