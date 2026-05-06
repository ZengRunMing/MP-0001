using Godot;
using System;

namespace NagaisoraFramework.STGSystem
{
	[GlobalClass]
	[Serializable]
	public partial class Enemy : AnimatedSpriteRendererSTGComponent
	{
		[Export]
		public float HealthValue
		{
			get
			{
				return m_HealthValue;
			}
			set
			{
				m_HealthValue = value;

				if (HealthValue < 0f)
				{
					//Delete_Effect = true;
					BaseDelete();
				}
			}
		}

		protected float m_HealthValue = 1f;

		public override void Init()
		{
			base.Init();

			//STGControler.Enemys.Add(this);

			//DetermineOffset = EnemyInfo.DetermineOffset;
			//DetermineRadius = EnemyInfo.DetermineRadius;

			SetAnimatorNormal();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			Check(STGControler.Player);
		}

		public virtual void Check(STGEntity Target)
		{
			if (Target == null || Target.Disabled)
			{
				return;
			}

			if (STGControler.TestStatus)
			{
				return;
			}

			//if (ThisTime < 5 || !Determing)
			//{
			//	return;
			//}

			//if (HitCheck(Target))
			//{
			//	STGControler.LifeSub();
			//	BaseDelete();
			//	return;
			//}
		}

		public void OnDamage(float damageValue)
		{
			HealthValue -= damageValue;
		}

		public override void BaseDelete()
		{
			//if (Delete_Effect)
			//{
			//	//STGControler.NewEnemyEndEffect(Order, Position);
			//}
			//STGControler.Enemys.Remove(this);

			base.BaseDelete();
		}

		public virtual void SetAnimatorNormal()
		{
			if (AnimatedSpriteRenderer is null)
			{
				return;
			}

			AnimationName = "Normal";
			FlipX = false;
		}

		public virtual void SetAnimatorMoveLeft()
		{
			if (AnimatedSpriteRenderer is null)
			{
				return;
			}

			AnimationName = "ToMove";
			FlipX = true;
		}

		public virtual void SetAnimatorMoveRight()
		{
			if (AnimatedSpriteRenderer is null)
			{
				return;
			}

			AnimationName = "ToMove";
			FlipX = true;
		}
	}
}
