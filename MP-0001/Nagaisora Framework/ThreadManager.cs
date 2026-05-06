using System.Collections;
using System.Threading;
using System;

namespace NagaisoraFramework
{
	public class ThreadManager
	{
		public int MaxWorkThreadCount, MaxCompletionThreadcCont;
		
		public int MinWorkThreadCount, MinCompletionThreadcCont;

		public ThreadManager()
		{
			ThreadPool.GetMaxThreads(out MaxWorkThreadCount, out MaxCompletionThreadcCont);
			ThreadPool.SetMinThreads(1001, 10);
			ThreadPool.GetMinThreads(out MinWorkThreadCount, out MinCompletionThreadcCont);// 默认最小值是cpu个数
			//ThreadPool.QueueUserWorkItem(new WaitCallback(count), null);
		}

		public (int, int) GetAvailableThreads()
		{
			ThreadPool.GetAvailableThreads(out int WorkThreadCount, out int CompletionThreadcCont);
			return (WorkThreadCount, CompletionThreadcCont);
		}
	}
}
