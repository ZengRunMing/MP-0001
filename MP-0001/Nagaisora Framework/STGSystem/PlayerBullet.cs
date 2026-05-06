
//using UnityEngine;

//namespace NagaisoraFramework.STGSystem
//{
//	//玩家子弹控制系统
//	public class PlayerBullet : Bullet
//	{
//		public PlayerBulletInfo BulletData
//		{
//			get
//			{
//				return m_BulletData;
//			}
//			set
//			{
//				m_BulletData = value;
//				BulletDataChanged = true;
//			}
//		}

//		public bool BulletDataChanged
//		{
//			get
//			{
//				return m_BulletDataChanged;
//			}
//			set
//			{
//				m_BulletDataChanged = value;
//			}
//		}

//		public float DamageValue
//		{
//			get
//			{
//				return m_DamageValue;
//			}
//			set
//			{
//				m_DamageValue = value;
//			}
//		}


//		protected PlayerBulletInfo m_BulletData;


//		protected float m_DamageValue;

//		// Unity Property Changed Flag

//		protected bool m_BulletDataChanged;

//		public override void Init()
//		{
//			base.Init();

//			SpriteRender.drawMode = SpriteDrawMode.Sliced;
//			SpriteRender.sortingLayerName = "PlayerBullet";
//			SpriteRender.sortingOrder = Order;

//			STGControler.PlayerBullets.Add(this);
//		}

//		public override void OnUpdate()
//		{
//			base.OnUpdate();

//			Enemy[] enemys = STGControler.Enemys.ToArray();
//			foreach (var enemy in enemys)
//			{
//				Check(enemy);
//			}
//		}

//		public override void UpdateUnityProperty()
//		{
//			if (m_BulletDataChanged)
//			{
//				DetermineOffset = BulletData.DetermineOffset;
//				DetermineRadius = BulletData.DetermineRadius;
//				AngleOffsetCompensation = BulletData.AngleOffsetCompensation;   //设定显示角度偏移补偿

//				Sprite = BulletData.Sprite;
//			}

//			base.UpdateUnityProperty();
			
//			if (m_BulletDataChanged)
//			{
//				SpriteRender.size = BulletData.Normoal_Size;
//			}
//		}

//		public override void ClearUnityPropertyUpdateFlags()
//		{
//			base.ClearUnityPropertyUpdateFlags();

//			m_BulletDataChanged = false;
//		}

//		public override void Check(STGComponent Target)
//		{
//			if (Target == null || Target.Disabled)
//			{
//				return;
//			}

//			if (HitCheck(Target))
//			{
//				BaseDelete();
//				Target.GetComponent<Enemy>().OnDamage(DamageValue);
//				return;
//			}
//		}

//		public override void BaseDelete()
//		{
//			STGControler.PlayerBullets.Remove(this);

//			base.BaseDelete();
//		}
//	}
//}
