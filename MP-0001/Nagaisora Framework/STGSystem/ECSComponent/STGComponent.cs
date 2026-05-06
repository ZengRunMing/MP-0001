using Godot;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	using EntityComponentSystem;

	[Tool, GlobalClass]
	public abstract partial class STGComponent(STGControler controler, STGEntity entity) : Component
	{
		public STGControler STGControler { get; set; } = controler;
		public STGEntity BaseSTGEntity { get; set; } = entity;

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
