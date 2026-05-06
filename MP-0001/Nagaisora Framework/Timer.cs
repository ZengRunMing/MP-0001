using System;

namespace NagaisoraFramework
{
	public class Timer : TimeControler
	{
		public TimeSpan SetTime;
		public bool MultipleExecutions = false;

		public Action Action;

		public void OnUpdate()
		{
			if (Elapsed.Ticks < SetTime.Ticks)
			{
				return;
			}

			Reset();

			if (MultipleExecutions)
			{
				Start();
			}

			Action?.Invoke();
		}
	}
}
