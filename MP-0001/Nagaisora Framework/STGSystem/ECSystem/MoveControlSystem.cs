using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using EntityComponentSystem;
	using ECSComponent;

	public class MoveControlSystem : ISystem<MoveControl>
	{
		public void Execute(MoveControl component)
		{
			if (component.MoveVector == Vector2.Zero || component.Velocity == 0)
			{
				return;
			}

			component.BaseSTGEntity.Position += component.MoveVector * component.Velocity;
		}
	}
}
