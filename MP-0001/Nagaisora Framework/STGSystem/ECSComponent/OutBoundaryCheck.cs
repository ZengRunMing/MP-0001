using Godot;
using Godot.Collections;

using System;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class OutBoundaryCheck(Action action) : STGComponent
	{
		public Transform Transform;

		public bool IsOutBoundary;

		public Action OutBoundary = action;

		public override Variant _Get(StringName property) => default;

		public override bool _Set(StringName property, Variant value) => false;

		public override Array<Dictionary> _GetPropertyList() => [];
	}
}
