using System;

namespace NagaisoraFramework
{
	public class VariableObject(Type type, string name)
	{
		public Type Type = type;
		public string Name = name;
		public object Value = null;
	}
}
