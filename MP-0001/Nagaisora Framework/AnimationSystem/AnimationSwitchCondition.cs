using System;

namespace NagaisoraFramework.AnimationSystem
{
	public class AnimationSwitchCondition(string targetStateName, bool conditionalSwitching)
	{
		public string TargetStateName = targetStateName;

		public bool ConditionalSwitching = conditionalSwitching;

		public bool ConditionState = false;
	}
}
