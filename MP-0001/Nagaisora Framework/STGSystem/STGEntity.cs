using Godot;
using System;
using System.Collections.Generic;

namespace NagaisoraFramework.STGSystem
{
	using EntityComponentSystem;
	using NagaisoraFramework.STGSystem.ECSComponent;

	using static FrameworkMath;

	/// <summary>
	/// STG基础实体, 继承自Node3D, 并实现IEntity接口, 由STGControler中的ECS系统管理
	/// </summary>
	/// <remarks>
	/// <para>实体在STGControler中通过事件系统和委托系统被统一管理和更新</para>
	/// </remarks>
	[Tool, GlobalClass, Serializable]
	public partial class STGEntity : Node3D, IEntity<STGComponent>
	{
		/// <summary>
		/// 实体的GUID, 用于唯一标识一个实体, 组件在第一次创建时由ECS系统为其生成一个新的GUID
		/// </summary>
		/// <remarks>
		/// 正常情况下STGComponent的销毁只是放入缓冲池堆栈中, 所以第二次创建时会继续使用之前的GUID
		/// </remarks>
		public Guid EUID { get; protected set; }

		/// <summary>
		/// 实体GUID的字符串表示形式, 以便在编辑器中调试时查看
		/// </summary>
		/// <remarks>
		/// 该属性是只读的
		/// </remarks>
		[Export(PropertyHint.MultilineText)]
		public string EUIDString
		{
			get => BitConverter.ToString(EUID.ToByteArray()).Replace('-', ' ');
			private set => _ = value;
		}

		/// <summary>
		/// 组件列表，用于在检查器面板中查看
		/// </summary>
		[Export]
		public Godot.Collections.Array<STGComponent> Components
		{
			get => [..ComponentDictionarys.Values];
			set => _ = value;
		}

		/// <summary>
		/// 组件字典
		/// </summary>
		public Dictionary<Type, STGComponent> ComponentDictionarys { get; set; }

		/// <summary>
		/// 指定的STGControler
		/// </summary>
		/// <remarks>
		/// <para>组件通过访问该属性获取当前的游戏时间和其他全局信息, 组件也可以通过访问该属性调用STGControler提供的全局方法</para>
		/// <para>组件在被添加到STGControler中时会自动设置该属性为所属的STGControler的实例</para>
		/// <para>使用Export特性以便在编辑器中调试时查看, 一般情况下不要尝试在编辑器中修改这个属性，否则可能造成未知的故障</para>
		/// </remarks>
		[Export]
		public STGControler STGControler;

		/// <summary>
		/// 自身的父实体
		/// </summary>
		/// <remarks>
		/// <para>组件可以通过访问该属性获取父组件的属性和方法</para>
		/// <para>组件在被添加到另一个组件中时会自动设置该属性为父组件的实例</para>
		/// </remarks>
		[Export]
		public STGEntity Parent;

		/// <summary>
		/// STGControler的全局游戏时间, 以帧为单位
		/// </summary>
		/// <remarks>
		/// <para>从STGControler开始运行时开始计时, 组件可以通过访问该属性获取当前的游戏时间, 用于实现基于时间的行为和条件</para>
		/// <para><b>这条属性是只读的</b></para>
		/// <para>使用Export特性以便在编辑器中调试时查看</para>
		/// </remarks>
		[Export]
		public uint GameTime
		{
			get => STGControler is not null ? STGControler.GameTime : 0;
			private set => _ = value;
		}

		/// <summary>
		/// 组件的本地时间, 以帧为单位
		/// </summary>
		/// <remarks>
		/// <para>从组件被启用时开始计时, 组件在每次更新时会自动增加该属性</para>
		/// <para>组件可以通过访问该属性获取当前的本地时间, 用于实现基于时间的行为和条件</para>
		/// <para>与GameTime不同的是, ThisTime只与组件自身的启用状态相关, 当组件被禁用时ThisTime会停止增加, 当组件再次被启用时ThisTime会继续增加, 这使得ThisTime非常适合用于实现需要在组件启用期间持续计时的行为和条件</para>
		/// <para>这条属性是只读的</para>
		/// <para>使用Export特性以便在编辑器中调试时查看</para>
		/// </remarks>
		[Export]
		public uint ThisTime
		{
			get => m_ThisTime;
			private set => _ = value;
		}

		/// <summary>
		/// 组件经过的坐标组
		/// </summary>
		/// <remarks>
		/// <para>这条属性是只读的</para>
		/// <para>可通过Reset()复位</para>
		/// <para>使用Export特性以便在编辑器中调试时查看</para>
		/// </remarks>
		[Export]
		public Vector2[] LastPositions
		{
			get => m_LastPositions;
			private set => m_LastPositions = value;
		}

		public List<Condition> Conditions;

		//public ExecuteSystem ExecuteSystem;

		public Vector2 StartPosition;

		public bool Disabled = false;

		public uint m_ThisTime;

		private Vector2[] m_LastPositions;

		public Action MoveToPointAction;

		public STGEntity()
		{
			EUID = Guid.NewGuid();
			ComponentDictionarys = [];

			_ =EUIDString;
		}

		public override void _PhysicsProcess(double delta)
		{
			m_ThisTime++;
		}

