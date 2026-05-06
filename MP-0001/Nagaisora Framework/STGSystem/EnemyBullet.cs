using System;

using Godot;

namespace NagaisoraFramework.STGSystem
{
	[GlobalClass]

	public partial class EnemyBullet : Bullet
	{
		public float GrazeRadiusOffset;

		/// <summary>
		/// 基于父类重写的初始化函数，在回调父类函数后添加了向STGControl注册自身的程序动作
		/// </summary>
		public override void Init()												//基于父类派生重写的初始化方法，此处编写初始化程序
		{
			base.Init();														//调用父类的初始化方法

			STGControler.EnemyBullets.Add(this);
		}

		/// <summary>
		/// 基于父类重写的主线程更新函数, 并回调父类
		/// </summary>
		public override void OnUpdate()											//基于父类派生重写的逻辑更新方法
		{
			base.OnUpdate();													//调用父类的逻辑更新方法
		}

		/// <summary>
		/// 敌机子弹的全判定函数，可以被重写
		/// </summary>
		/// <param name="Target">自机的STGComponment</param>
		public override void Check(STGEntity target)							//基于父类派生重写的判定检查方法
		{
			if (target == null || target.Disabled)
			{
				return;
			}

			if (STGControler.TestStatus)										//如果是测试模式则不进行下一步动作
			{
				return;
			}

			//if (CollisionCheck(target, DetermineRadius + GrazeRadiusOffset))	//判断指定对象(玩家)是否在Graze判定范围内
			//{
			//	if (ThisTime % 2 == 0)											//每间隔一次更新执行一次
			//	{
			//		//SoundEffectControler.PlaySE("Graze");						//调用播放音效方法
			//	}
			//}

			//if (ThisTime < 5 || !Determing)										//判断本地逻辑更新时间是否小于5以及是否未开启判定
			//{
			//	return;															//执行无条件返回
			//}

			//if (CollisionCheck(target))                                         //判断指定对象(玩家)是否在判定范围内
			//{
			//	//STGControler.LifeSub();											//调用STG管理器玩家残机减一函数
			//	BaseDelete();													//销毁自身 (入列对象池)
			//	return;															//执行无条件返回
			//}
		}

		/// <summary>
		/// 基于父类派生重写的销毁自身方法
		/// </summary>
		public override void BaseDelete()
		{
			if (Delete_Effect)
			{
				//STGControler.NewEnemyShootEffect(Color, Order - 21, Position);
			}

			STGControler.EnemyBullets.Remove(this);

			base.BaseDelete();
		}
	}
}
