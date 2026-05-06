using System;

using Godot;
using Godot.Collections;

namespace NagaisoraFramework.EntityComponentSystem
{
	[GlobalClass, Tool]
	public abstract partial class Component : Resource, IDisposable
	{
		public abstract override Variant _Get(StringName property);

		public abstract override bool _Set(StringName property, Variant value);

		public abstract override Array<Dictionary> _GetPropertyList();

		public new virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
