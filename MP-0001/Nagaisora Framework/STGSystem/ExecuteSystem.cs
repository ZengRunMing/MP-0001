using System;
using System.Reflection;

namespace NagaisoraFramework.STGSystem
{
	public class ExecuteSystem : IDisposable
	{
		public string Name;

		public AssemblySystem AssemblySystem;

		public Type NFEProgram;

		public object MainObject;

		public MethodInfo Main;

		public ExecuteSystem(Assembly assembly, STGControler controler)
		{
			AssemblySystem = new AssemblySystem(assembly);
			Name = AssemblySystem.Name.FullName;
			Type[] types = AssemblySystem.ListAllExportedType();

			foreach (Type type in types)
			{
				if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NFEProgram)))
				{
					NFEProgram = type;
					Main = NFEProgram.GetMethod("Main");

					MainObject = AssemblySystem.CreateInstance(NFEProgram, new object[] { this, controler });
				}
			}
		}

		public void MainInvoke()
		{
			Main.Invoke(MainObject, null);
		}

		public void Dispose()
		{

		}
	}
}

