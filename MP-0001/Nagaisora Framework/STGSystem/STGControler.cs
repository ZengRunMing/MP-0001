using System;
using System.Collections.Generic;

using Godot;

namespace NagaisoraFramework.STGSystem
{
	using DataSystem;
	using EntityComponentSystem;
	using ECSComponent;
	using ECSystem;

	[GlobalClass]

	public partial class STGControler : Node3D, IDisposable
	{
		public ECSControler<STGComponent> ECSControler;

		[ExportGroup("渲染组件")]
		[ExportSubgroup("视口组件")]
		[Export]
		public SubViewport OutputViewport;
		[Export]
		public SubViewport CanvasViewport;
		[Export]
		public SubViewport BackgroundViewport;
		
		[ExportSubgroup("摄像机组件")]
		[Export]
		public Camera3D CanvasCamera;
		[Export]
		public Camera3D MainCamera;
		
		[ExportSubgroup("纹理渲染组件")]
		[Export]
		public Sprite3D BackgroundImage;
		[Export]
		public Sprite3D OutputImage;

		[ExportSubgroup("主画布组件")]
		[Export]
		public Node3D MainCanvas;

		[ExportGroup("事件系统组件")]
		[Export]
		public InputSystem InputSystem;
		[ExportGroup("时钟信号组件")]
		[Export]
		public ClockSystem ClockSystem;

		[ExportGroup("玩家实体")]
		[Export]
		public Player Player;

		[ExportGroup("渲染参数")]
		[Export]
		public Color BackgroundColor
		{
			get
			{
				return BackgroundImage != null ? BackgroundImage.Modulate : new Color(0, 0, 0, 0);
			}
			set
			{
				if (BackgroundImage != null)
				{
					BackgroundImage.Modulate = value;
				}
			}
		}

		[ExportGroup("系统定义参数")]
		[Export]
		public int MaxEnemyBulletCount = 1000000;
		
		[Export]
		public Vector2 Size = new(512, 512);
		[Export]
		public Vector2 BoundarySize;

		[Export]
		public Vector2 PlayerMaxPosition = new(270, 310);
		[Export]
		public Vector2 PlayerDefaultPositon = new(0, -250);

		[Export]
		public Vector2 DisablePosition = new(0, 800);

		[Export]
		public bool TestStatus = false;

		[Export]
		public uint PlayerType;
		[Export]
		public uint Rank;

		[ExportGroup("实时数据")]
		[Export]
		public uint BulletEffectCount;
		[Export]
		public uint EnemyBulletCount;
		[Export]
		public uint GameTime = 0;

		[Export]
		public bool IsReplaying;

		[Export]
		public bool PlayerInvincible;

		public ReplaySystem ReplaySystem;

		public PoolManager<STGEntity> PoolManager;

		public ExecuteSystem ExecuteSystem;

		public List<Timer> Timers;
		public List<Condition> Conditions;
		public Dictionary<string, SpellCard> SpellCards;

		//public List<Enemy> Enemys;
		public List<EnemyBullet> EnemyBullets;
		//public List<Laser> EnemyLasers;
		//public List<PlayerBullet> PlayerBullets;
		//public List<Effect> Effects;

		//public Enemy[] EnemysArray;
		//public EnemyBullet[] EnemyBulletsArray;
		//public Laser[] EnemyLasersArray;
		//public PlayerBullet[] PlayerBulletsArray;
		//public Effect[] EffectsArray;

		public STGControler()
		{

		}

