using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;

	public class CollisionCheckSystem : ISystem<CollisionCheck>
	{
		public void Execute(CollisionCheck component)
		{
			int i = 0;

			foreach (bool collision in component.IsCollision)
			{
				if (collision)
				{
					component.Action?.Invoke(component.TargetEntitys[i]);
				}

				i++;
			}
		}

		public void SubThreadExecute(CollisionCheck component)
		{
			if (component.BaseEntity.Disabled) return;

			for (int i = 0; i < component.TargetEntitys.Count; i++)
			{
				if (component.m_TargetEntitys[i].Disabled) return;

				Transform targetTransform = component.TargetEntityTransforms[i];
				DetermineData targetDetermineData = component.TargetEntityDetermineDatas[i];

				component.IsCollision[i] = CollisionCheck(component.BaseEntityTransform, targetTransform, component.BaseEntityDetermineData, targetDetermineData, component.Scale);
			}
		}

		/// <summary>
		/// 检查是否与指定的STGComponment产生碰撞，指定判定半径，以及是否加入缩放影响，加入缩放影响即将判定半径乘以组件的缩放值，适用于需要根据组件的缩放来调整碰撞范围的情况，例如当组件被放大时碰撞范围也应该相应增大
		/// </summary>
		/// <returns>返回一个Boolean值, 为True则满足碰撞条件</returns>
		public static bool CollisionCheck(Transform baseTransform, Transform targetTransform, DetermineData baseDetermineData, DetermineData targetDetermineData, bool scale = true)
		{
			float f = baseDetermineData.DetermineRadius + targetDetermineData.DetermineRadius * (scale ? baseTransform.Scale + targetTransform.Scale : 1f);

			float baseRotation = FrameworkMath.EulerAnglesADS(baseTransform.Rotation.Z);
			Vector2 baseOffset = baseDetermineData.DetermineOffset * new Vector2(FrameworkMath.Sin(baseRotation), FrameworkMath.Cos(baseRotation));

			float targetRotation = FrameworkMath.EulerAnglesADS(targetTransform.Rotation.Z);
			Vector2 targetOffset = targetDetermineData.DetermineOffset * new Vector2(FrameworkMath.Sin(targetRotation), FrameworkMath.Cos(targetRotation));

			Vector2 cacule = (baseTransform.Position - targetTransform.Position) - (baseOffset - targetOffset);
			Vector2 AbsCacule = new(Mathf.Abs(cacule.X), Mathf.Abs(cacule.Y));

			if (baseTransform.OriginalScale.X != baseTransform.OriginalScale.Y)
			{
				float angle = Mathf.Atan2(cacule.Y, cacule.X) * 57.29578f - baseTransform.Rotation.Z - 90f;
				float adsAngle = FrameworkMath.EulerAnglesADS(angle);

				float num1 = Mathf.Pow(cacule.X * cacule.X + cacule.Y * cacule.Y, 0.5f) * FrameworkMath.Cos(adsAngle);
				float num2 = Mathf.Pow(cacule.X * cacule.X + cacule.Y * cacule.Y, 0.5f) * FrameworkMath.Sin(adsAngle);
				if (num1 / f * (num1 / f) + num2 / f * (num2 / f) < 1f)
				{
					return true;
				}
			}
			else
			{
				if (AbsCacule.Y < f && AbsCacule.X < f && f * f > cacule.Y * cacule.Y + cacule.X * cacule.X)
				{
					return true;
				}
			}

			return false;
		}
	}
}
