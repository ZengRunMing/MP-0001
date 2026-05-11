using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;

	using static FrameworkMath;

	public partial class GuidanceControlerSystem : ISystem<GuidanceControler>
	{
		public void Execute(GuidanceControler component)
		{
			
		}

		public void SubThreadExecute(GuidanceControler component)
		{
			if (component.TargetEntity is null || component.TargetEntity.Disabled || component.BaseEntity.Disabled) return;

			float num3 = Direction(component.BaseTransform.Position, component.TargetTransform.Position);
			float num4 = Mathf.DegToRad(component.MoveVectorCalculater.MoveDirection);
			float num5 = num3 - num4;
			if (num5 > Mathf.Pi)
			{
				num5 -= Mathf.Pi * 2f;
			}
			else if (num5 < -Mathf.Pi)
			{
				num5 += Mathf.Pi * 2f;
			}
			if (Mathf.Abs(num5) > 0.02f && Distance(component.BaseTransform.Position, component.TargetTransform.Position) > 50f)
			{
				component.MoveVectorCalculater.MoveDirection += Mathf.RadToDeg(num5 / 5f);
			}
			else
			{
				component.MoveVectorCalculater.MoveDirection += Mathf.RadToDeg(num5);
			}
		}
	}
}