		/// <summary>
		/// 创建一个新的STG控制器实例
		/// </summary>
		/// <param name="parent">挂载的Node</param>
		/// <param name="name">控制器的名称</param>
		/// <param name="inputSystem">指定控制器所用的输入系统</param>
		/// <param name="clockSystem">指定控制器的时钟信号系统</param>
		/// <param name="size">指定控制器的渲染画布大小</param>
		/// <param name="boundaryMargin">指定STG组件边界离画布的边距</param>
		public STGControler(string name, InputSystem inputSystem, ClockSystem clockSystem, Vector2I size, float boundaryMargin = 100f)
		{
			ECSControler = new();

			ECSControler.RegesterComponent(new CollisionCheckSystem());

			// 设置名称和输入系统
			Name = name;
			InputSystem = inputSystem;
			ClockSystem = clockSystem;

			Size = size;
			BoundarySize = Size + (Vector2.One * boundaryMargin * 2);

			// 创建渲染系统

			// 创建子视口组件
			OutputViewport = new()
			{
				CanvasCullMask = 1,
				Name = $"OutputViewport",
				Size = size,
				TransparentBg = true,
			};
			CanvasViewport = new()
			{
				CanvasCullMask = 2,
				Name = $"CanvasViewport",
				Size = size,
				TransparentBg = true,
			};
			BackgroundViewport = new()
			{
				CanvasCullMask = 4,
				Name = $"BackgroundViewport",
				Size = size,
				TransparentBg = true,
			};

			AddChild(OutputViewport);
			AddChild(CanvasViewport);
			AddChild(BackgroundViewport);

			// 创建相机组件
			Godot.Environment CanvasCameraEnvironment = new()
			{
				BackgroundMode = Godot.Environment.BGMode.ClearColor,
				BackgroundEnergyMultiplier = 1,

				AmbientLightSource = Godot.Environment.AmbientSource.Disabled,
				ReflectedLightSource = Godot.Environment.ReflectionSource.Disabled,
				
				TonemapMode = Godot.Environment.ToneMapper.Linear,
				TonemapExposure = 1,
			};
			Godot.Environment MainCameraEnvironment = new()
			{
				BackgroundMode = Godot.Environment.BGMode.ClearColor,
				BackgroundEnergyMultiplier = 1,

				AmbientLightSource = Godot.Environment.AmbientSource.Disabled,
				ReflectedLightSource = Godot.Environment.ReflectionSource.Disabled,

				TonemapMode = Godot.Environment.ToneMapper.Linear,
				TonemapExposure = 1,
			};

			float CameraSize = (size.Y * 0.01f) + 0.01f;

			CanvasCamera = new()
			{
				Name = "CanvasCamera",
				Environment = CanvasCameraEnvironment,
				CullMask = 2,
				Projection = Camera3D.ProjectionType.Orthogonal,
				Size = CameraSize,
				Position = new Vector3(0, 0, 1)
			};
			MainCamera = new()
			{
				Name = "MainCamera",
				Environment = MainCameraEnvironment,
				CullMask = 1,
				Projection = Camera3D.ProjectionType.Orthogonal,
				Size = CameraSize,
				Position = new Vector3(0, 0, 5)
			};

			CanvasViewport.AddChild(CanvasCamera);
			OutputViewport.AddChild(MainCamera);

			// 创建纹理渲染组件
			BackgroundImage = new()
			{
				Name = "BackgroundImage",
				Texture = BackgroundViewport.GetTexture(),
				Layers = 2,
				Position = new Vector3(0, 0, -1)
			};
			CanvasViewport.AddChild(BackgroundImage);

			OutputImage = new()
			{
				Name = "OutputImage",
				Texture = CanvasViewport.GetTexture(),
				Layers = 1,
			};
			OutputViewport.AddChild(OutputImage);

			// 创建主画布
			MainCanvas = new()
			{
				Name = "MainCanvas"
			};
			CanvasViewport.AddChild(MainCanvas);

			// 创建对象池系统
			PoolManager = new();

			// 创建定时器列表
			Timers = [];

			// 创建判断程序列表
			Conditions = [];

			// 创建符卡注册表
			SpellCards = [];

			// 创建对象栈列表
			//Enemys = [];
			EnemyBullets = [];
			//EnemyLasers = [];
			//PlayerBullets = [];
			//Effects = [];

			// 新建动作录像系统及注册输入系统按键事件
			ReplaySystem = new(this);
			InputSystem.KeyDown += ReplaySystem.PushKey;

			// 注册局部时钟信号事件
			ClockSystem.FixedUpdate += FixedUpdate;
			ClockSystem.Update += Update;
		}

		public new virtual void Dispose()
		{
			// 注销按键事件
			InputSystem.KeyDown -= ReplaySystem.PushKey;

			// 注销时钟事件
			ClockSystem.FixedUpdate -= FixedUpdate;
			ClockSystem.Update -= Update;

			// 销毁动作录像系统
			ReplaySystem.Dispose();

			base.Dispose();
		}

		public virtual void Update(ulong tick)
		{
			// 更新数据
			EnemyBulletCount = (uint)EnemyBullets.Count;
			IsReplaying = ReplaySystem != null && ReplaySystem.IsReplaying;
		}

		public virtual void FixedUpdate(ulong tick)
		{
			TimeClock((uint)tick);
		}

		public void TimeClock(uint tick)
		{
			// 向时钟发生器同步游戏时间
			GameTime = tick;

			// 同步录像系统时间
			if (ReplaySystem != null)
			{
				ReplaySystem.GameTime = GameTime;
			}

			// 更新游戏数据
			OnUpdate();
		}

