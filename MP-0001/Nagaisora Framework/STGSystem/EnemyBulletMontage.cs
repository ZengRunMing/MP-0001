//using Godot;

//namespace NagaisoraFramework.STGSystem
//{
//	using static FrameworkMath;

//	public partial class EnemyBulletMontage : EnemyBullet
//	{
//		public EnemyBullet[] Bullets;

//		public Vector2[] BulletVectors;

//		public Vector2 Vector;

//		public override void Init()
//		{
//			base.Init();

//			if (Bullets is null || Bullets.Length == 0)
//			{
//				return;
//			}

//			BulletVectors = new Vector2[Bullets.Length];

//			for (int i = 0; i < Bullets.Length; i++)
//			{
//				Bullets[i].Init();

//				BulletVectors[i] = Bullets[i].Position - Position;
//			}
//		}

//		public override void OnMove()
//		{
//			base.OnMove();

//			float ADSAngle = EulerAnglesADS(Rotation);

//			Vector = new Vector2(Sin(ADSAngle), Cos(ADSAngle));

//			int i = 0;
//			foreach (var item in Bullets)
//			{
//				item.Position = (Position + BulletVectors[i]) * Vector;
//				i++;
//			}
//		}
//	}
//}
