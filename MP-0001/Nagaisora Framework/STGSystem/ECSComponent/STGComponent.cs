using Godot;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	using EntityComponentSystem;

	[Tool, GlobalClass]
	public abstract partial class STGComponent : Component
	{
		public STGControler STGControler { get; set; }

		public STGEntity BaseEntity { get; set; }

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
