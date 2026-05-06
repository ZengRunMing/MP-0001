using System;
using System.Collections.Generic;

namespace NagaisoraFramework
{
	public class PoolManager<T>() where T : class, new()
	{
		public Dictionary<Type, Stack<T>> Stack = [];

		public T1 NewObject<T1>() where T1 : class, T, new()
		{
			if (Stack.TryGetValue(typeof(T1), out Stack<T> value))
			{
				if (value.Count == 0)
				{
					return new T1();
				}

				return value.Pop() as T1;
			}
			else
			{
				Stack.Add(typeof(T1), new Stack<T>());
				return new T1();
			}
		}

		public void DeleteObject<T1>(T1 sender) where T1 : class, T
		{
			if (!Stack.ContainsKey(typeof(T1)))
			{
				throw new NullReferenceException($"[{GetType()}] Type {typeof(T1)} not found in PoolManager.");
			}

			Stack[typeof(T1)].Push(sender);
			return;
		}

		public void Clear()
		{
			Stack.Clear();
		}
	}
}