using System;
using System.Collections.Generic;

using Godot;

namespace NagaisoraFramework.STGSystem
{
	using DataSystem;

	/// <summary>
	/// STG动作记录与回放系统，能够记录玩家的输入，并在之后进行回放
	/// 动作记录与回放系统的核心是一个字典，键为游戏时间，值为玩家在该时间点的输入数据
	/// </summary>
	/// <param name="controler"></param>
	public class ReplaySystem(STGControler controler) : IDisposable
	{
		public STGControler STGControler = controler;

		public uint GameTime;

		public ReplayActionData[] ActionDatas;

		public bool IsRecording = false;
		public bool IsReplaying = false;

		public Vector2 LastVector = Vector2.Zero;
		public Dictionary<uint, ReplayActionData> Actions = null;

		public ushort LastKeys;
		public ushort DownKeys;

		public InputKey InputKey;

		public void Dispose()
		{
			
		}

		/// <summary>
		/// 更新方法，在游戏的每一帧调用，负责处理回放逻辑
		/// 在Replay模式下，根据当前的游戏时间从字典中获取玩家的输入数据，并将其通过事件反馈到STG控制器中，以实现回放效果
		/// </summary>
		public void OnUpdate()
		{
			if (IsReplaying)
			{
				if (Actions.TryGetValue(GameTime, out ReplayActionData data))
				{
					DownKeys = data.DownKeys;

					InputKey = new InputKey();
					InputKey.FromBinary(DownKeys);
				}

				STGControler.InputSystem.KeyDownEventCall(InputKey);
			}
		}

		/// <summary>
		/// 加入玩家输入按键的方法，在玩家输入发生时调用，负责记录玩家的输入
		/// 如果输入的按键与上一次记录的按键相同，则不进行记录，以节省存储空间
		/// 不建议每一帧都调用该方法，建议在玩家输入发生时调用，以减少不必要的性能开销
		/// </summary>
		/// <param name="inputKey">传入的按键输入数据</param>
		public void PushKey(InputKey inputKey)
        {
			if (IsRecording)
			{
				if (Actions.ContainsKey(GameTime))
				{
					return;
				}

				ushort nowkeys = inputKey.ToBinary();

				if (nowkeys == LastKeys)
				{
					return;
				}

				Actions.Add(GameTime, new ReplayActionData(GameTime, nowkeys));
				LastKeys = nowkeys;
			}
		}

		/// <summary>
		/// 开始记录的方法，STG控制器在非Replay模式下会自动在游戏开始时调用，负责初始化记录系统并开始记录玩家的输入
		/// </summary>
		public void RecordStart()
        {
            Actions = [];
			IsRecording = true;
        }

		/// <summary>
		/// 继续记录的方法，STG控制器在非Replay模式下会自动在游戏退出暂停时调用，该方法不会清空之前记录的数据，而是继续在之前的基础上记录玩家的输入，以便在游戏暂停后继续记录玩家的输入
		/// </summary>
		public void RecordContinue()
		{
			IsRecording = true;
		}

		/// <summary>
		/// 停止记录的方法，STG控制器在非Replay模式下会自动在游戏结束时调用，也会在游戏暂停时调用，负责停止记录玩家的输入并将记录的数据保存到ActionDatas数组中，以便之后进行回放
		/// </summary>
		public void RecordStop()
		{
            ActionDatas = [.. Actions.Values];
            IsRecording = false;
		}

		/// <summary>
		/// 回放开始的方法，STG控制器在Replay模式会自动在游戏开始时调用，负责初始化回放系统并开始回放玩家的输入
		/// </summary>
		public void ReplayStart()
        {
			if (ActionDatas == null)
			{
				return;
			}

			Actions = [];

			foreach(ReplayActionData actionData in ActionDatas)
			{
				Actions.Add(actionData.GameTime, actionData);
			}

			IsReplaying = true;
        }

		/// <summary>
		/// 继续回放的方法，STG控制器在Replay模式会自动在游戏退出暂停时调用，该方法不会清空之前回放的数据，而是继续在之前的基础上回放玩家的输入，以便在游戏暂停后继续回放玩家的输入
		/// </summary>
		public void ReplayContinue()
		{
			IsReplaying = true;
		}

		/// <summary>
		/// 停止回放的方法，STG控制器在Replay模式会自动在游戏结束时调用，也会在游戏暂停时调用，负责停止回放玩家的输入并清空回放系统的数据，以便之后进行新的回放
		/// </summary>
		public void ReplayStop()
		{
			IsReplaying = false;
		}
	}
}
