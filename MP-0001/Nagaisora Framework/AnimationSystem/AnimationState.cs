using System.Collections.Generic;

namespace NagaisoraFramework.AnimationSystem
{
	public class AnimationState
	{
		public string StateName;

		public List<AnimationSwitchCondition> Conditions;
	}
}