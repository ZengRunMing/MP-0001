using Godot;
using System;
using System.Collections.Generic;

namespace NagaisoraFramework.AnimationSystem
{
	public class AnimationStateMachine(AnimatedSprite3D spriteRenderer, AnimationStateMachineData data)
	{
		public AnimatedSprite3D AnimatedSpriteRenderer = spriteRenderer;

		public AnimationState CurrentState;

		public Dictionary<string, AnimationState> States = [];

		public void Update()
		{
			foreach (AnimationSwitchCondition condition in CurrentState.Conditions)
			{
				//if(condition.Condition.Invoke(this))
				//{
				//	SwitchState(condition.TargetStateName);
				//}
			}
		}

		public void SwitchState(string targetStateName)
		{
			if (!States.TryGetValue(targetStateName, out AnimationState targetState))
			{
				throw new ArgumentException($"State '{targetStateName}' does not exist.");
			}

			CurrentState = targetState;
			AnimatedSpriteRenderer.Animation = CurrentState.StateName;
		}
	}
}
