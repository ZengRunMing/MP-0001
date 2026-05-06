using System;

namespace NagaisoraFramework.PluginSystem
{
	public interface IPlugin : IDisposable
	{
		PluginInfo PluginInfo { get; }
	}
}