		public virtual void OnUpdate()
		{
			ReplaySystem?.OnUpdate();

			foreach (Timer timer in Timers.ToArray())
			{
				timer.OnUpdate();
			}

			//EnemysArray = Enemys.ToArray();
			//EnemyBulletsArray = EnemyBullets.ToArray();
			//EnemyLasersArray = EnemyLasers.ToArray();
			//PlayerBulletsArray = PlayerBullets.ToArray();
			//EffectsArray = Effects.ToArray();

			//Player?.OnUpdate();

			//foreach (var enemy in EnemysArray)
			//{
			//	enemy?.OnUpdate();
			//}

			//foreach (var enemyBullet in EnemyBulletsArray)
			//{
			//	enemyBullet?.OnUpdate();
			//}

			//foreach (var enemyLaser in EnemyLasersArray)
			//{
			//	enemyLaser?.OnUpdate();
			//}

			//foreach (var playerBullet in PlayerBulletsArray)
			//{
			//	playerBullet?.OnUpdate();
			//}

			//foreach (var effect in EffectsArray)
			//{
			//	effect?.OnUpdate();
			//}
		}

		public virtual void Reset()
		{
			GameTime = 0;

			if (ReplaySystem != null)
			{
				ReplaySystem.GameTime = GameTime;
			}

			PoolManager.Clear();
		}

		public virtual void Start()
		{
			if (ClockSystem.IsRunning) return;

			ExecuteSystem?.MainInvoke();

			if (TestStatus)
			{
				Player.Visible = false;
			}
			else
			{
				Player.Visible = true;
			}

			//Player?.Init();

			ClockSystem.Start();
		}

		public virtual void Stop()
		{
			if (!ClockSystem.IsRunning) return;

			ClockSystem.Stop();
		}

		public Timer RegisterTimer(TimeSpan setTime, Action executeAction)
		{
			Timer timer = new()
			{
				SetTime = setTime,
				Action = executeAction
			};

			RegisterTimer(timer);

			return timer;
		}

		public void RegisterTimer(Timer timer)
		{
			Timers.Add(timer);
		}

		public void UnRegisterTimer(Timer timer)
		{
			Timers.Remove(timer);
		}

		public Condition RegisterCondition(Func<bool> condition, Action executeAction, bool loopExecution)
		{
			Condition Condition = new Condition(condition, executeAction, loopExecution);

			RegisterCondition(Condition);

			return Condition;
		}

		public void RegisterCondition(Condition condition)
		{
			Conditions.Add(condition);
		}

		public void UnRegisterCondition(Condition condition)
		{
			Conditions.Remove(condition);
		}

		public void RegisterSpellCard(SpellCard spellCard)
		{
			SpellCards.Add(spellCard.Name, spellCard);
		}

		public void UnRegisterSpellCard(string name)
		{
			SpellCards.Remove(name);
		}

		public void RegisterKeyEvent(KeyDownEvent @event)
		{
			InputSystem.KeyDown += @event;
		}

		public void UnRegisterKeyEvent(KeyDownEvent @event)
		{
			InputSystem.KeyDown -= @event;
		}

		public T NewEntity<T>(string name) where T : STGEntity, new()
		{
			return NewEntity<T>(name, MainCanvas);
		}

		public T NewEntity<T>(string name, Node parent) where T : STGEntity, new()
		{
			T Entity = PoolManager.NewObject<T>();
			Entity.Name = name;
			Entity.STGControler = this;

			parent.AddChild(Entity);

			return Entity;
		}

		//public GameObject NewObjectOfPrefab(Type type, string name, GameObject parent, GameObject prefab)
		//{
		//	GameObject Object = PoolManager.NewObjectOfPrefab(type, prefab);

		//	if (Object.transform.parent != parent.transform)
		//	{
		//		Object.transform.SetParent(parent.transform);
		//	}

		//	Object.name = name;
		//	Object.layer = parent.layer;
		//	Object.transform.localScale = new Vector3(1, 1, 1);

		//	return Object;
		//}

		//public (GameObject, T) CreatePlayer<T>() where T : Player
		//{
		//	if (!(Player is null))
		//	{
		//		return (Player.gameObject, Player as T);
		//	}

		//	GameObject Object = new GameObject();

		//	if (!Object.TryGetComponent(out T component))
		//	{
		//		component = Object.AddComponent<T>();
		//		component.STGControler = this;
		//	}

		//	Player = component;

		//	return (Object, component);
		//}

