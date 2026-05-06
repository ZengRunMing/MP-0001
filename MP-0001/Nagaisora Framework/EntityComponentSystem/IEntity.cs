using System;

using Godot;
using Godot.Collections;

namespace NagaisoraFramework.EntityComponentSystem
{
	public interface IEntity<[MustBeVariant] T> where T : Component
	{
		Guid EUID { get; }

		string EUIDString { get; }

		Array<T> Components { get; }

		System.Collections.Generic.Dictionary<Type, T> ComponentDictionarys { get; set; }

		void AddComponent(T component);

		T1 GetComponent<T1>() where T1 : T;

		void RemoveComponent<T1>() where T1 : T;

		void ClearComponents();
	}
}
