using Godot;
using System;
using System.Collections.Generic;

namespace NagaisoraFramework.STGSystem
{
	using EntityComponentSystem;
	using Godot.NativeInterop;
	using Melanchall.DryWetMidi.MusicTheory;
	using NagaisoraFramework.STGSystem.ECSComponent;
	using System.IO;
	using System.Linq;
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

		[Export]
		public Godot.Collections.Array<STGComponent> Components
		{
			get => [..ComponentDictionarys.Values];
			set => _ = value;
		}

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
		/// 自身的父组件
		/// </summary>
		/// <remarks>
		/// <para>组件可以通过访问该属性获取父组件的属性和方法</para>
		/// <para>组件在被添加到另一个组件中时会自动设置该属性为父组件的实例</para>
		/// </remarks>
		[Export]
		public STGEntity Parent;

		public List<Condition> Conditions;

		//public ExecuteSystem ExecuteSystem;

		/// <summary>
		/// 组件在STGCanvas中的位置
		/// </summary>
		/// <remarks>
		/// <para>使用Export特性以便在编辑器中调试修改</para>
		/// </remarks>
		[Export]
		public new Vector2 Position
		{
			get => m_Position;
			set
			{
				m_Position = value;
				PositionChanged = true;
			}
		}

		/// <summary>
		/// 组件的虚拟面向方向
		/// </summary>
		/// <remarks>
		/// <para>由于STGComponent的面向方向与旋转角度无关, 因此在设置时不会修改Node3D的Rotation属性</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改</para>
		/// </remarks>
		[Export]
		public float Direction
		{
			get => m_Direction;
			set => m_Direction = value;
		}


		/// <summary>
		/// 组件的X轴旋转角度
		/// </summary>
		/// <remarks>
		/// <para>由于STGComponent的旋转角度与面向方向无关, 因此在设置时不会修改面向方向属性</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改</para>
		/// </remarks>
		[Export]
		public float RotationX
		{
			get => m_RotationX;
			set
			{
				m_RotationX = value;
				RotationChanged = true;
			}
		}

		/// <summary>
		/// 组件的Y轴旋转角度
		/// </summary>
		/// <remarks>
		/// <para>由于STGComponent的旋转角度与面向方向无关, 因此在设置时不会修改面向方向属性</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改</para>
		/// </remarks>
		[Export]
		public float RotationY
		{
			get => m_RotationY;
			set
			{
				m_RotationY = value;
				RotationChanged = true;
			}
		}

		/// <summary>
		/// 组件的Z轴旋转角度
		/// </summary>
		/// <remarks>
		/// <para>由于STGComponent的旋转角度与面向方向无关, 因此在设置时不会修改面向方向属性</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改</para>
		/// </remarks>
		[Export]
		public float RotationZ
		{
			get => m_RotationZ;
			set
			{
				m_RotationZ = value;
				RotationChanged = true;
			}
		}

		/// <summary>
		/// 组件的缩放, 重写了Node3D的Scale属性
		/// </summary>
		/// <remarks>
		/// <para>该缩放是等距的</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改</para>
		/// </remarks>
		[Export]
		public new float Scale
		{
			get => m_Scale;
			set
			{
				m_Scale = value;
				ScaleChanged = true;
			}
		}

		/// <summary>
		/// Node3D的原始坐标
		/// </summary>
		/// <remarks>
		/// 使用Export特性以便在编辑器中调试时查看和修改
		/// </remarks>
		[Export]
		public Vector3 OriginalPosition
		{
			get => base.Position;
			set => base.Position = value;
		}

		/// <summary>
		/// Node3D的原始缩放
		/// </summary>
		/// <remarks>
		/// 使用Export特性以便在编辑器中调试时查看和修改
		/// </remarks>
		[Export]
		public Vector3 OriginalScale
		{
			get => base.Scale;
			set => base.Scale = value;
		}

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

		/// <summary>
		/// 组件的面向方向向量, 由Direction决定, 是一个单位向量
		/// </summary>
		/// <remarks>
		/// <para>在每次获取值时会根据当前的Direction计算新的值并返回, 组件可以通过访问该属性获取当前的面向方向向量, 用于实现基于面向方向的行为和条件</para>
		/// <para>这条属性是只读的</para>
		/// <para>使用Export特性以便在编辑器中调试时查看</para>
		/// </remarks>
		[Export]
		public Vector2 DirectionVector
		{
			get => new(Sin(ADSDitection), Cos(ADSDitection));
			private set => _ = value;
		}

