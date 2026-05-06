using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;

	public class MoveControlerSystem : ISystem<MoveControler>
	{
		public void Execute(MoveControler component)
		{
			if (component.MoveVector == Vector2.Zero || component.Velocity == 0)
			{
				return;
			}

			component.Transform.Position += component.MoveVector * component.Velocity;
		}

		public void SubThreadExecute(MoveControler component)
		{

		}
	}
}
