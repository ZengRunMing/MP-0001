using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;

	using static FrameworkMath;

	public class MoveVectorCalculaterSystem : ISystem<MoveVectorCalculater>
	{
		public void Execute(MoveVectorCalculater component)
		{

		}

		public void SubThreadExecute(MoveVectorCalculater component)
		{
			float adsAngle = EulerAnglesADS(component.MoveDirection);

			component.MoveControler.MoveVector = new Vector2(Sin(adsAngle), Cos(adsAngle));
		}
	}
}
