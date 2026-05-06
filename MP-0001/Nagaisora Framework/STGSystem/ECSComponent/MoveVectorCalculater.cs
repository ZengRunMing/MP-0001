using Godot;
using Godot.Collections;

using System;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class MoveVectorCalculater : STGComponent
	{
		public MoveControler MoveControler;

		public float MoveDirection;

		public override Variant _Get(StringName property)
		{
			throw new NotImplementedException();
		}

		public override bool _Set(StringName property, Variant value)
		{
			throw new NotImplementedException();
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			throw new NotImplementedException();
		}
	}
}
