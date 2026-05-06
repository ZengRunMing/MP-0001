//using System;
//using System.Collections.Generic;
//using System.Linq;

//using Godot;

//namespace NagaisoraFramework.STGSystem
//{
//	using static MainSystem;
//	using static FrameworkMath;

//	public partial class Laser : STGComponent
//	{
//		public LaserType Type;
//		public int Color;

//		public bool Determing = true;
//		public bool Delete_Effect = false;

//		public BendLaserMesh BendLaserMesh;

//		public int LaserLength;
//		public float Width = 16f;

//		public List<Vector2> KeyPoints;

//		public FixedLengthQueue<Vector2> KeyPointQueue;

//		public Vector2 HeadPosition;
//		public Vector2 LastPosition;

//		public override void Init()
//		{
//			base.Init();

//			DetermineRadius = STGControler.STGSystemData.EnemyLongLaser.DetermineRadius;

//			if (Type == LaserType.Long)
//			{
//				LaserLength = 2000;

//				GameObject objectT = new GameObject()
//				{
//					name = $"LaserObject[{name}]",
//					layer = 6,
//				};

//				objectT.transform.SetParent(transform);

//				InitSpriteRender();

//				SpriteRender.drawMode = SpriteDrawMode.Sliced;
//				SpriteRender.sprite = STGControler.STGSystemData.EnemyLongLaser.Info[Color].Sprite;
//				SpriteRender.size = STGControler.STGSystemData.EnemyLongLaser.Normoal_Size;
//				SpriteRender.sortingLayerName = "EnemyBullet";
//				SpriteRender.sortingOrder = Order;

//				Transform.localPosition = HeadPosition;

//				objectT.AddComponent<PositionofSize>();
//			}
//			else
//			{
//				KeyPointQueue = new FixedLengthQueue<Vector2>(LaserLength);

//				BendLaserMesh = gameObject.AddComponent<BendLaserMesh>();
//				Transform.localPosition = Vector3.zero;

//				BendLaserMesh.Init("StageMain", Order);

//				SetBendLaserMesh();
//			}

//		}

//		public override void OnUpdate()
//		{
//			base.OnUpdate();

//			Check();

//			if (Type == LaserType.Segmental)
//			{
//				if (HeadPosition != LastPosition)
//				{
//					KeyPointQueue.Enqueue(HeadPosition);
//					LastPosition = HeadPosition;
//				}

//				Vector2[] vector2s = KeyPointQueue.ToArray();
//				Array.Reverse(vector2s);
//				KeyPoints = vector2s.ToList();

//				SetBendLaserMesh();
//			}
//		}

//		public override void OnMove()
//		{
//			if (MoveVector == Vector2.zero)
//			{
//				return;
//			}

//			HeadPosition += MoveVector * Velocity;
//		}

//		public virtual void Check()
//		{
//			if (ThisTime < 5 || !Determing)
//			{
//				return;
//			}

//			if (HitCheck(STGControler.Player))
//			{
//				Debug.Log($"[{name}] >> HitCheck() -> True");
//				return;
//			}

//			if (GrazeCheck(STGControler.Player))
//			{
//				if (ThisTime % 2 == 0)
//				{
//					SoundEffectControler.PlaySE("Graze");
//				}
//			}
//		}

//		public override bool HitCheck(STGComponent Target)
//		{
//			return HitCheck(Target, DetermineRadius);
//		}

//		public override bool HitCheck(STGComponent Target, float DetermineRadius)
//		{
//			if (Target == null || Target.Disabled)
//			{
//				return false;
//			}

//			if (DetermineRadius <= 0f)
//			{
//				return false;
//			}

//			float fx = DetermineRadius * Scale.x;
//			float fy = DetermineRadius * Scale.y;

//			float px = fx + STGControler.DetermineVector;
//			float py = fy + STGControler.DetermineVector;

//			for (int i = 0; i < LaserLength; i++)
//			{
//				float ax = Position.x;
//				float ay = Position.y;

//				if (Type == LaserType.Segmental)
//				{
//					if (i >= KeyPoints.Count)
//					{
//						break;
//					}

//					if (i < 3)
//					{
//						continue;
//					}

//					ax += KeyPoints[i].x;
//					ay += KeyPoints[i].y;
//				}
//				else
//				{
//					ax += i * Sin(ADSDitection);
//					ay += i * Cos(ADSDitection);
//				}

//				float dx = ax - Target.Position.x - DetermineOffset.x;
//				float dy = ay - Target.Position.y - DetermineOffset.y;

//				if (Mathf.Abs(dy) < py && Mathf.Abs(dx) < px && py * px > dy * dy + dx * dx)
//				{
//					return true;
//				}
//			}

//			return false;
//		}

//		public override bool OutSizeCheck()
//		{
//			if (KeyPoints is null || KeyPoints.Count == 0)
//			{
//				return false;
//			}

//			Vector2 position = KeyPoints.Last();

//			if (position.x > STGControler.MaxPosition.x || position.x < -STGControler.MaxPosition.x || position.y > STGControler.MaxPosition.y || position.y < -STGControler.MaxPosition.y)
//			{
//				return true;
//			}

//			return false;
//		}

//		public virtual bool GrazeCheck(STGComponent Target)
//		{
//			return HitCheck(STGControler.Player, DetermineRadius + 4f);
//		}

//		public void SetBendLaserMesh()
//		{
//			if (BendLaserMesh == null)
//			{
//				return;
//			}

//			if (KeyPoints != null)
//			{
//				BendLaserMesh.KeyPoints = KeyPoints.ToArray();
//			}

//			BendLaserMesh.ColorType = Color;
//			BendLaserMesh.Width = Width;
//		}

//		public override void BaseDelete()
//		{
//			if (BendLaserMesh != null)
//			{
//				BendLaserMesh.Clear();
//				Destroy(BendLaserMesh);
//			}
//			if (SpriteRender != null)
//			{
//				Destroy(SpriteRender);
//			}

//			if (Delete_Effect)
//			{
//				STGControler.NewEnemyShootEffect(Color, Order - 21, Position);
//			}

//			STGControler.EnemyLasers.Remove(this);

//			base.BaseDelete();
//		}
//	}
//}