		//public (GameObject, T) NewEnemy<T>(int type, int color, string name, int order, Vector2 position, bool init = true, BlendMode blendMode = BlendMode.AlphaBlend) where T : Enemy
		//{
		//	EnemyInfo EnemyInfo = STGSystemData.Enemy[type];

		//	GameObject Object = NewObject(typeof(T), name, Parent);

		//	if (!Object.TryGetComponent(out T component))
		//	{
		//		component = Object.AddComponent<T>();
		//		component.STGControler = this;
		//	}

		//	component.Position = position;
		//	component.Type = type;
		//	component.Color = color;
		//	component.DetermineOffset = EnemyInfo.DetermineOffset;
		//	component.DetermineRadius = EnemyInfo.DetermineRadius;
		//	component.Order = order;

		//	if (init)
		//	{
		//		component.Init();
		//	}

		//	return (Object, component);
		//}

		public T NewEnemyBullet<T>(int type, int color, string name, int order, Vector2 position, float angle, bool init = true, BlendMode blendMode = BlendMode.AlphaBlend) where T : EnemyBullet, new()
		{
			if (EnemyBullets.Count >= MaxEnemyBulletCount)
			{
				GD.PushWarning("BulletNumberOutMaxCount");
				return null;
			}

			return default;
		}

		//public (GameObject, T) NewEnemyLaser<T>(LaserType type, int color, int length, string name, int order, Vector2 position, float angle, bool init = true, BlendMode blendMode = BlendMode.AlphaBlend) where T : Laser
		//{
		//	if (EnemyLasers.Count >= MaxEnemyBulletCount)
		//	{
		//		Debug.Log("LaserNumberOutMaxCount");
		//		return (null, null);
		//	}

		//	GameObject Object = NewObject(typeof(T), name, Parent);

		//	if (!Object.TryGetComponent(out T component))
		//	{
		//		component = Object.AddComponent<T>();
		//		component.STGControler = this;
		//	}

		//	component.Position = Vector2.zero;
		//	component.HeadPosition = position;
		//	component.Type = type;
		//	component.Color = color;
		//	component.LaserLength = length;
		//	component.Order = order;
		//	component.Direction = angle;

		//	if (init)
		//	{
		//		component.Init();
		//	}

		//	return (Object, component);
		//}

		//public (GameObject, T) NewPlayerBullet<T>(int type, string name, int order, Vector2 position, float angle, bool init = true, BlendMode blendMode = BlendMode.AlphaBlend) where T : PlayerBullet
		//{
		//	GameObject Object = NewObject(typeof(T), name, Parent);

		//	if (!Object.TryGetComponent(out T component))
		//	{
		//		component = Object.AddComponent<T>();
		//		component.STGControler = this;
		//	}

		//	component.Position = position;
		//	component.BulletData = STGSystemData.PlayerBullet[type];
		//	component.Order = order;
		//	component.Direction = angle;

		//	if (init)
		//	{
		//		component.Init();
		//	}

		//	return (Object, component);
		//}

		//public (GameObject, EnemyShootEffect) NewEnemyShootEffect(int color, int order, Vector3 position, bool init = true, BlendMode blendMode = BlendMode.AlphaBlend)
		//{
		//	GameObject Object = NewObject(typeof(EnemyShootEffect), "EnemyShootEffect", Parent);

		//	if (!Object.TryGetComponent(out EnemyShootEffect component))
		//	{
		//		component = Object.AddComponent<EnemyShootEffect>();
		//		component.STGControler = this;
		//	}

		//	component.Position = position;
		//	component.Color = color;
		//	component.Order = order;

		//	if (init)
		//	{
		//		component.Init();
		//	}

		//	return (Object, component);
		//}

		//public (GameObject, ParticleSystemEffect) NewEnemyEndEffect(int order, Vector3 position, bool init = true)
		//{
		//	GameObject Object = NewObjectOfPrefab(typeof(ParticleSystemEffect), "EnemyEndEffect", Parent, EnemyEndPrefab);

		//	if (!Object.TryGetComponent(out ParticleSystemEffect component))
		//	{
		//		component = Object.AddComponent<ParticleSystemEffect>();
		//	}

		//	if (component.STGControler is null || component.STGControler != this)
		//	{
		//		component.STGControler = this;
		//	}

		//	component.Position = position;
		//	component.Order = order;

		//	if (init)
		//	{
		//		component.Init();
		//	}

		//	return (Object, component);
		//}

		public void SpellCardAttack(string name)
		{
			SpellCard spellCard = SpellCards[name];
		}
	}
}