		/// <summary>
		/// 组件的面向方向与轴对齐方向的夹角, 由Direction决定
		/// </summary>
		/// <remarks>
		/// <para>在每次获取值时会根据当前的Direction计算新的值并返回, 组件可以通过访问该属性获取当前的ADSDitection, 用于实现基于轴对齐方向的行为和条件</para>
		/// <para>这条属性是只读的</para>
		/// <para>使用Export特性以便在编辑器中调试时查看</para>
		/// </remarks>
		[Export]
		public float ADSDitection
		{
			get => EulerAnglesADS(Direction);
			private set => _ = value;
		}

		public bool OnMoveToPoint;
		public Vector2 DestPoint;
		public Vector2 StartPosition;

		public bool ViewAngleWithDirection;
		public bool MoveVectorWithDirection;

		public bool CheckOutBoundary;

		public float Accelerate;

		public bool Disabled = false;

		private Vector2 m_Position;
		private float m_Direction;

		private float m_RotationX;
		private float m_RotationY;
		private float m_RotationZ;

		private float m_Scale;

		public uint m_ThisTime;

		private Vector2[] m_LastPositions;

		public Action MoveToPointAction;

		// 属性修改执行标志
		public bool PositionChanged;
		public bool RotationChanged;
		public bool ScaleChanged;
		// ===============

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
			StartPosition = Position;

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

			if (MoveVectorWithDirection)
			{
				GetMoveVectorWithDirection();
			}

			PropertyUpdate();
			ClearPropertyUpdateFlags();

			if (ThisTime < 5)
			{
				goto Skip;
			}

			if (CheckOutBoundary && OutBoundaryCheck())
			{
				BaseDelete();
				return;
			}

