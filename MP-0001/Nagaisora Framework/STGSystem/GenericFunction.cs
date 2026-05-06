//using System.Collections.Generic;

//using Godot;

//namespace NagaisoraFramework.STGSystem
//{
//	public static class GenericFunction
//	{
//		public enum AlignmentMode
//		{
//			Left, Middle, Right
//		}

//		public static T[] NewCircularBullet<T>(string identifier, int order, int count, int type, int color, Vector2 position, float initialAngle, STGControler controler) where T : EnemyBullet
//		{
//			initialAngle = FrameworkMath.EulerAnglesADS(initialAngle);

//			float Angle = initialAngle;
//			float AngleStep = 360f / count;

//			List<T> results = [];

//			controler.NewEnemyShootEffect(color, 0, position, true, BlendMode.AlphaBlend);

//			for (int i = 0; i < count; i++)
//			{
//				T Component = controler.NewEnemyBullet<T>(type, color, $"CircularBullet_{identifier}_{i}", order, position, Angle, false);

//				Component.Init();

//				results.Add(Component);

//				Angle += AngleStep;
//			}

//			return results.ToArray();
//		}

//		public static (GameObject, T)[] NewPolygonalRingBullet<T>(string identifier, int order, int count, int type, int color, Vector2 position, float initialAngle, STGControler controler) where T : EnemyBullet
//		{
//			initialAngle = FrameworkMath.EulerAngles_ADS(initialAngle);

//			float Angle = initialAngle;
//			float AngleStep = 360f / count;

//			List<(GameObject, T)> results = new List<(GameObject, T)>();

//			controler.NewEnemyShootEffect(color, 0, position, true, BlendMode.AlphaBlend);

//			for (int i = 0; i < count; i++)
//			{
//				(GameObject obj, T Component) = controler.NewEnemyBullet<T>(type, color, $"PolygonalRingBullet_{identifier}_{i}", order, position, Angle, false);
				
//				Component.Init();

//				results.Add((obj, Component));

//				Angle += AngleStep;
//			}

//			return results.ToArray();
//		}

//		public static (GameObject, T)[] NewSectorBullet<T>(string identifier, int order, int count, int type, int color, Vector2 position, float unilateralAngle, float initialAngle, AlignmentMode mode, STGControler controler) where T : EnemyBullet
//		{
//			initialAngle = FrameworkMath.EulerAngles_ADS(initialAngle);

//			float Angle;
//			float AngleStep = unilateralAngle / (count - 1);

//			if (mode == AlignmentMode.Middle)
//			{
//				Angle = initialAngle + (unilateralAngle / 2f);
//			}
//			else
//			{
//				Angle = initialAngle;
//			}

//			if (mode == AlignmentMode.Left || mode == AlignmentMode.Middle)
//			{
//				AngleStep = -AngleStep;
//			}

//			List<(GameObject, T)> results = new List<(GameObject, T)>();

//			controler.NewEnemyShootEffect(color, 0, position, true, BlendMode.AlphaBlend);

//			for (int i = 0; i < count; i++)
//			{
//				(GameObject obj, T Component) = controler.NewEnemyBullet<T>(type, color, $"SectorBullet_{identifier}_{i}", order, position, Angle, false);

//				Component.Init();

//				results.Add((obj, Component));

//				Angle += AngleStep;
//			}

//			return results.ToArray();
//		}

//		public static (GameObject, T) NewRandomDirectionBullet<T>(string identifier, int order, int type, int color, Vector2 position, float angleMin, float angleMax, STGControler controler) where T : EnemyBullet
//		{
//			float angle = MainSystem.RandomFloat(angleMin, angleMax);

//			controler.NewEnemyShootEffect(color, 0, position, true, BlendMode.AlphaBlend);
			
//			(GameObject obj, T Component) = controler.NewEnemyBullet<T>(type, color, $"RandomDirectionBullet_{identifier}", order, position, angle, false);

//			Component.Init();

//			return (obj, Component);
//		}

//		public static (GameObject, T) NewAimedBullet<T>(string identifier, int order, int type, int color, Vector2 position, Vector2 targetPosition, float initialAngle, STGControler controler) where T : EnemyBullet
//		{
//			initialAngle = FrameworkMath.EulerAngles_ADS(initialAngle);

//			Vector2 direction = targetPosition - position;

//			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + initialAngle;
			
//			controler.NewEnemyShootEffect(color, 0, position, true, BlendMode.AlphaBlend);
			
//			(GameObject obj, T Component) = controler.NewEnemyBullet<T>(type, color, $"AimedBullet_{identifier}", order, position, angle, false);

//			Component.Init();

//			return (obj, Component);
//		}

//		public static T[] RandomArrayArrangement<T>(T[] i)
//		{
//			List<T> source = new List<T>(i);

//			List<T> result = new List<T>();

//			while (source.Count > 0)
//			{
//				int index = MainSystem.RandomInt(0, source.Count - 1);
//				result.Add(source[index]);
//				source.RemoveAt(index);
//			}

//			return result.ToArray();
//		}
		
//		public static float[] GetAngleDivisionArray(int count, float Angle)
//		{
//			float[] result = new float[count];

//			float AngleStep = Angle / count;

//			float currentAngle = 0f;

//			for (int i = 0; i < count; i++)
//			{
//				result[i] = currentAngle;
//				currentAngle += AngleStep;
//			}

//			return result;
//		}

//		public static T GetValueFromRank<T>(STGControler controler, params T[] values)
//		{
//			return values[controler.Rank];
//		}

//		public static T GetValueFromPlayerType<T>(STGControler controler, params T[] values)
//		{
//			return values[controler.PlayerType];
//		}
//	}
//}
