using System.Collections.Generic;

namespace NagaisoraFramework
{
	public class MapDictionary<T1, T2>
	{
		public readonly Dictionary<T1, T2> _forward = [];
		public readonly Dictionary<T2, T1> _reverse = [];

		public MapDictionary()
		{
			Forward = new Indexer<T1, T2>(_forward);
			Reverse = new Indexer<T2, T1>(_reverse);
		}

		public class Indexer<T3, T4>(Dictionary<T3, T4> dictionary)
		{
			public readonly Dictionary<T3, T4> _dictionary = dictionary;

			public T4 this[T3 index]
			{
				get { return _dictionary[index]; }
				set { _dictionary[index] = value; }
			}

			public bool ContainsKey(T3 key)
			{
				return _dictionary.ContainsKey(key);
			}
		}

		public void Add(T1 t1, T2 t2)
		{
			_forward.Add(t1, t2);
			_reverse.Add(t2, t1);
		}

		public Indexer<T1, T2> Forward;
		public Indexer<T2, T1> Reverse;
	}
}
