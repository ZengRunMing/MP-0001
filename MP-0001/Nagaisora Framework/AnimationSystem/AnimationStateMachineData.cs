using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NagaisoraFramework.AnimationSystem
{
	public class AnimationStateMachineData
	{
		public List<AnimationState> States;

		public AnimationStateMachineData()
		{
			States = [];
		}

		public AnimationStateMachineData(params AnimationState[] states)
		{
			States = [.. states];
		}

		public void AddState(AnimationState state)
		{
			States.Add(state);
		}

		public void AddStates(params AnimationState[] states)
		{
			States.AddRange(states);
		}

		public void RemoveState(AnimationState state)
		{
			States.Remove(state);
			foreach (var s in States)
			{
				s.Conditions.RemoveAll(c => c.TargetStateName == state.StateName);
			}
		}

		public void RemoveState(string stateName)
		{
			var state = States.FirstOrDefault(s => s.StateName == stateName);
			if (state != null)
			{
				RemoveState(state);
			}
		}

		public void RemoveStates(params AnimationState[] states)
		{
			foreach (var state in states)
			{
				RemoveState(state);
			}
		}

		public void RemoveStates(params string[] stateNames)
		{
			foreach (var stateName in stateNames)
			{
				RemoveState(stateName);
			}
		}

		public void AddCondition(AnimationState state, AnimationSwitchCondition condition)
		{
			if (!States.Contains(state))
			{
				throw new ArgumentException($"State '{state.StateName}' does not exist.");
			}
			state.Conditions.Add(condition);
		}

		public void AddCondition(string stateName, AnimationSwitchCondition condition)
		{
			var state = States.FirstOrDefault(s => s.StateName == stateName) ?? throw new ArgumentException($"State '{stateName}' does not exist.");
			
			AddCondition(state, condition);
		}

		public void RemoveCondition(AnimationState state, AnimationSwitchCondition condition)
		{
			if (!States.Contains(state))
			{
				throw new ArgumentException($"State '{state.StateName}' does not exist.");
			}
			state.Conditions.Remove(condition);
		}

		public void RemoveCondition(string stateName, AnimationSwitchCondition condition)
		{
			var state = States.FirstOrDefault(s => s.StateName == stateName) ?? throw new ArgumentException($"State '{stateName}' does not exist.");
			
			RemoveCondition(state, condition);
		}

		public void ClearStates()
		{
			States.Clear();
		}

		//public byte[] ToBinary()
		//{

		//}

		//public static AnimationStateMachineData FromBinary(byte[] data)
		//{
		//}
	}
}
