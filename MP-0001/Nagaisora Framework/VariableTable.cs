using System;
using System.Collections.Generic;

namespace NagaisoraFramework
{
	public class VariableTable
	{
		public Dictionary<string, VariableObject> Variables = [];

		public void AddVariable(string name, Type type)
		{
			if (Variables.ContainsKey(name))
			{
				throw new ArgumentException($"Variable '{name}' already exists.");
			}

			VariableObject @object = new(type, name);

			Variables.Add(name, @object);
		}

		public void RemoveVariable(string name)
		{
			if (!Variables.ContainsKey(name))
			{
				throw new ArgumentException($"Variable '{name}' does not exist.");
			}
			Variables.Remove(name);
		}

		public void SetVariable(string name, object value)
		{
			if (!Variables.TryGetValue(name, out VariableObject @object))
			{
				throw new ArgumentException($"Variable '{name}' does not exist.");
			}

			if (value.GetType() != @object.Type)
			{
				throw new InvalidOperationException($"Cannot assign value of type {value.GetType()} to variable of type {@object.Type}");
			}

			@object.Value = value;
		}

		public object GetVariable(string name)
		{
			if (!Variables.TryGetValue(name, out VariableObject @object))
			{
				throw new ArgumentException($"Variable '{name}' does not exist.");
			}

			return @object.Value;
		}
	}
}
