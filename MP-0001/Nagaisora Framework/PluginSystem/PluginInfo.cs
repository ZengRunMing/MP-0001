using System;
using System.Drawing;

namespace NagaisoraFramework.PluginSystem
{
	public class PluginInfo
	{
		public Image Image;
		public string Name;
		public string Author;
		public Version Version;
		public string URL;
		public bool OpenScource;
		public string License;
		public string Describe;

		public override string ToString()
		{
			return $"拓展模块信息\n" +
				   $"  名称 : {Name}\n" +
				   $"制作者 : {Author}\n" +
				   $"  版本 : {Version}";
		}
	}
}
