using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;

	public partial class MoveToPointControlerSystem : ISystem<MoveToPointControler>
	{
		public void Execute(MoveToPointControler component)
		{
			if (!component.OnMoeToPoint)
			{
				return;
			}

			if (component.Transform.Position == component.DestPoint)
			{
				component.OnMoeToPoint = false;
				component.Action?.Invoke();
			}
		}

		public void SubThreadExecute(MoveToPointControler component)
		{
			if (!component.OnMoeToPoint)
			{
				return;
			}

			MoveToPoint(component);

			component.MoveControler.Velocity += component.Accelerate;
		}

		public static void MoveToPoint(MoveToPointControler component)
		{
			float velocity = component.MoveControler.Velocity;
			float distance = FrameworkMath.Distance(component.Transform.Position, component.DestPoint);

			if (distance > Mathf.Abs(velocity))
			{
				component.MoveVectorCalculater.MoveDirection = Mathf.RadToDeg(FrameworkMath.Direction(component.Transform.Position, component.DestPoint));
				component.Accelerate = (0f - velocity) * velocity / (2f * distance);

				if (velocity < 0f)
				{
					component.MoveControler.Velocity = 0f;
				}
			}
			else
			{
				component.Transform.Position = component.DestPoint;
				component.MoveControler.Velocity = 0f;
				component.Accelerate = 0f;
			}
		}
	}
}