			Skip:
			m_ThisTime++;
		}

		/// <summary>
		/// 子线程更新函数，用于重写
		/// </summary>
		public virtual void OnSubThreadUpdate()
		{
			if (Disabled)
			{
				return;
			}

			// 子线程更新内容
			// 在此处的执行不涉及引擎API的计算

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
		/// <para>按键事件</para>
		/// <para>可在派生类中重写以实现具体的行为</para>
		/// </summary>
		/// <param name="keys">采集的按键</param>
		public virtual void OnKeyDown(InputKey keys)
		{

		}

		/// <summary>
		/// 根据设定的目标点计算移动速度和方向
		/// </summary>
		public void OnMovePoint()
		{
			float distance = GetDistance(DestPoint);
			if (distance > Mathf.Abs(Velocity))
			{
				Direction =  Mathf.RadToDeg(GetDirection(DestPoint));
				Accelerate = (0f - Velocity) * Velocity / (2f * distance);
				if (Velocity < 0f)
				{
					Velocity = 0f;
				}
			}
			else
			{
				Position = DestPoint;
				Accelerate = 0f;
				Velocity = 0f;

				OnMoveToPoint = false;

				MoveToPointAction?.Invoke();

				MoveToPointAction = null;
			}
		}

		public virtual void PropertyUpdate()
		{
			if (PositionChanged)
			{
				base.Position = new Vector3(Position.X, Position.Y, base.Position.Z);
			}
			if (RotationChanged)
			{
				Rotation = new Vector3(RotationX, RotationY, RotationZ);
			}
			if (ScaleChanged)
			{
				OriginalScale = Vector3.One * Scale;
			}

			ClearPropertyUpdateFlags();
		}

		public virtual void ClearPropertyUpdateFlags()
		{
			PositionChanged = false;
			RotationChanged = false;
			ScaleChanged = false;
		}

		/// <summary>
		/// 注册移动到指定点的控制
		/// </summary>
		/// <param name="destPoint">指定点位</param>
		/// <param name="maxVelocity">最大速度</param>
		/// <param name="defaultVelovity">默认速度</param>
		/// <param name="action">执行完成时执行的动作</param>
		public void RegisterMoveToPointAction(Vector2 destPoint, float maxVelocity, float defaultVelovity, Action action = null)
		{
			if (action is null)
			{
				MoveToPointAction = null;
			}
			else
			{
				MoveToPointAction = new Action(action);
			}

			DestPoint = destPoint;
			OnMoveToPoint = true;

			MaxVelocity = maxVelocity;
			MinVelocity = 0f;
			Velocity = defaultVelovity;
		}

		/// <summary>
		/// 将移动向量设置为面向方向, 适用于需要组件的移动方向与面向方向保持一致的情况, 例如当组件需要朝向一个目标移动时, 可以通过设置MoveVectorWithDirection为true来自动将移动向量与面向方向保持一致
		/// </summary>
		public void GetMoveVectorWithDirection()
		{
			MoveVector = DirectionVector;
		}

		/// <summary>
		/// 检查自身是否出界
		/// </summary>
		/// <returns>返回一个Boolean值，True则为出界</returns>
		public virtual bool OutBoundaryCheck()
		{
			if (Position.X > STGControler.BoundarySize.X / 2 || Position.X < -STGControler.BoundarySize.X || Position.Y > STGControler.BoundarySize.Y || Position.Y < -STGControler.BoundarySize.Y)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// 诱导系统
		/// </summary>
		/// <param name="target">指定的STGComponment</param>
		public virtual void GuidanceControl(STGEntity target)
		{
			if (target == null || target.Disabled)
			{
				return;
			}

			float num3 = GetDirection(target);
			float num4 = Mathf.DegToRad(Direction);
			float num5 = num3 - num4;
			if (num5 > (float)Math.PI)
			{
				num5 -= (float)Math.PI * 2f;
			}
			else if (num5 < -(float)Math.PI)
			{
				num5 += (float)Math.PI * 2f;
			}
			if (Mathf.Abs(num5) > 0.02f && GetDistance(target) > 50f)
			{
				Direction += Mathf.RadToDeg(num5 / 5f);
			}
			else
			{
				Direction += Mathf.RadToDeg(num5);
			}
		}

		/// <summary>
		/// 获取指定的STGComponment在自身的方向
		/// </summary>
		/// <param name="Target"></param>
		/// <returns>返回方向角度</returns>
		public float GetDirection(STGEntity Target)
		{
			return GetDirection(Target.Position);
		}

		/// <summary>
		/// 获取指定的坐标在自身的方向
		/// </summary>
		/// <param name="TargetPosition">指定的坐标</param>
		/// <returns>返回方向角度</returns>
		public float GetDirection(Vector2 TargetPosition)
		{
			return (float)Math.PI + Mathf.Atan2(Position.X - TargetPosition.X, Position.Y - TargetPosition.Y);
		}

		/// <summary>
		/// 获取与另一个STGComponment的距离
		/// </summary>
		/// <param name="Target">指定的STGComponment</param>
		/// <returns>返回距离</returns>
		public float GetDistance(STGEntity Target)
		{
			return GetDistance(Target.Position);
		}

		/// <summary>
		/// 获取与指定坐标的距离
		/// </summary>
		/// <param name="TargetPosition">指定的坐标</param>
		/// <returns>返回距离</returns>
		public float GetDistance(Vector2 TargetPosition)
		{
			return (TargetPosition - Position).Length();
		}

		/// <summary>
		/// 向STGControler注册KeyDown事件
		/// </summary>
		public void RegisterKeyEvent()
		{
			STGControler.KeyDown += OnKeyDown;
		}

		/// <summary>
		/// 向STGControler注销KeyDown事件
		/// </summary>
		public void UnRegisterKeyEvent()
		{
			STGControler.KeyDown -= OnKeyDown;
		}

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
			Position = STGControler.DisablePosition;

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
			OnMoveToPoint = false;
			CheckOutBoundary = false;

			DestPoint = Vector2.Zero;

			MoveVector = Vector2.Zero;

			DetermineRadius = 0f;
			DetermineOffset = Vector2.Zero;

			Scale = 1F;

			RotationX = 0f;
			RotationY = 0f;
			RotationZ = 0f;

			Direction = 0f;

			Velocity = 0f;
			MaxVelocity = 0f;
			MinVelocity = 0f;
			
			m_ThisTime = 0;

			LastPositions = null;

			//ExecuteSystem?.Dispose();
		}
	}
}
