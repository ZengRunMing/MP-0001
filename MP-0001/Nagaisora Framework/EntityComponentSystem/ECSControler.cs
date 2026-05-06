using System;
using System.Collections.Generic;

namespace NagaisoraFramework.EntityComponentSystem
{
	public class ECSControler<T> where T : Component
	{
		public Dictionary<Type, Action<T>> Systems;

		public ECSControler()
		{
			Systems = [];
		}

		public void Execute(T component)
		{
			if (Systems.TryGetValue(component.GetType(), out var action))
			{
				action.Invoke(component);
			}
		}

		public void RegesterComponent<T1>(ISystem<T1> system) where T1 : T
		{
			if (Systems.ContainsKey(typeof(T1)))
			{
				return;
			}

			Systems.Add(typeof(T1), new Action<T>((T) =>{ system.Execute(T as T1); }));
		}

		public void UnRegesterComponent<T1>(ISystem<T1> system) where T1 : T
		{
			if (!Systems.ContainsKey(typeof(T1)))
			{
				return;
			}

			Systems.Remove(typeof(T1));
		}
	}
}
