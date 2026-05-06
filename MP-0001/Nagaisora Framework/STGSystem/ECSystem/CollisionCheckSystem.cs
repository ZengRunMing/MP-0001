using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using EntityComponentSystem;
	using ECSComponent;

	public class CollisionCheckSystem : ISystem<CollisionCheck>
	{
		public void Execute(CollisionCheck component)
		{
			STGEntity baseSTGEntity = component.BaseSTGEntity;
			STGEntity targetSTGEntity = component.TargetSTGEntity;

			float baseDetermineRadius = component.BaseEntityDetermineRadius.Radius;
			float targetDetermineRadius = component.TargetEntityDetermineRadius.Radius;

			component.IsCollision = CollisionCheck(baseSTGEntity, targetSTGEntity, baseDetermineRadius, targetDetermineRadius);
		}

		/// <summary>
		/// 检查是否与指定的STGComponment产生碰撞，指定判定半径，以及是否加入缩放影响，加入缩放影响即将判定半径乘以组件的缩放值，适用于需要根据组件的缩放来调整碰撞范围的情况，例如当组件被放大时碰撞范围也应该相应增大
		/// </summary>
		/// <param name="baseSTGEntity">自身实体</param>
		/// <param name="targetSTGEntity">目标实体</param>
		/// <param name="baseDetermineRadius">自身判定半径</param>
		/// <param name="targetDetermineRadius">目标判定半径</param>
		/// <param name="scale">是否加入缩放影响, 默认为True</param>
		/// <returns>返回一个Boolean值, 为True则满足碰撞条件</returns>
		public static bool CollisionCheck(STGEntity baseSTGEntity, STGEntity targetSTGEntity, float baseDetermineRadius, float targetDetermineRadius, bool scale = true)
		{
			if (baseSTGEntity is null || baseSTGEntity.Disabled || targetSTGEntity is null || targetSTGEntity.Disabled)
			{
				return false;
			}

			if (baseDetermineRadius <= 0f || targetDetermineRadius <= 0f)
			{
				return false;
			}

			float f = baseDetermineRadius + targetDetermineRadius * (scale ? baseSTGEntity.Scale + targetSTGEntity.Scale : 1f);

			float rads = FrameworkMath.EulerAnglesADS(baseSTGEntity.RotationZ);

			float dx = baseSTGEntity.Position.X - targetSTGEntity.Position.X - (baseSTGEntity.DetermineOffset.X * FrameworkMath.Sin(rads));
			float dy = baseSTGEntity.Position.Y - targetSTGEntity.Position.Y - (baseSTGEntity.DetermineOffset.Y * FrameworkMath.Cos(rads));

			float adx = Mathf.Abs(dx);
			float ady = Mathf.Abs(dy);

			if (baseSTGEntity.OriginalScale.X != baseSTGEntity.OriginalScale.Y)
			{
				float EADS_Angle = Mathf.Atan2(dy, dx) * 57.29578f - baseSTGEntity.RotationZ - 90f;
				float EADS = FrameworkMath.EulerAnglesADS(EADS_Angle);
				float num1 = Mathf.Pow(dx * dx + dy * dy, 0.5f) * FrameworkMath.Cos(EADS);
				float num2 = Mathf.Pow(dx * dx + dy * dy, 0.5f) * FrameworkMath.Sin(EADS);
				if (num1 / f * (num1 / f) + num2 / f * (num2 / f) < 1f)
				{
					return true;
				}
			}
			else
			{
				if (ady < f && adx < f && f * f > dy * dy + dx * dx)
				{
					return true;
				}
			}

			return false;
		}
	}
}
