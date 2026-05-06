using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NagaisoraFramework.PluginSystem
{
	public class Plugin
	{
		public static List<string> FindPlugin()
		{
			List<string> pluginpath = new List<string>();

			try
			{
				string path = AppDomain.CurrentDomain.BaseDirectory;
				path = Path.Combine(path, "Plugins");
				foreach (string filename in Directory.GetFiles(path, "*.dll"))
				{
					pluginpath.Add(filename);
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
			return pluginpath;
		}

		public static object LoadObject(Assembly asm, string className, string interfacename, object[] param)
		{
			try
			{
				Type t = asm.GetType(className);
				if (t == null
					|| !t.IsClass
					|| !t.IsPublic
					|| t.IsAbstract
					|| t.GetInterface(interfacename) == null
				   )
				{
					return null;
				}
				object o = Activator.CreateInstance(t, param);
				if (o == null)
				{
					return null;
				}
				return o;
			}
			catch
			{
				return null;
			}
		}

		public static List<string> DeleteInvalidPlungin(List<string> PlunginPath)
		{
			string interfacename = typeof(IPlugin).FullName;
			List<string> rightPluginPath = new List<string>();

			foreach (string filename in PlunginPath)
			{
				try
				{
					Assembly asm = Assembly.LoadFile(filename);

					foreach (Type t in asm.GetExportedTypes())
					{

						object plugin = LoadObject(asm, t.FullName, interfacename, null);

						if (plugin != null)
						{
							rightPluginPath.Add(filename);
							break;
						}
					}
				}
				catch
				{
					Console.WriteLine(filename + "不是有效插件");
				}
			}
			return rightPluginPath;
		}
	}
}
