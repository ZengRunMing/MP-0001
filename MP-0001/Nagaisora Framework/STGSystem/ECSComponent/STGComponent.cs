using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	using EntityComponentSystem;

	[Tool, GlobalClass]
	public abstract partial class STGComponent : Component
	{
		public STGEntity BaseSTGEntity { get; set; }

		public abstract override Variant _Get(StringName property);

		public abstract override bool _Set(StringName property, Variant value);

		public abstract override Array<Dictionary> _GetPropertyList();

		public virtual void Initialize()
		{

		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == GetType())
			{
				return true;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