		public virtual void OnEnable()
		{
			
		}

		public virtual void OnDisable()
		{

		}

		public virtual void Init()
		{
			//StartPosition = Position;

			m_ThisTime = 0;

			Disabled = false;
		}

		//public virtual void Init(ExecuteSystem system)
		//{
		//	ExecuteSystem = system;
		//}

		//public virtual void Init(Assembly assembly)
		//{
		//	Init(new ExecuteSystem(assembly, STGControler, this));
		//}

		/// <summary>
		/// 自身的更新方法
		/// </summary>
		/// <remarks>
		/// <para>可在派生类中重写以实现其它行为</para>
		/// <para>组件在每一帧都会调用该方法来更新自身的状态和行为, 组件在被禁用时STG控制器会跳过该方法的执行, 组件在该方法中可以访问当前的游戏时间和本地时间, 以及其他属性和方法, 来实现基于时间和状态的行为和条件</para>
		/// </remarks>
		public virtual void OnUpdate()
		{
			OnCondition();

			if (ThisTime < 5)
			{
				goto Skip;
			}

			Skip:
			m_ThisTime++;
		}

		/// <summary>
		/// Condition检查与执行方法
		/// </summary>
		public void OnCondition()
		{
			foreach (Condition condition in Conditions)
			{
				condition.ConditionExecute();
			}
		}

		/// <summary>
		/// 诱导系统
		/// </summary>
		/// <param name="target">指定的STGComponment</param>
		//public virtual void GuidanceControl(STGEntity target)
		//{
		//	if (target == null || target.Disabled)
		//	{
		//		return;
		//	}

		//	float num3 = GetDirection(target);
		//	float num4 = Mathf.DegToRad(Direction);
		//	float num5 = num3 - num4;
		//	if (num5 > (float)Math.PI)
		//	{
		//		num5 -= (float)Math.PI * 2f;
		//	}
		//	else if (num5 < -(float)Math.PI)
		//	{
		//		num5 += (float)Math.PI * 2f;
		//	}
		//	if (Mathf.Abs(num5) > 0.02f && GetDistance(target) > 50f)
		//	{
		//		Direction += Mathf.RadToDeg(num5 / 5f);
		//	}
		//	else
		//	{
		//		Direction += Mathf.RadToDeg(num5);
		//	}
		//}

		/// <summary>
		/// 注册Condition
		/// </summary>
		/// <param name="Condition">条件</param>
		/// <param name="ExecuteAction">执行动作</param>
		/// <param name="LoopExecution">是否循环执行</param>
		/// <returns>返回注册的Condition</returns>
		public Condition RegisterCondition(Func<bool> Condition, Action ExecuteAction, bool LoopExecution = false)
		{
			Condition condition = new Condition(Condition, ExecuteAction, LoopExecution);
			RegisterCondition(condition);

			return condition;
		}

		/// <summary>
		/// 注册Condition
		/// </summary>
		/// <param name="condition">指定的Condition</param>
		public void RegisterCondition(Condition condition)
		{
			lock (Conditions)
			{
				if (Conditions.Contains(condition))
				{
					GD.PushWarning($"[{GetType()}] Condition already registered.");
					return;
				}
				Conditions.Add(condition);
			}
		}

		/// <summary>
		/// 注销Condition
		/// </summary>
		/// <param name="condition">指定的Condition</param>
		public void UnRegisterCondition(Condition condition)
		{
			lock (Conditions)
			{
				if (!Conditions.Contains(condition))
				{
					return;
				}

				Conditions.Remove(condition);
			}
		}

		public void AddComponent(STGComponent component)
		{
			if (ComponentDictionarys.ContainsKey(component.GetType()))
			{
				GD.PushWarning($"[{GetType()}] Component \"{component.GetType()}\" already added.");
				return;
			}

			ComponentDictionarys.Add(component.GetType(), component);
			component.BaseSTGEntity = this;
			component.Initialize();
		}

		public T GetComponent<T>() where T : STGComponent
		{
			if (ComponentDictionarys.TryGetValue(typeof(T), out STGComponent component))
			{
				return component as T;
			}

			return default;
		}

		public void RemoveComponent<T>() where T : STGComponent
		{
			ComponentDictionarys.Remove(typeof(T));
		}

		public void ClearComponents()
		{
			foreach (STGComponent component in Components)
			{
				component.BaseSTGEntity = null;
			}
			Components.Clear();
		}

		/// <summary>
		/// 删除自身
		/// </summary>
		public virtual void BaseDelete()
		{
			//Position = STGControler.DisablePosition;

			//UpdateUnityProperty();
			//ClearUnityPropertyUpdateFlags();

			Reset();
			Disabled = true;

			STGControler.PoolManager.DeleteObject(this);
		}

		/// <summary>
		/// 将所有属性复位
		/// </summary>
		public virtual void Reset()
		{
			//MoveVector = Vector2.Zero;

			//DetermineRadius = 0f;
			//DetermineOffset = Vector2.Zero;

			//Scale = 1F;

			//Velocity = 0f;
			//MaxVelocity = 0f;
			//MinVelocity = 0f;
			
			m_ThisTime = 0;

			LastPositions = null;

			//ExecuteSystem?.Dispose();
		}
	}
}
