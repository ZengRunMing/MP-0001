
//using UnityEngine;

//namespace NagaisoraFramework.STGSystem
//{
//	public class ParticleSystemEffect : Effect
//	{
//		public bool Inited = false; // 是否已初始化

//		public ParticleSystem[] ParticleSystems;                       // 必要的粒子系统组件

//		public override void Init()
//		{
//			base.Init(); // 调用父类的初始化方法

//			Transform.localPosition = Position; // 设置组件的本地位置

//			foreach (ParticleSystem particleSystem in ParticleSystems)
//			{
//				if (particleSystem is null)
//				{
//					Debug.LogWarning("ParticleSystemEffectControl: ParticleSystem is null."); // 如果粒子系统为空，则输出警告信息
//					continue; // 跳过当前循环，继续下一个粒子系统
//				}

//				ParticleSystemRenderer render = particleSystem.GetComponentInChildren<ParticleSystemRenderer>(); // 获取粒子系统渲染器组件

//				render.sortingLayerName = RenderLayerName;	// 设置粒子系统的渲染图层
//				render.sortingOrder = Order;				// 设置粒子系统的渲染顺序
//			}

//			Play();			// 播放粒子效果

//			Inited = true;	// 标记为已初始化
//		}

//		public void Play()
//		{
//			if (ParticleSystems == null)
//			{
//				Debug.LogError("ParticleSystemEffectControl: ParticleSystem is not assigned or found."); // 如果粒子系统未分配，则输出错误信息
//			}

//			foreach(ParticleSystem particleSystem in ParticleSystems)
//			{
//				if (particleSystem is null)
//				{
//					Debug.LogWarning("ParticleSystemEffectControl: ParticleSystem is null."); // 如果粒子系统为空，则输出警告信息
//					continue; // 跳过当前循环，继续下一个粒子系统
//				}

//				particleSystem.Play(); // 播放粒子效果
//			}
			
//		}

//		public override void OnUpdate()
//		{
//			base.OnUpdate(); // 调用父类的更新方法

//			if (!Inited)
//			{
//				return; // 如果未初始化，则不执行后续逻辑
//			}

//			foreach (ParticleSystem particleSystem in ParticleSystems)
//			{
//				if (particleSystem is null)
//				{
//					Debug.LogWarning("ParticleSystemEffectControl: ParticleSystem is null."); // 如果粒子系统为空，则输出警告信息
//					continue; // 跳过当前循环，继续下一个粒子系统
//				}

//				if (particleSystem.isPlaying) // 检查粒子系统是否正在播放
//				{
//					return; // 任意一个还在播放则直接返回，等待下一帧重新检查
//				}
//			}

//			foreach (ParticleSystem particleSystem in ParticleSystems)
//			{
//				if (particleSystem is null)
//				{
//					Debug.LogWarning("ParticleSystemEffectControl: ParticleSystem is null."); // 如果粒子系统为空，则输出警告信息
//					continue; // 跳过当前循环，继续下一个粒子系统
//				}

//				particleSystem.Stop(); // 停止所有粒子系统，避免状态不一致造成无法预知的播放问题
//			}

//			BaseDelete(); // 将效果在系统中失能

//			Inited = false; // 重置初始化状态
//		}
//	}
//}
