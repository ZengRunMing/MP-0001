using System;
using System.Linq;
using System.Collections.Generic;

namespace NagaisoraFramework
{
	public class TimeLineFlag
	{
		public string Name;

		public TimeSpan Time;

		public bool MultipleExecutions;

		public Action OnEnter;
		public Action OnAction;
		public Action OnLeave;
	}

	[Serializable]
	public class TimeLineSystem : TimeControler
	{
		public Dictionary<string, TimeLineFlag> Flags;
		public List<TimeLineFlag> RemovedConditionFlags;
		public Dictionary<string, TimeLineFlag> RunningFlags;
		public List<TimeLineFlag> RemovedRunningFlags;

		public TimeLineSystem()
		{
			Flags = new Dictionary<string, TimeLineFlag>();
			RemovedConditionFlags = new List<TimeLineFlag>();
			RunningFlags = new Dictionary<string, TimeLineFlag>();
			RemovedRunningFlags = new List<TimeLineFlag>();
		}

		public void OnUpdate()
		{
			FlagCheck();
		}

		public virtual void FlagCheck()
		{
			if (!IsRunning)
			{
				return;
			}

			if ((Flags == null || Flags.Count == 0) && (RunningFlags == null || RunningFlags.Count == 0))
			{
				return;
			}

			TimeLineFlag[] ConditionFlagsArray = Flags.Values.ToArray();
			RemovedConditionFlags.Clear();
			foreach (var flag in ConditionFlagsArray)
			{
				if (Elapsed.Ticks < flag.Time.Ticks)
				{
					continue;
				}

				flag.OnEnter?.Invoke();
				AddRunningFlags(flag);
				RemovedConditionFlags.Add(flag);
			}

			TimeLineFlag[] RemovedConditionFlagsArray = RemovedConditionFlags.ToArray();
			foreach (var flag in RemovedConditionFlagsArray)
			{
				RemoveFlags(flag);
			}

			TimeLineFlag[] RunningFlagsArray = RunningFlags.Values.ToArray();
			RemovedRunningFlags.Clear();
			foreach (var flag in RunningFlagsArray)
			{
				flag.OnAction();

				if (!flag.MultipleExecutions)
				{
					flag.OnLeave();
					RemovedRunningFlags.Add(flag);
				}
			}

			TimeLineFlag[] RemovedRunningFlagsArray = RemovedRunningFlags.ToArray();
			foreach (var flag in RemovedRunningFlagsArray)
			{
				RemoveRunningFlags(flag);
			}
		}

		public virtual void AddFlags(params TimeLineFlag[] flags)
		{
			foreach (var flag in flags)
			{
				if (Flags.ContainsKey(flag.Name))
				{
					throw new Exception($"ConditionFlag {flag.Name} already exists in {GetHashCode()}.");
				}
				Flags.Add(flag.Name, flag);
			}
		}

		public virtual void RemoveFlags(params TimeLineFlag[] flags)
		{
			foreach (var flag in flags)
			{
				Flags.Remove(flag.Name);
			}
		}

		public virtual void AddRunningFlags(params TimeLineFlag[] flags)
		{
			foreach (var flag in flags)
			{
				if (RunningFlags.ContainsKey(flag.Name))
				{
					throw new Exception($"RunningFlag {flag.Name} already exists in {GetHashCode()}.");
				}
				RunningFlags.Add(flag.Name, flag);
			}
		}

		public virtual void RemoveRunningFlags(params TimeLineFlag[] flags)
		{
			foreach (var flag in flags)
			{
				RunningFlags.Remove(flag.Name);
			}
		}
	}
}
